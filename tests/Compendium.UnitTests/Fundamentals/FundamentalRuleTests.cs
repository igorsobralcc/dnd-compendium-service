using Compendium.Domain.Fundamentals;
using Compendium.Domain.SharedKernel;

namespace Compendium.UnitTests.Fundamentals;

public sealed class FundamentalRuleTests
{
    [Fact]
    public void Ability_code_normalizes_to_uppercase()
    {
        var code = AbilityCode.Create("str");

        Assert.True(code.IsSuccess);
        Assert.Equal("STR", code.Value.Value);
    }

    [Fact]
    public void Skill_can_reference_default_ability()
    {
        var abilityId = CompendiumEntityId.New();

        var skill = Skill.Create(
            CompendiumEntityId.New(),
            CompendiumEntityId.New(),
            SkillCode.Create("ATHLETICS").Value,
            DisplayName.Create("Athletics").Value,
            abilityId,
            DateTimeOffset.UtcNow);

        Assert.True(skill.IsSuccess);
        Assert.Equal(abilityId, skill.Value.DefaultAbilityId);
    }

    [Fact]
    public void Proficiency_rejects_invalid_type()
    {
        var proficiency = Proficiency.Create(
            CompendiumEntityId.New(),
            CompendiumEntityId.New(),
            ProficiencyCode.Create("SKILL_ATHLETICS").Value,
            DisplayName.Create("Athletics").Value,
            (ProficiencyType)999,
            null,
            DateTimeOffset.UtcNow);

        Assert.True(proficiency.IsFailure);
        Assert.Equal("compendium.fundamentals.proficiency-type.invalid", proficiency.Error.Code);
    }

    [Fact]
    public void Hit_die_rejects_unsupported_die()
    {
        var hitDie = HitDie.Create(
            CompendiumEntityId.New(),
            CompendiumEntityId.New(),
            20,
            DateTimeOffset.UtcNow);

        Assert.True(hitDie.IsFailure);
        Assert.Equal("compendium.fundamentals.hit-die.invalid", hitDie.Error.Code);
    }

    [Fact]
    public void Standard_array_requires_six_ordered_values()
    {
        var method = AbilityScoreMethod.Create(
            CompendiumEntityId.New(),
            CompendiumEntityId.New(),
            AbilityScoreMethodCode.Create("STANDARD_ARRAY").Value,
            DisplayName.Create("Standard Array").Value,
            AbilityScoreMethodType.StandardArray,
            [],
            [15, 14, 13],
            [],
            null,
            DateTimeOffset.UtcNow);

        Assert.True(method.IsFailure);
        Assert.Equal("compendium.fundamentals.ability-score-method.standard-array-values.required", method.Error.Code);
    }

    [Fact]
    public void Point_buy_requires_costs_by_score()
    {
        var method = AbilityScoreMethod.Create(
            CompendiumEntityId.New(),
            CompendiumEntityId.New(),
            AbilityScoreMethodCode.Create("POINT_BUY").Value,
            DisplayName.Create("Point Buy").Value,
            AbilityScoreMethodType.PointBuy,
            [],
            [],
            [],
            null,
            DateTimeOffset.UtcNow);

        Assert.True(method.IsFailure);
        Assert.Equal("compendium.fundamentals.ability-score-method.point-buy-costs.required", method.Error.Code);
    }

    [Fact]
    public void Random_roll_requires_valid_drop_rule()
    {
        var method = AbilityScoreMethod.Create(
            CompendiumEntityId.New(),
            CompendiumEntityId.New(),
            AbilityScoreMethodCode.Create("RANDOM_GENERATION").Value,
            DisplayName.Create("Random Generation").Value,
            AbilityScoreMethodType.RandomRoll,
            [],
            [],
            [],
            new AbilityScoreRollRuleInput(4, 6, 3, 4, 6),
            DateTimeOffset.UtcNow);

        Assert.True(method.IsFailure);
        Assert.Equal("compendium.fundamentals.ability-score-method.roll-drop.invalid", method.Error.Code);
    }

    [Fact]
    public void Ability_score_method_accepts_srd_random_generation()
    {
        var method = AbilityScoreMethod.Create(
            CompendiumEntityId.New(),
            CompendiumEntityId.New(),
            AbilityScoreMethodCode.Create("RANDOM_GENERATION").Value,
            DisplayName.Create("Random Generation").Value,
            AbilityScoreMethodType.RandomRoll,
            [new AbilityScoreMethodRuleInput(AbilityScoreMethodRuleCode.Create("GENERATED_SCORE_COUNT").Value, 6, null)],
            [],
            [],
            new AbilityScoreRollRuleInput(4, 6, 3, 1, 6),
            DateTimeOffset.UtcNow);

        Assert.True(method.IsSuccess);
        Assert.Single(method.Value.RollRules);
    }
}
