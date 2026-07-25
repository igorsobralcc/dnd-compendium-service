using Compendium.Application.InternalQueries;
using Compendium.Domain.Features;
using Compendium.Domain.SharedKernel;
using Compendium.Infra.Persistence.Translations;
using Microsoft.EntityFrameworkCore;

namespace Compendium.Infra.Persistence.InternalQueries;

internal sealed class InternalCompendiumQueryGateway(CompendiumDbContext dbContext)
    : IInternalCompendiumQueryGateway
{
    public async Task<CharacterCreationOptionsV1> GetCharacterCreationOptionsAsync(
        CharacterCreationOptionsRequest request,
        CancellationToken cancellationToken)
    {
        var versionId = Id(request.SourceVersionId);
        var versionMatchesRuleset = await (
            from version in dbContext.SourceVersions.AsNoTracking()
            join source in dbContext.RuleSources.AsNoTracking() on version.RuleSourceId equals source.Id
            where version.Id == versionId && source.RulesetId == Id(request.RulesetId)
            select version.Id).AnyAsync(cancellationToken);

        if (!versionMatchesRuleset)
            return EmptyOptions(request);

        var classEntities = await dbContext.CharacterClasses.AsNoTracking()
            .Where(x => x.SourceVersionId == versionId)
            .OrderBy(x => x.Code)
            .ToArrayAsync(cancellationToken);
        var classes = classEntities.Select(x => new RawOption(x.Id.Value, x.Code.Value, x.Name.Value, "class")).ToArray();
        var methodEntities = await dbContext.AbilityScoreMethods.AsNoTracking()
            .Where(x => x.SourceVersionId == versionId)
            .OrderBy(x => x.Code)
            .ToArrayAsync(cancellationToken);
        var methods = methodEntities.Select(x => new RawOption(x.Id.Value, x.Code.Value, x.Name.Value, "ability_score_method")).ToArray();
        var proficiencyEntities = await dbContext.Proficiencies.AsNoTracking()
            .Where(x => x.SourceVersionId == versionId)
            .OrderBy(x => x.Code)
            .ToArrayAsync(cancellationToken);
        var proficiencies = proficiencyEntities.Select(x => new RawOption(x.Id.Value, x.Code.Value, x.Name.Value, "proficiency")).ToArray();
        var languageEntities = await dbContext.Languages.AsNoTracking()
            .Where(x => x.SourceVersionId == versionId)
            .OrderBy(x => x.Code)
            .ToArrayAsync(cancellationToken);
        var languages = languageEntities.Select(x => new RawOption(x.Id.Value, x.Code.Value, x.Name.Value, "language")).ToArray();
        var equipmentEntities = await dbContext.EquipmentItems.AsNoTracking()
            .Where(x => x.SourceVersionId == versionId)
            .OrderBy(x => x.Code)
            .ToArrayAsync(cancellationToken);
        var equipment = equipmentEntities.Select(x => new RawEquipment(
            x.Id.Value, x.Code.Value, x.Name.Value, x.Category.ToString(),
            x.CostAmount, x.CostCurrency.ToString())).ToArray();

        var allIds = classes.Select(x => x.Id)
            .Concat(methods.Select(x => x.Id))
            .Concat(proficiencies.Select(x => x.Id))
            .Concat(languages.Select(x => x.Id))
            .Concat(equipment.Select(x => x.Id))
            .ToArray();
        var names = await GetLocalizedNamesAsync(allIds, request.Locale, cancellationToken);

        return new(
            "v1", request.RulesetId, request.SourceVersionId, request.Locale, request.Level,
            MapOptions(classes, names), [], [], MapOptions(methods, names),
            MapOptions(proficiencies, names), MapOptions(languages, names),
            equipment.Select(x => new EquipmentOptionV1(
                x.Id, x.Code, names.GetValueOrDefault(x.Id, x.Name), x.Type, x.CostAmount, x.CostCurrency)).ToArray(),
            []);
    }

    public async Task<MechanicalEntityDetailsV1?> GetMechanicalEntityDetailsAsync(
        string entityType,
        Guid entityId,
        string locale,
        CancellationToken cancellationToken)
    {
        var normalized = entityType.Trim().ToLowerInvariant().Replace('-', '_');
        return normalized switch
        {
            "class" => await GetClassAsync(entityId, locale, cancellationToken),
            "feature" => await GetFeatureAsync(entityId, locale, cancellationToken),
            "equipment" or "equipment_item" => await GetEquipmentAsync(entityId, locale, cancellationToken),
            "choice_set" or "choiceset" => await GetChoiceSetAsync(entityId, cancellationToken),
            _ => null
        };
    }

    public async Task<CompendiumChangesV1> ListChangesAsync(
        CompendiumChangesRequest request,
        CancellationToken cancellationToken)
    {
        var page = Math.Max(1, request.Page);
        var pageSize = Math.Clamp(request.PageSize, 1, 200);
        var query = dbContext.CompendiumChanges.AsNoTracking().AsQueryable();
        if (request.SourceVersionId.HasValue)
            query = query.Where(x => x.SourceVersionId == request.SourceVersionId);
        if (!string.IsNullOrWhiteSpace(request.EntityType))
            query = query.Where(x => x.EntityType == request.EntityType.Trim().ToLowerInvariant().Replace('-', '_'));
        if (request.ChangedSince.HasValue)
            query = query.Where(x => x.ChangedAtUtc > request.ChangedSince.Value);
        if (request.Revision.HasValue)
            query = query.Where(x => x.Revision > request.Revision.Value);

        var total = await query.LongCountAsync(cancellationToken);
        var items = await query.OrderBy(x => x.Revision)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(x => new CompendiumChangeV1(
                x.Revision, x.SourceVersionId, x.EntityType, x.EntityId, x.ChangeType, x.ChangedAtUtc))
            .ToArrayAsync(cancellationToken);

        return new("v1", items, page, pageSize, total, items.LastOrDefault()?.Revision);
    }

    private async Task<MechanicalEntityDetailsV1?> GetClassAsync(Guid entityId, string locale, CancellationToken cancellationToken)
    {
        var entity = await dbContext.CharacterClasses.AsNoTracking()
            .Include(x => x.PrimaryAbilities)
            .Include(x => x.Levels).ThenInclude(x => x.SpellSlots)
            .Include(x => x.Levels).ThenInclude(x => x.ProficiencyGrants)
            .SingleOrDefaultAsync(x => x.Id == Id(entityId), cancellationToken);
        if (entity is null) return null;

        var name = await LocalizeAsync(entity.Id.Value, locale, "name", entity.Name.Value, cancellationToken);
        var description = await LocalizeAsync(entity.Id.Value, locale, "description", entity.Description?.Value, cancellationToken);
        var prerequisites = await GetPrerequisitesAsync(CompendiumEntityKind.Class, entity.Id, cancellationToken);
        return new(
            "v1", "class", entity.Id.Value, entity.Code.Value, name, description,
            new(entity.RuleSourceId.Value, entity.SourceVersionId.Value),
            new(
                entity.Levels.OrderBy(x => x.Level).Select(x => new ClassLevelMechanicsV1(
                    x.Level, x.ProficiencyBonus,
                    x.SpellSlots.OrderBy(s => s.SpellLevel).Select(s => new SpellSlotV1(s.SpellLevel, s.Slots)).ToArray(),
                    x.ProficiencyGrants.Select(p => p.ProficiencyId.Value).ToArray())).ToArray(),
                entity.PrimaryAbilities.OrderBy(x => x.SortOrder).Select(x => x.AbilityId.Value).ToArray()),
            null, null, null, prerequisites, []);
    }

    private async Task<MechanicalEntityDetailsV1?> GetFeatureAsync(Guid entityId, string locale, CancellationToken cancellationToken)
    {
        var entity = await dbContext.Features.AsNoTracking()
            .Include(x => x.Effects).ThenInclude(x => x.FieldValues)
            .Include(x => x.Effects).ThenInclude(x => x.Conditions)
            .SingleOrDefaultAsync(x => x.Id == Id(entityId), cancellationToken);
        if (entity is null) return null;

        var name = await LocalizeAsync(entity.Id.Value, locale, "name", entity.Name.Value, cancellationToken);
        var description = await LocalizeAsync(entity.Id.Value, locale, "description", entity.Description?.Value, cancellationToken);
        var prerequisites = await GetPrerequisitesAsync(CompendiumEntityKind.Feature, entity.Id, cancellationToken);
        var effects = entity.Effects.Select(effect => new EffectV1(
            effect.Id.Value, effect.Type.ToString(), effect.Target.ToString(),
            effect.FieldValues.Select(field => new TypedFieldV1(field.EffectSchemaFieldId.Value, MapValue(field.Value))).ToArray(),
            effect.Conditions.Select(condition => new TypedConditionV1(condition.Type.ToString(), MapValue(condition.Value))).ToArray())).ToArray();
        var choiceEntities = await dbContext.ChoiceSets.AsNoTracking()
            .Where(x => x.SourceEntityKind == CompendiumEntityKind.Feature && x.SourceEntityId == entity.Id)
            .ToArrayAsync(cancellationToken);
        var choices = choiceEntities.Select(x => new RelatedReferenceV1("choice_set", x.Id.Value)).ToArray();

        return new(
            "v1", "feature", entity.Id.Value, entity.Code.Value, name, description,
            new(entity.RuleSourceId.Value, entity.SourceVersionId.Value),
            null, new(entity.LevelRequirement, effects), null, null, prerequisites, choices);
    }

    private async Task<MechanicalEntityDetailsV1?> GetEquipmentAsync(Guid entityId, string locale, CancellationToken cancellationToken)
    {
        var entity = await dbContext.EquipmentItems.AsNoTracking()
            .SingleOrDefaultAsync(x => x.Id == Id(entityId), cancellationToken);
        if (entity is null) return null;
        var name = await LocalizeAsync(entity.Id.Value, locale, "name", entity.Name.Value, cancellationToken);
        var description = await LocalizeAsync(entity.Id.Value, locale, "description", entity.Description, cancellationToken);
        return new(
            "v1", "equipment", entity.Id.Value, entity.Code.Value, name, description,
            new(entity.RuleSourceId.Value, entity.SourceVersionId.Value),
            null, null,
            new(entity.Category.ToString(), entity.CostAmount, entity.CostCurrency.ToString(), entity.Weight, true),
            null, [], []);
    }

    private async Task<MechanicalEntityDetailsV1?> GetChoiceSetAsync(Guid entityId, CancellationToken cancellationToken)
    {
        var entity = await dbContext.ChoiceSets.AsNoTracking()
            .Include(x => x.Options)
            .Include(x => x.Filters)
            .SingleOrDefaultAsync(x => x.Id == Id(entityId), cancellationToken);
        if (entity is null) return null;

        var source = await ResolveSourceAsync(entity.SourceEntityKind, entity.SourceEntityId, cancellationToken);
        var prerequisites = await GetPrerequisitesAsync(CompendiumEntityKind.ChoiceSet, entity.Id, cancellationToken);
        var detail = new ChoiceSetMechanicsV1(
            entity.SourceEntityKind.ToString(),
            entity.SourceEntityId.Value,
            entity.MinimumChoices,
            entity.MaximumChoices,
            entity.Options.OrderBy(x => x.SortOrder).Select(x =>
                new ChoiceOptionV1(x.Id.Value, x.Type.ToString(), x.ReferenceId?.Value, x.DisplayText, x.SortOrder)).ToArray(),
            entity.Filters.Select(x => new ChoiceFilterV1(x.Id.Value, x.Type.ToString(), MapValue(x.Value))).ToArray());

        return new(
            "v1", "choice_set", entity.Id.Value, entity.Code.Value, entity.Code.Value, null,
            source, null, null, null, detail, prerequisites,
            [new(entity.SourceEntityKind.ToString().ToLowerInvariant(), entity.SourceEntityId.Value)]);
    }

    private async Task<SourceReferenceV1> ResolveSourceAsync(
        CompendiumEntityKind kind,
        CompendiumEntityId entityId,
        CancellationToken cancellationToken)
    {
        if (kind == CompendiumEntityKind.Class)
        {
            var source = await dbContext.CharacterClasses.AsNoTracking()
                .SingleOrDefaultAsync(x => x.Id == entityId, cancellationToken);
            if (source is not null) return new(source.RuleSourceId.Value, source.SourceVersionId.Value);
        }
        if (kind == CompendiumEntityKind.Feature)
        {
            var source = await dbContext.Features.AsNoTracking()
                .SingleOrDefaultAsync(x => x.Id == entityId, cancellationToken);
            if (source is not null) return new(source.RuleSourceId.Value, source.SourceVersionId.Value);
        }
        return new(Guid.Empty, Guid.Empty);
    }

    private async Task<IReadOnlyCollection<PrerequisiteV1>> GetPrerequisitesAsync(
        CompendiumEntityKind kind,
        CompendiumEntityId entityId,
        CancellationToken cancellationToken) =>
        (await dbContext.EntityPrerequisites.AsNoTracking()
            .Where(x => x.EntityKind == kind && x.EntityId == entityId)
            .ToArrayAsync(cancellationToken))
        .Select(x => new PrerequisiteV1(
            x.Id.Value, x.Type.ToString(), x.Operator.ToString(), x.Target.ToString(), MapValue(x.Value)))
        .ToArray();

    private async Task<Dictionary<Guid, string>> GetLocalizedNamesAsync(
        IReadOnlyCollection<Guid> entityIds,
        string locale,
        CancellationToken cancellationToken)
    {
        if (entityIds.Count == 0) return [];
        var normalizedLocale = Compendium.Domain.Translations.Locale.Create(NormalizeLocale(locale)).Value;
        var nameField = Compendium.Domain.Translations.TranslationField.Create("name").Value;
        var ids = entityIds.Select(Id).ToArray();
        var translations = await dbContext.Translations.AsNoTracking()
            .Where(x => ids.Contains(x.EntityId) && x.Locale == normalizedLocale && x.Field == nameField)
            .ToArrayAsync(cancellationToken);
        return translations.ToDictionary(x => x.EntityId.Value, x => x.Text.Value);
    }

    private async Task<string> LocalizeAsync(
        Guid entityId,
        string locale,
        string field,
        string? fallback,
        CancellationToken cancellationToken)
    {
        var normalizedLocale = Compendium.Domain.Translations.Locale.Create(NormalizeLocale(locale)).Value;
        var normalizedField = Compendium.Domain.Translations.TranslationField.Create(field).Value;
        var translation = await dbContext.Translations.AsNoTracking()
            .SingleOrDefaultAsync(
                x => x.EntityId == Id(entityId) && x.Locale == normalizedLocale && x.Field == normalizedField,
                cancellationToken);
        return translation?.Text.Value ?? fallback ?? string.Empty;
    }

    private static string NormalizeLocale(string locale) =>
        Compendium.Domain.Translations.Locale.Create(locale).IsSuccess
            ? Compendium.Domain.Translations.Locale.Create(locale).Value.Value
            : "en-US";

    private static TypedValueV1 MapValue(TypedMechanicalValue value) =>
        new(value.ValueType.ToString(), value.TextValue, value.NumericValue, value.BooleanValue, value.ReferenceId?.Value, value.EnumValue);

    private static IReadOnlyCollection<OptionV1> MapOptions(
        IEnumerable<RawOption> options,
        IReadOnlyDictionary<Guid, string> names) =>
        options.Select(x => new OptionV1(x.Id, x.Code, names.GetValueOrDefault(x.Id, x.Name))).ToArray();

    private static CharacterCreationOptionsV1 EmptyOptions(CharacterCreationOptionsRequest request) =>
        new("v1", request.RulesetId, request.SourceVersionId, request.Locale, request.Level, [], [], [], [], [], [], [], []);

    private static CompendiumEntityId Id(Guid value) => CompendiumEntityId.Create(value).Value;
    private sealed record RawOption(Guid Id, string Code, string Name, string EntityType);
    private sealed record RawEquipment(Guid Id, string Code, string Name, string Type, decimal CostAmount, string CostCurrency);
}
