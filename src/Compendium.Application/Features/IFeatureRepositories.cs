using Compendium.Domain.Features;
using Compendium.Domain.SharedKernel;

namespace Compendium.Application.Features;

public interface IFeatureRepository
{
    Task AddAsync(Feature feature, CancellationToken cancellationToken);
    Task<Feature?> GetByCodeAsync(FeatureCode code, CancellationToken cancellationToken);
    Task<bool> ExistsByCodeAsync(FeatureCode code, CancellationToken cancellationToken);
    Task<IReadOnlyCollection<Feature>> ListAsync(CancellationToken cancellationToken);
    Task SaveChangesAsync(CancellationToken cancellationToken);
}

public interface IEffectSchemaRepository
{
    Task AddAsync(EffectSchema schema, CancellationToken cancellationToken);
    Task<EffectSchema?> GetByCodeAsync(FeatureCode code, CancellationToken cancellationToken);
    Task SaveChangesAsync(CancellationToken cancellationToken);
}

public interface IEntityPrerequisiteRepository
{
    Task AddAsync(EntityPrerequisite prerequisite, CancellationToken cancellationToken);
    void Remove(EntityPrerequisite prerequisite);
    Task<EntityPrerequisite?> GetByIdAsync(CompendiumEntityId id, CancellationToken cancellationToken);
    Task<IReadOnlyCollection<EntityPrerequisite>> ListByEntityAsync(CompendiumEntityKind entityKind, CompendiumEntityId entityId, CancellationToken cancellationToken);
    Task SaveChangesAsync(CancellationToken cancellationToken);
}

public interface IChoiceSetRepository
{
    Task AddAsync(ChoiceSet choiceSet, CancellationToken cancellationToken);
    Task<ChoiceSet?> GetByCodeAsync(ChoiceSetCode code, CancellationToken cancellationToken);
    Task<bool> ExistsByCodeAsync(ChoiceSetCode code, CancellationToken cancellationToken);
    Task<IReadOnlyCollection<ChoiceSet>> ListBySourceEntityAsync(CompendiumEntityKind entityKind, CompendiumEntityId entityId, CancellationToken cancellationToken);
    Task SaveChangesAsync(CancellationToken cancellationToken);
}
