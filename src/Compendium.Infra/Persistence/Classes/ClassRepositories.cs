using Compendium.Application.Classes;
using Compendium.Domain.Classes;
using Compendium.Domain.SharedKernel;
using Microsoft.EntityFrameworkCore;

namespace Compendium.Infra.Persistence.Classes;

internal sealed class CharacterClassRepository : ICharacterClassRepository
{
    private readonly CompendiumDbContext dbContext;

    public CharacterClassRepository(CompendiumDbContext dbContext) => this.dbContext = dbContext;

    public async Task AddAsync(CharacterClass characterClass, CancellationToken cancellationToken) =>
        await dbContext.CharacterClasses.AddAsync(characterClass, cancellationToken);

    public Task<CharacterClass?> GetByCodeAsync(ClassCode code, CancellationToken cancellationToken) =>
        ClassDetailsQuery().SingleOrDefaultAsync(characterClass => characterClass.Code == code, cancellationToken);

    public Task<bool> ExistsByCodeAsync(ClassCode code, CancellationToken cancellationToken) =>
        dbContext.CharacterClasses.AnyAsync(characterClass => characterClass.Code == code, cancellationToken);

    public async Task<IReadOnlyCollection<CharacterClass>> ListAsync(CancellationToken cancellationToken) =>
        await ClassDetailsQuery()
            .OrderBy(characterClass => characterClass.Code)
            .ToArrayAsync(cancellationToken);

    public Task SaveChangesAsync(CancellationToken cancellationToken) => dbContext.SaveChangesAsync(cancellationToken);

    private IQueryable<CharacterClass> ClassDetailsQuery() =>
        dbContext.CharacterClasses
            .Include(characterClass => characterClass.CoreTraits)
            .Include(characterClass => characterClass.PrimaryAbilities)
            .Include(characterClass => characterClass.Levels)
                .ThenInclude(level => level.SpellSlots)
            .Include(characterClass => characterClass.Levels)
                .ThenInclude(level => level.ProficiencyGrants)
            .Include(characterClass => characterClass.Levels)
                .ThenInclude(level => level.WeaponMasteryCounts)
            .Include(characterClass => characterClass.SpellcastingProgression)
                .ThenInclude(progression => progression!.LevelRules);
}

internal sealed class CharacterSubclassRepository : ICharacterSubclassRepository
{
    private readonly CompendiumDbContext dbContext;

    public CharacterSubclassRepository(CompendiumDbContext dbContext) => this.dbContext = dbContext;

    public async Task AddAsync(CharacterSubclass subclass, CancellationToken cancellationToken) =>
        await dbContext.CharacterSubclasses.AddAsync(subclass, cancellationToken);

    public Task<CharacterSubclass?> GetByClassAndCodeAsync(
        CompendiumEntityId characterClassId,
        ClassCode code,
        CancellationToken cancellationToken) =>
        SubclassDetailsQuery()
            .SingleOrDefaultAsync(
                subclass => subclass.CharacterClassId == characterClassId && subclass.Code == code,
                cancellationToken);

    public Task<bool> ExistsByClassAndCodeAsync(
        CompendiumEntityId characterClassId,
        ClassCode code,
        CancellationToken cancellationToken) =>
        dbContext.CharacterSubclasses.AnyAsync(
            subclass => subclass.CharacterClassId == characterClassId && subclass.Code == code,
            cancellationToken);

    public async Task<IReadOnlyCollection<CharacterSubclass>> ListByClassAsync(
        CompendiumEntityId characterClassId,
        CancellationToken cancellationToken) =>
        await SubclassDetailsQuery()
            .Where(subclass => subclass.CharacterClassId == characterClassId)
            .OrderBy(subclass => subclass.Code)
            .ToArrayAsync(cancellationToken);

    public Task SaveChangesAsync(CancellationToken cancellationToken) => dbContext.SaveChangesAsync(cancellationToken);

    private IQueryable<CharacterSubclass> SubclassDetailsQuery() =>
        dbContext.CharacterSubclasses.Include(subclass => subclass.Features);
}
