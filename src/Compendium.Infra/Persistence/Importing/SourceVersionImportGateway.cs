using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Compendium.Application.Importing;
using Compendium.Domain.Equipment;
using Compendium.Domain.Fundamentals;
using Compendium.Domain.Importing;
using Compendium.Infra.Persistence.Integration;
using Microsoft.EntityFrameworkCore;

namespace Compendium.Infra.Persistence.Importing;

public sealed class SourceVersionImportGateway : ISourceVersionImportGateway, ISourceVersionValidationGateway
{
    private readonly CompendiumDbContext db;
    private readonly TimeProvider timeProvider;

    public SourceVersionImportGateway(CompendiumDbContext db, TimeProvider timeProvider)
    {
        this.db = db;
        this.timeProvider = timeProvider;
    }

    public async Task<ImportSourceVersionResult> ImportAsync(ImportSourceVersionCommand command, CancellationToken cancellationToken)
    {
        var previous = await db.SourceVersionImports.AsNoTracking().SingleOrDefaultAsync(x => x.SourceVersionId == command.SourceVersionId, cancellationToken);
        if (previous is not null)
            return new(previous.Id, previous.SourceVersionId, true, previous.ImportedEntityCount);

        var version = await db.SourceVersions.SingleOrDefaultAsync(x => x.Id.Value == command.SourceVersionId, cancellationToken)
            ?? throw new InvalidOperationException("Source version was not found.");
        var sourceExists = await db.RuleSources.AnyAsync(x => x.Id == version.RuleSourceId, cancellationToken);
        if (!sourceExists) throw new InvalidOperationException("Rule source was not found.");
        var rulesetExists = await db.RuleSources.Where(x => x.Id == version.RuleSourceId)
            .Join(db.Rulesets, source => source.RulesetId, ruleset => ruleset.Id, (_, _) => 1)
            .AnyAsync(cancellationToken);
        if (!rulesetExists) throw new InvalidOperationException("Ruleset was not found.");

        await using var transaction = await db.Database.BeginTransactionAsync(cancellationToken);
        var now = timeProvider.GetUtcNow();
        var count = 0;
        var abilityIds = new Dictionary<string, Domain.SharedKernel.CompendiumEntityId>(StringComparer.OrdinalIgnoreCase);

        foreach (var seed in command.Abilities.OrderBy(x => x.Code, StringComparer.Ordinal))
        {
            var code = Required(AbilityCode.Create(seed.Code));
            var existing = await db.Abilities.SingleOrDefaultAsync(x => x.Code == code, cancellationToken);
            if (existing is null)
            {
                existing = Required(Ability.Create(version.RuleSourceId, version.Id, code, Required(DisplayName.Create(seed.Name)), now));
                db.Abilities.Add(existing);
                count++;
            }
            abilityIds[code.Value] = existing.Id;
        }

        foreach (var seed in command.Skills.OrderBy(x => x.Code, StringComparer.Ordinal))
        {
            var code = Required(SkillCode.Create(seed.Code));
            if (await db.Skills.AnyAsync(x => x.Code == code, cancellationToken)) continue;
            if (!abilityIds.TryGetValue(seed.DefaultAbilityCode, out var abilityId))
            {
                var abilityCode = Required(AbilityCode.Create(seed.DefaultAbilityCode));
                abilityId = (await db.Abilities.SingleOrDefaultAsync(x => x.Code == abilityCode, cancellationToken))?.Id
                    ?? throw new InvalidOperationException($"Skill '{seed.Code}' references unknown ability '{seed.DefaultAbilityCode}'.");
            }
            db.Skills.Add(Required(Skill.Create(version.RuleSourceId, version.Id, code, Required(DisplayName.Create(seed.Name)), abilityId, now)));
            count++;
        }

        foreach (var seed in command.Languages.OrderBy(x => x.Code, StringComparer.Ordinal))
        {
            var code = Required(LanguageCode.Create(seed.Code));
            if (await db.Languages.AnyAsync(x => x.Code == code, cancellationToken)) continue;
            db.Languages.Add(Required(Language.Create(version.RuleSourceId, version.Id, code, Required(DisplayName.Create(seed.Name)), now)));
            count++;
        }

        foreach (var seed in command.Proficiencies.OrderBy(x => x.Code, StringComparer.Ordinal))
        {
            var code = Required(ProficiencyCode.Create(seed.Code));
            if (await db.Proficiencies.AnyAsync(x => x.Code == code, cancellationToken)) continue;
            db.Proficiencies.Add(Required(Proficiency.Create(version.RuleSourceId, version.Id, code, Required(DisplayName.Create(seed.Name)), seed.Type, null, now)));
            count++;
        }

        foreach (var die in command.HitDice.Distinct().Order())
        {
            var code = Required(HitDieCode.Create(die));
            if (await db.HitDice.AnyAsync(x => x.Code == code, cancellationToken)) continue;
            db.HitDice.Add(Required(HitDie.Create(version.RuleSourceId, version.Id, die, now)));
            count++;
        }

        foreach (var seed in command.Equipment.OrderBy(x => x.Code, StringComparer.Ordinal))
        {
            var code = Required(EquipmentCode.Create(seed.Code));
            if (await db.EquipmentItems.AnyAsync(x => x.Code == code, cancellationToken)) continue;
            if (!Enum.TryParse<EquipmentCategory>(seed.Category, true, out var category))
                throw new InvalidOperationException($"Equipment '{seed.Code}' has invalid category '{seed.Category}'.");
            if (!Enum.TryParse<Currency>(seed.Currency, true, out var currency))
                throw new InvalidOperationException($"Equipment '{seed.Code}' has invalid currency '{seed.Currency}'.");
            db.EquipmentItems.Add(Required(EquipmentItem.Create(version.RuleSourceId, version.Id, code, Required(EquipmentName.Create(seed.Name)),
                category, Required(Weight.Create(seed.Weight)), Required(Cost.Create(seed.CostAmount, currency)), seed.Description, now)));
            count++;
        }

        var hash = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(command))));
        var import = new SourceVersionImportRecord(command.SourceVersionId, hash, count, now);
        db.SourceVersionImports.Add(import);
        var outbox = new IntegrationOutbox("compendium.source-version-imported.v1", 1, "SourceVersion", command.SourceVersionId.ToString(), command.CorrelationId, now);
        outbox.Fields.Add(new IntegrationOutboxField(outbox.Id, "sourceVersionId", "reference", now, referenceValue: command.SourceVersionId.ToString()));
        outbox.Fields.Add(new IntegrationOutboxField(outbox.Id, "importId", "reference", now, referenceValue: import.Id.ToString()));
        db.IntegrationOutbox.Add(outbox);
        await db.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        return new(import.Id, command.SourceVersionId, false, count);
    }

    public async Task<SourceVersionContentSummary?> GetSummaryAsync(Guid sourceVersionId, CancellationToken cancellationToken)
    {
        if (!await db.SourceVersions.AnyAsync(x => x.Id.Value == sourceVersionId, cancellationToken)) return null;
        return new(
            await db.Abilities.CountAsync(x => x.SourceVersionId.Value == sourceVersionId, cancellationToken),
            await db.Skills.CountAsync(x => x.SourceVersionId.Value == sourceVersionId, cancellationToken),
            await db.Languages.CountAsync(x => x.SourceVersionId.Value == sourceVersionId, cancellationToken),
            await db.Proficiencies.CountAsync(x => x.SourceVersionId.Value == sourceVersionId, cancellationToken),
            await db.HitDice.CountAsync(x => x.SourceVersionId.Value == sourceVersionId, cancellationToken),
            await db.CharacterClasses.CountAsync(x => x.SourceVersionId.Value == sourceVersionId, cancellationToken),
            await db.ClassLevels.CountAsync(x => db.CharacterClasses.Where(c => c.SourceVersionId.Value == sourceVersionId).Select(c => c.Id).Contains(x.CharacterClassId), cancellationToken),
            await db.EquipmentItems.CountAsync(x => x.SourceVersionId.Value == sourceVersionId, cancellationToken),
            0, 0, 0, 0, 0, 0, 0, 0);
    }

    public async Task<IReadOnlyCollection<ValidationIssueDto>> ReplaceIssuesAsync(Guid sourceVersionId, IReadOnlyCollection<ConsistencyIssue> issues, CancellationToken cancellationToken)
    {
        var existing = await db.SourceVersionValidationIssues.Where(x => x.SourceVersionId == sourceVersionId).ToListAsync(cancellationToken);
        db.SourceVersionValidationIssues.RemoveRange(existing);
        var entities = issues.Select(x => new SourceVersionValidationIssue(sourceVersionId, x, timeProvider.GetUtcNow())).ToArray();
        db.SourceVersionValidationIssues.AddRange(entities);
        if (issues.All(x => x.Severity != ValidationIssueSeverity.Blocker))
        {
            var version = await db.SourceVersions.SingleAsync(x => x.Id.Value == sourceVersionId, cancellationToken);
            version.MarkAsImported(timeProvider.GetUtcNow());
        }
        await db.SaveChangesAsync(cancellationToken);
        return entities.Select(ToDto).ToArray();
    }

    public async Task<IReadOnlyCollection<ValidationIssueDto>> ListIssuesAsync(Guid sourceVersionId, CancellationToken cancellationToken) =>
        await db.SourceVersionValidationIssues.AsNoTracking().Where(x => x.SourceVersionId == sourceVersionId)
            .OrderBy(x => x.Severity).ThenBy(x => x.Code).Select(x => new ValidationIssueDto(x.Id, x.Code, x.Severity, x.Message)).ToArrayAsync(cancellationToken);

    private static ValidationIssueDto ToDto(SourceVersionValidationIssue issue) => new(issue.Id, issue.Code, issue.Severity, issue.Message);
    private static T Required<T>(Domain.SharedKernel.Result<T> result) => result.IsSuccess ? result.Value : throw new InvalidOperationException(result.Error.Message);
}
