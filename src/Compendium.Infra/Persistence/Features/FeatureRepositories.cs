using Compendium.Application.Features;
using Compendium.Domain.Features;
using Compendium.Domain.SharedKernel;
using Microsoft.EntityFrameworkCore;

namespace Compendium.Infra.Persistence.Features;

internal sealed class FeatureRepository : IFeatureRepository
{
    private readonly CompendiumDbContext dbContext;

    public FeatureRepository(CompendiumDbContext dbContext) => this.dbContext = dbContext;

    public async Task AddAsync(Feature feature, CancellationToken cancellationToken) =>
        await dbContext.Features.AddAsync(feature, cancellationToken);

    public Task<bool> ExistsByIdAsync(CompendiumEntityId id, CancellationToken cancellationToken) =>
        dbContext.Features.AnyAsync(feature => feature.Id == id, cancellationToken);

    public Task<Feature?> GetByCodeAsync(FeatureCode code, CancellationToken cancellationToken) =>
        DetailsQuery().SingleOrDefaultAsync(feature => feature.Code == code, cancellationToken);

    public Task<bool> ExistsByCodeAsync(FeatureCode code, CancellationToken cancellationToken) =>
        dbContext.Features.AnyAsync(feature => feature.Code == code, cancellationToken);

    public async Task<IReadOnlyCollection<Feature>> ListAsync(CancellationToken cancellationToken) =>
        await DetailsQuery().OrderBy(feature => feature.Code).ToArrayAsync(cancellationToken);

    public Task SaveChangesAsync(CancellationToken cancellationToken) => dbContext.SaveChangesAsync(cancellationToken);

    private IQueryable<Feature> DetailsQuery() =>
        dbContext.Features
            .Include(feature => feature.Effects)
                .ThenInclude(effect => effect.FieldValues)
            .Include(feature => feature.Effects)
                .ThenInclude(effect => effect.Conditions);
}

internal sealed class EffectSchemaRepository : IEffectSchemaRepository
{
    private readonly CompendiumDbContext dbContext;

    public EffectSchemaRepository(CompendiumDbContext dbContext) => this.dbContext = dbContext;

    public async Task AddAsync(EffectSchema schema, CancellationToken cancellationToken) =>
        await dbContext.EffectSchemas.AddAsync(schema, cancellationToken);

    public Task<EffectSchema?> GetByCodeAsync(FeatureCode code, CancellationToken cancellationToken) =>
        dbContext.EffectSchemas.Include(schema => schema.Fields).SingleOrDefaultAsync(schema => schema.Code == code, cancellationToken);

    public Task SaveChangesAsync(CancellationToken cancellationToken) => dbContext.SaveChangesAsync(cancellationToken);
}

internal sealed class EntityPrerequisiteRepository : IEntityPrerequisiteRepository
{
    private readonly CompendiumDbContext dbContext;

    public EntityPrerequisiteRepository(CompendiumDbContext dbContext) => this.dbContext = dbContext;

    public async Task AddAsync(EntityPrerequisite prerequisite, CancellationToken cancellationToken) =>
        await dbContext.EntityPrerequisites.AddAsync(prerequisite, cancellationToken);

    public void Remove(EntityPrerequisite prerequisite) => dbContext.EntityPrerequisites.Remove(prerequisite);

    public Task<EntityPrerequisite?> GetByIdAsync(CompendiumEntityId id, CancellationToken cancellationToken) =>
        dbContext.EntityPrerequisites.SingleOrDefaultAsync(prerequisite => prerequisite.Id == id, cancellationToken);

    public async Task<IReadOnlyCollection<EntityPrerequisite>> ListByEntityAsync(CompendiumEntityKind entityKind, CompendiumEntityId entityId, CancellationToken cancellationToken) =>
        await dbContext.EntityPrerequisites
            .Where(prerequisite => prerequisite.EntityKind == entityKind && prerequisite.EntityId == entityId)
            .OrderBy(prerequisite => prerequisite.Type)
            .ToArrayAsync(cancellationToken);

    public Task SaveChangesAsync(CancellationToken cancellationToken) => dbContext.SaveChangesAsync(cancellationToken);
}

internal sealed class ChoiceSetRepository : IChoiceSetRepository
{
    private readonly CompendiumDbContext dbContext;

    public ChoiceSetRepository(CompendiumDbContext dbContext) => this.dbContext = dbContext;

    public async Task AddAsync(ChoiceSet choiceSet, CancellationToken cancellationToken) =>
        await dbContext.ChoiceSets.AddAsync(choiceSet, cancellationToken);

    public Task<ChoiceSet?> GetByCodeAsync(ChoiceSetCode code, CancellationToken cancellationToken) =>
        DetailsQuery().SingleOrDefaultAsync(choiceSet => choiceSet.Code == code, cancellationToken);

    public Task<bool> ExistsByCodeAsync(ChoiceSetCode code, CancellationToken cancellationToken) =>
        dbContext.ChoiceSets.AnyAsync(choiceSet => choiceSet.Code == code, cancellationToken);

    public async Task<IReadOnlyCollection<ChoiceSet>> ListBySourceEntityAsync(CompendiumEntityKind entityKind, CompendiumEntityId entityId, CancellationToken cancellationToken) =>
        await DetailsQuery()
            .Where(choiceSet => choiceSet.SourceEntityKind == entityKind && choiceSet.SourceEntityId == entityId)
            .OrderBy(choiceSet => choiceSet.Code)
            .ToArrayAsync(cancellationToken);

    public Task SaveChangesAsync(CancellationToken cancellationToken) => dbContext.SaveChangesAsync(cancellationToken);

    private IQueryable<ChoiceSet> DetailsQuery() =>
        dbContext.ChoiceSets
            .Include(choiceSet => choiceSet.Filters)
            .Include(choiceSet => choiceSet.Options);
}
