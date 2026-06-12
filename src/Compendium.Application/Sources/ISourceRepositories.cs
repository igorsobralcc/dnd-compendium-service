using Compendium.Domain.SharedKernel;
using Compendium.Domain.Sources;

namespace Compendium.Application.Sources;

public interface IRulesetRepository
{
    Task AddAsync(Ruleset ruleset, CancellationToken cancellationToken);

    Task<Ruleset?> GetByIdAsync(CompendiumEntityId id, CancellationToken cancellationToken);

    Task<Ruleset?> GetByCodeAsync(RulesetCode code, CancellationToken cancellationToken);

    Task<bool> ExistsByCodeAsync(RulesetCode code, CancellationToken cancellationToken);

    Task SaveChangesAsync(CancellationToken cancellationToken);
}

public interface IRuleSourceRepository
{
    Task AddAsync(RuleSource source, CancellationToken cancellationToken);

    Task<RuleSource?> GetByIdAsync(CompendiumEntityId id, CancellationToken cancellationToken);

    Task<bool> ExistsByRulesetAndCodeAsync(CompendiumEntityId rulesetId, SourceCode code, CancellationToken cancellationToken);

    Task<IReadOnlyCollection<RuleSource>> ListByRulesetAsync(
        CompendiumEntityId rulesetId,
        bool includeInactive,
        CancellationToken cancellationToken);

    Task SaveChangesAsync(CancellationToken cancellationToken);
}

public interface ISourceVersionRepository
{
    Task AddAsync(SourceVersion version, CancellationToken cancellationToken);

    Task<SourceVersion?> GetByIdAsync(CompendiumEntityId id, CancellationToken cancellationToken);

    Task<bool> ExistsBySourceAndVersionAsync(
        CompendiumEntityId ruleSourceId,
        SourceVersionNumber versionNumber,
        CancellationToken cancellationToken);

    Task<IReadOnlyCollection<SourceVersion>> ListBySourceAsync(
        CompendiumEntityId ruleSourceId,
        CancellationToken cancellationToken);

    Task<SourceVersion?> GetCurrentBySourceAsync(CompendiumEntityId ruleSourceId, CancellationToken cancellationToken);

    Task SaveChangesAsync(CancellationToken cancellationToken);
}
