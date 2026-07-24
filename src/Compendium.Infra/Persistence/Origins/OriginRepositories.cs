using Compendium.Application.Origins;
using Compendium.Domain.Origins;
using Compendium.Domain.SharedKernel;
using Microsoft.EntityFrameworkCore;

namespace Compendium.Infra.Persistence.Origins;

internal sealed class SpeciesRepository(CompendiumDbContext dbContext) : ISpeciesRepository
{
    public async Task AddAsync(Species species, CancellationToken cancellationToken) => await dbContext.Species.AddAsync(species, cancellationToken);
    public Task<Species?> GetByCodeAsync(SpeciesCode code, CancellationToken cancellationToken) =>
        dbContext.Species.Include(entity => entity.Features).SingleOrDefaultAsync(entity => entity.Code == code, cancellationToken);
    public Task<bool> ExistsByCodeAsync(SpeciesCode code, CancellationToken cancellationToken) =>
        dbContext.Species.AnyAsync(entity => entity.Code == code, cancellationToken);
    public async Task<IReadOnlyCollection<Species>> ListAsync(CancellationToken cancellationToken) =>
        await dbContext.Species.OrderBy(entity => entity.Code).ToArrayAsync(cancellationToken);
    public Task SaveChangesAsync(CancellationToken cancellationToken) => dbContext.SaveChangesAsync(cancellationToken);
}

internal sealed class BackgroundRepository(CompendiumDbContext dbContext) : IBackgroundRepository
{
    public async Task AddAsync(Background background, CancellationToken cancellationToken) => await dbContext.Backgrounds.AddAsync(background, cancellationToken);
    public Task<Background?> GetByCodeAsync(BackgroundCode code, CancellationToken cancellationToken) =>
        Details().SingleOrDefaultAsync(entity => entity.Code == code, cancellationToken);
    public Task<bool> ExistsByCodeAsync(BackgroundCode code, CancellationToken cancellationToken) =>
        dbContext.Backgrounds.AnyAsync(entity => entity.Code == code, cancellationToken);
    public async Task<IReadOnlyCollection<Background>> ListAsync(CancellationToken cancellationToken) =>
        await dbContext.Backgrounds.OrderBy(entity => entity.Code).ToArrayAsync(cancellationToken);
    public Task SaveChangesAsync(CancellationToken cancellationToken) => dbContext.SaveChangesAsync(cancellationToken);
    private IQueryable<Background> Details() => dbContext.Backgrounds
        .Include(entity => entity.AbilityOptions)
        .Include(entity => entity.AbilityBoostRules)
        .Include(entity => entity.FeatGrants)
        .Include(entity => entity.SkillProficiencies)
        .Include(entity => entity.ToolProficiencies)
        .Include(entity => entity.StartingEquipmentRules)
        .Include(entity => entity.Features);
}

internal sealed class FeatRepository(CompendiumDbContext dbContext) : IFeatRepository
{
    public async Task AddAsync(Feat feat, CancellationToken cancellationToken) => await dbContext.Feats.AddAsync(feat, cancellationToken);
    public Task<Feat?> GetByIdAsync(CompendiumEntityId id, CancellationToken cancellationToken) =>
        dbContext.Feats.Include(entity => entity.Features).SingleOrDefaultAsync(entity => entity.Id == id, cancellationToken);
    public Task<Feat?> GetByCodeAsync(FeatCode code, CancellationToken cancellationToken) =>
        dbContext.Feats.Include(entity => entity.Features).SingleOrDefaultAsync(entity => entity.Code == code, cancellationToken);
    public Task<bool> ExistsByCodeAsync(FeatCode code, CancellationToken cancellationToken) =>
        dbContext.Feats.AnyAsync(entity => entity.Code == code, cancellationToken);
    public async Task<IReadOnlyCollection<Feat>> ListAsync(CancellationToken cancellationToken) =>
        await dbContext.Feats.OrderBy(entity => entity.Code).ToArrayAsync(cancellationToken);
    public Task SaveChangesAsync(CancellationToken cancellationToken) => dbContext.SaveChangesAsync(cancellationToken);
}
