using Compendium.Domain.Classes;
using Compendium.Domain.SharedKernel;

namespace Compendium.Application.Classes;

public interface ICharacterClassRepository
{
    Task AddAsync(CharacterClass characterClass, CancellationToken cancellationToken);

    Task<CharacterClass?> GetByCodeAsync(ClassCode code, CancellationToken cancellationToken);

    Task<bool> ExistsByCodeAsync(ClassCode code, CancellationToken cancellationToken);

    Task<IReadOnlyCollection<CharacterClass>> ListAsync(CancellationToken cancellationToken);

    Task SaveChangesAsync(CancellationToken cancellationToken);
}

public interface ICharacterSubclassRepository
{
    Task AddAsync(CharacterSubclass subclass, CancellationToken cancellationToken);

    Task<CharacterSubclass?> GetByClassAndCodeAsync(
        CompendiumEntityId characterClassId,
        ClassCode code,
        CancellationToken cancellationToken);

    Task<bool> ExistsByClassAndCodeAsync(
        CompendiumEntityId characterClassId,
        ClassCode code,
        CancellationToken cancellationToken);

    Task<IReadOnlyCollection<CharacterSubclass>> ListByClassAsync(
        CompendiumEntityId characterClassId,
        CancellationToken cancellationToken);

    Task SaveChangesAsync(CancellationToken cancellationToken);
}
