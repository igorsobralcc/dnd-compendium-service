using Compendium.Domain.Classes;
using Compendium.Domain.SharedKernel;

namespace Compendium.UnitTests.Classes;

public sealed class CharacterClassTests
{
    [Fact]
    public void Class_code_normalizes_to_uppercase()
    {
        var code = ClassCode.Create("fighter");

        Assert.True(code.IsSuccess);
        Assert.Equal("FIGHTER", code.Value.Value);
    }

    [Fact]
    public void Character_class_requires_unique_levels()
    {
        var characterClass = CreateClass(
            [
                CreateLevelInput(1),
                CreateLevelInput(1)
            ]);

        Assert.True(characterClass.IsFailure);
        Assert.Equal("compendium.classes.level.duplicate", characterClass.Error.Code);
    }

    [Fact]
    public void Character_class_rejects_unsupported_level()
    {
        var characterClass = CreateClass([CreateLevelInput(21)]);

        Assert.True(characterClass.IsFailure);
        Assert.Equal("compendium.classes.level.invalid", characterClass.Error.Code);
    }

    [Fact]
    public void Character_class_rejects_invalid_spell_slot_level()
    {
        var characterClass = CreateClass(
            [
                CreateLevelInput(
                    1,
                    [new ClassLevelSpellSlotInput(10, 2)])
            ]);

        Assert.True(characterClass.IsFailure);
        Assert.Equal("compendium.classes.spell-slot-level.invalid", characterClass.Error.Code);
    }

    [Fact]
    public void Character_class_accepts_spellcasting_progression()
    {
        var characterClass = CreateClass([CreateLevelInput(1)]);
        var spellcasting = new ClassSpellcastingProgressionInput(
            ClassSpellcastingProgressionType.FullCaster,
            CompendiumEntityId.New(),
            [new ClassSpellcastingLevelRuleInput(1, 1)]);

        var result = characterClass.Value.ConfigureProgression(
            [CreateLevelInput(1, [new ClassLevelSpellSlotInput(1, 2)])],
            spellcasting,
            CompendiumEntityId.New(),
            DateTimeOffset.UtcNow);

        Assert.True(result.IsSuccess);
        Assert.NotNull(characterClass.Value.SpellcastingProgression);
        Assert.Single(characterClass.Value.Levels.Single().SpellSlots);
    }

    [Fact]
    public void Subclass_rejects_duplicate_feature_at_same_level()
    {
        var subclass = CharacterSubclass.Create(
            CompendiumEntityId.New(),
            CompendiumEntityId.New(),
            CompendiumEntityId.New(),
            ClassCode.Create("CHAMPION").Value,
            ClassName.Create("Champion").Value,
            null,
            DateTimeOffset.UtcNow).Value;
        var featureId = CompendiumEntityId.New();

        var firstLink = subclass.LinkFeature(featureId, CompendiumEntityId.New(), 3, DateTimeOffset.UtcNow);
        var secondLink = subclass.LinkFeature(featureId, CompendiumEntityId.New(), 3, DateTimeOffset.UtcNow);

        Assert.True(firstLink.IsSuccess);
        Assert.True(secondLink.IsFailure);
        Assert.Equal("compendium.classes.subclass-feature.duplicate", secondLink.Error.Code);
    }

    private static Result<CharacterClass> CreateClass(IReadOnlyCollection<ClassLevelInput> levels)
    {
        return CharacterClass.Create(
            CompendiumEntityId.New(),
            CompendiumEntityId.New(),
            ClassCode.Create("FIGHTER").Value,
            ClassName.Create("Fighter").Value,
            ClassDescription.CreateOptional("Martial defender").Value,
            new ClassCoreTraitsInput(CompendiumEntityId.New(), null, 2),
            [CompendiumEntityId.New()],
            levels,
            DateTimeOffset.UtcNow);
    }

    private static ClassLevelInput CreateLevelInput(
        int level,
        IReadOnlyCollection<ClassLevelSpellSlotInput>? spellSlots = null) =>
        new(level, 2, spellSlots ?? [], [], null);
}
