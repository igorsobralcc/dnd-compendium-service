using Compendium.Application.Sources;
using Compendium.Domain.SharedKernel;
using Compendium.Domain.Sources;
using Microsoft.EntityFrameworkCore;

namespace Compendium.Infra.Persistence.Sources;

internal sealed class RulesetRepository : IRulesetRepository
{
    private readonly CompendiumDbContext dbContext;

    public RulesetRepository(CompendiumDbContext dbContext) => this.dbContext = dbContext;

    public async Task AddAsync(Ruleset ruleset, CancellationToken cancellationToken) =>
        await dbContext.Rulesets.AddAsync(ruleset, cancellationToken);

    public Task<Ruleset?> GetByIdAsync(CompendiumEntityId id, CancellationToken cancellationToken) =>
        dbContext.Rulesets.SingleOrDefaultAsync(ruleset => ruleset.Id == id, cancellationToken);

    public Task<Ruleset?> GetByCodeAsync(RulesetCode code, CancellationToken cancellationToken) =>
        dbContext.Rulesets.SingleOrDefaultAsync(ruleset => ruleset.Code == code, cancellationToken);

    public Task<bool> ExistsByCodeAsync(RulesetCode code, CancellationToken cancellationToken) =>
        dbContext.Rulesets.AnyAsync(ruleset => ruleset.Code == code, cancellationToken);

    public Task SaveChangesAsync(CancellationToken cancellationToken) => dbContext.SaveChangesAsync(cancellationToken);
}

internal sealed class RuleSourceRepository : IRuleSourceRepository
{
    private readonly CompendiumDbContext dbContext;

    public RuleSourceRepository(CompendiumDbContext dbContext) => this.dbContext = dbContext;

    public async Task AddAsync(RuleSource source, CancellationToken cancellationToken) =>
        await dbContext.RuleSources.AddAsync(source, cancellationToken);

    public Task<RuleSource?> GetByIdAsync(CompendiumEntityId id, CancellationToken cancellationToken) =>
        dbContext.RuleSources.SingleOrDefaultAsync(source => source.Id == id, cancellationToken);

    public Task<bool> ExistsByRulesetAndCodeAsync(CompendiumEntityId rulesetId, SourceCode code, CancellationToken cancellationToken) =>
        dbContext.RuleSources.AnyAsync(source => source.RulesetId == rulesetId && source.Code == code, cancellationToken);

    public async Task<IReadOnlyCollection<RuleSource>> ListByRulesetAsync(
        CompendiumEntityId rulesetId,
        bool includeInactive,
        CancellationToken cancellationToken)
    {
        var query = dbContext.RuleSources.Where(source => source.RulesetId == rulesetId);
        if (!includeInactive)
        {
            query = query.Where(source => source.Status == SourceStatus.Active);
        }

        return await query.OrderBy(source => source.Code).ToArrayAsync(cancellationToken);
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken) => dbContext.SaveChangesAsync(cancellationToken);
}

internal sealed class SourceVersionRepository : ISourceVersionRepository
{
    private readonly CompendiumDbContext dbContext;

    public SourceVersionRepository(CompendiumDbContext dbContext) => this.dbContext = dbContext;

    public async Task AddAsync(SourceVersion version, CancellationToken cancellationToken) =>
        await dbContext.SourceVersions.AddAsync(version, cancellationToken);

    public Task<bool> ExistsBySourceAndVersionAsync(
        CompendiumEntityId ruleSourceId,
        SourceVersionNumber versionNumber,
        CancellationToken cancellationToken) =>
        dbContext.SourceVersions.AnyAsync(
            version => version.RuleSourceId == ruleSourceId && version.VersionNumber == versionNumber,
            cancellationToken);

    public async Task<IReadOnlyCollection<SourceVersion>> ListBySourceAsync(
        CompendiumEntityId ruleSourceId,
        CancellationToken cancellationToken) =>
        await dbContext.SourceVersions
            .Where(version => version.RuleSourceId == ruleSourceId)
            .OrderBy(version => version.PublicationDate)
            .ToArrayAsync(cancellationToken);

    public Task<SourceVersion?> GetCurrentBySourceAsync(CompendiumEntityId ruleSourceId, CancellationToken cancellationToken) =>
        dbContext.SourceVersions.SingleOrDefaultAsync(
            version => version.RuleSourceId == ruleSourceId && version.IsCurrent,
            cancellationToken);

    public Task SaveChangesAsync(CancellationToken cancellationToken) => dbContext.SaveChangesAsync(cancellationToken);
}
