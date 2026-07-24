using Compendium.Domain.Origins;
using Compendium.Domain.SharedKernel;

namespace Compendium.UnitTests.Origins;

public sealed class OriginTests
{
    private static readonly DateTimeOffset Now = new(2026, 7, 24, 18, 0, 0, TimeSpan.Zero);

    [Fact]
    public void Species_rejects_duplicate_feature_links()
    {
        var species = Species.Create(
            CompendiumEntityId.New(),
            CompendiumEntityId.New(),
            SpeciesCode.Create("ELF").Value,
            SpeciesName.Create("Elf").Value,
            null,
            Now).Value;
        var featureId = CompendiumEntityId.New();

        var first = species.LinkFeature(featureId, CompendiumEntityId.New(), Now);
        var duplicate = species.LinkFeature(featureId, CompendiumEntityId.New(), Now);

        Assert.True(first.IsSuccess);
        Assert.True(duplicate.IsFailure);
        Assert.Equal("compendium.origins.feature.duplicate", duplicate.Error.Code);
    }

    [Theory]
    [InlineData(1, 3)]
    [InlineData(2, 1)]
    public void Background_accepts_srd_ability_boost_patterns(int firstAmount, int firstCount)
    {
        var background = CreateBackground();
        var rules = firstAmount == 1 && firstCount == 3
            ? new[] { new BackgroundAbilityBoostRuleInput(1, 3) }
            : new[] { new BackgroundAbilityBoostRuleInput(2, 1), new BackgroundAbilityBoostRuleInput(1, 1) };

        var result = background.ConfigureMechanics(CreateMechanics(rules), CompendiumEntityId.New(), Now);

        Assert.True(result.IsSuccess);
        Assert.Equal(3, background.AbilityOptions.Count);
        Assert.Single(background.FeatGrants);
        Assert.Equal(2, background.SkillProficiencies.Count);
    }

    [Fact]
    public void Background_rejects_non_srd_ability_boost_pattern()
    {
        var background = CreateBackground();

        var result = background.ConfigureMechanics(
            CreateMechanics([new BackgroundAbilityBoostRuleInput(3, 1)]),
            CompendiumEntityId.New(),
            Now);

        Assert.True(result.IsFailure);
        Assert.Equal("compendium.origins.background.ability-boost-rules.invalid", result.Error.Code);
    }

    [Fact]
    public void Feat_rejects_unknown_category()
    {
        var result = Feat.Create(
            CompendiumEntityId.New(),
            CompendiumEntityId.New(),
            FeatCode.Create("ALERT").Value,
            FeatName.Create("Alert").Value,
            null,
            (FeatCategory)999,
            false,
            Now);

        Assert.True(result.IsFailure);
        Assert.Equal("compendium.origins.feat-category.invalid", result.Error.Code);
    }

    private static Background CreateBackground() =>
        Background.Create(
            CompendiumEntityId.New(),
            CompendiumEntityId.New(),
            BackgroundCode.Create("SOLDIER").Value,
            BackgroundName.Create("Soldier").Value,
            null,
            Now).Value;

    private static BackgroundMechanicsInput CreateMechanics(IReadOnlyCollection<BackgroundAbilityBoostRuleInput> rules) =>
        new(
            [CompendiumEntityId.New(), CompendiumEntityId.New(), CompendiumEntityId.New()],
            rules,
            [CompendiumEntityId.New()],
            [CompendiumEntityId.New(), CompendiumEntityId.New()],
            [CompendiumEntityId.New()],
            [new BackgroundStartingEquipmentRuleInput(CompendiumEntityId.New(), StartingEquipmentReferenceType.Rule)]);
}
