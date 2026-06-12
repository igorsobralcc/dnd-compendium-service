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
}
