using Compendium.Domain.Origins;
using Compendium.Domain.SharedKernel;

namespace Compendium.Application.Origins;

public interface ISpeciesRepository
{
    Task AddAsync(Species species, CancellationToken cancellationToken);
    Task<Species?> GetByCodeAsync(SpeciesCode code, CancellationToken cancellationToken);
    Task<bool> ExistsByCodeAsync(SpeciesCode code, CancellationToken cancellationToken);
    Task<IReadOnlyCollection<Species>> ListAsync(CancellationToken cancellationToken);
    Task SaveChangesAsync(CancellationToken cancellationToken);
}

public interface IBackgroundRepository
{
    Task AddAsync(Background background, CancellationToken cancellationToken);
    Task<Background?> GetByCodeAsync(BackgroundCode code, CancellationToken cancellationToken);
    Task<bool> ExistsByCodeAsync(BackgroundCode code, CancellationToken cancellationToken);
    Task<IReadOnlyCollection<Background>> ListAsync(CancellationToken cancellationToken);
    Task SaveChangesAsync(CancellationToken cancellationToken);
}

public interface IFeatRepository
{
    Task AddAsync(Feat feat, CancellationToken cancellationToken);
    Task<Feat?> GetByIdAsync(CompendiumEntityId id, CancellationToken cancellationToken);
    Task<Feat?> GetByCodeAsync(FeatCode code, CancellationToken cancellationToken);
    Task<bool> ExistsByCodeAsync(FeatCode code, CancellationToken cancellationToken);
    Task<IReadOnlyCollection<Feat>> ListAsync(CancellationToken cancellationToken);
    Task SaveChangesAsync(CancellationToken cancellationToken);
}
