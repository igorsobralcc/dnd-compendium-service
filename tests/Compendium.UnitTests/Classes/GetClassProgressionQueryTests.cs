using Compendium.Application.Classes;
using Compendium.Domain.Classes;
using Compendium.Domain.SharedKernel;

namespace Compendium.UnitTests.Classes;

public sealed class GetClassProgressionQueryTests
{
    [Fact]
    public async Task Query_returns_levels_and_spellcasting_progression()
    {
        var characterClass = CreateSpellcastingClass();
        var query = new GetClassProgressionQuery(new StubCharacterClassRepository(characterClass));

        var result = await query.ExecuteAsync("wizard", CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Single(result.Value.Levels);
        Assert.NotNull(result.Value.SpellcastingProgression);
        Assert.Equal(ClassSpellcastingProgressionType.FullCaster, result.Value.SpellcastingProgression.Type);
        Assert.Single(result.Value.SpellcastingProgression.LevelRules);
    }

    private static CharacterClass CreateSpellcastingClass()
    {
        var characterClass = CharacterClass.Create(
            CompendiumEntityId.New(),
            CompendiumEntityId.New(),
            ClassCode.Create("WIZARD").Value,
            ClassName.Create("Wizard").Value,
            ClassDescription.CreateOptional("Arcane spellcaster").Value,
            new ClassCoreTraitsInput(CompendiumEntityId.New(), null, 2),
            [CompendiumEntityId.New()],
            [new ClassLevelInput(1, 2, [new ClassLevelSpellSlotInput(1, 2)], [], null)],
            DateTimeOffset.UtcNow).Value;

        characterClass.ConfigureProgression(
            [new ClassLevelInput(1, 2, [new ClassLevelSpellSlotInput(1, 2)], [], null)],
            new ClassSpellcastingProgressionInput(
                ClassSpellcastingProgressionType.FullCaster,
                CompendiumEntityId.New(),
                [new ClassSpellcastingLevelRuleInput(1, 1)]),
            CompendiumEntityId.New(),
            DateTimeOffset.UtcNow);

        return characterClass;
    }

    private sealed class StubCharacterClassRepository : ICharacterClassRepository
    {
        private readonly CharacterClass? characterClass;

        public StubCharacterClassRepository(CharacterClass? characterClass) => this.characterClass = characterClass;

        public Task AddAsync(CharacterClass characterClass, CancellationToken cancellationToken) =>
            Task.CompletedTask;

        public Task<CharacterClass?> GetByCodeAsync(ClassCode code, CancellationToken cancellationToken) =>
            Task.FromResult(characterClass is not null && characterClass.Code == code ? characterClass : null);

        public Task<bool> ExistsByCodeAsync(ClassCode code, CancellationToken cancellationToken) =>
            Task.FromResult(characterClass is not null && characterClass.Code == code);

        public Task<IReadOnlyCollection<CharacterClass>> ListAsync(CancellationToken cancellationToken) =>
            Task.FromResult<IReadOnlyCollection<CharacterClass>>(characterClass is null ? [] : [characterClass]);

        public Task SaveChangesAsync(CancellationToken cancellationToken) =>
            Task.CompletedTask;
    }
}
