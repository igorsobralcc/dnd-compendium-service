using Compendium.Domain.Sources;

namespace Compendium.UnitTests.Sources;

public sealed class RulesetTests
{
    [Fact]
    public void Create_accepts_srd_5_2_1_ruleset()
    {
        var code = RulesetCode.Create("srd-5.2.1");
        var name = RulesetName.Create("SRD 5.2.1");
        var version = RulesetVersion.Create("5.2.1");

        var ruleset = Ruleset.Create(
            code.Value,
            name.Value,
            version.Value,
            RulesetStatus.Active,
            DateTimeOffset.UtcNow);

        Assert.True(ruleset.IsSuccess);
        Assert.Equal("SRD-5.2.1", ruleset.Value.Code.Value);
        Assert.Equal(RulesetStatus.Active, ruleset.Value.Status);
    }

    [Fact]
    public void Code_rejects_blank_values()
    {
        var result = RulesetCode.Create(" ");

        Assert.True(result.IsFailure);
        Assert.Equal("compendium.sources.ruleset-code.required", result.Error.Code);
    }

    [Fact]
    public void Update_rejects_invalid_status()
    {
        var ruleset = Ruleset.Create(
            RulesetCode.Create("SRD").Value,
            RulesetName.Create("System Reference Document").Value,
            RulesetVersion.Create("5.2.1").Value,
            RulesetStatus.Active,
            DateTimeOffset.UtcNow).Value;

        var result = ruleset.Update(
            RulesetName.Create("System Reference Document").Value,
            RulesetVersion.Create("5.2.1").Value,
            (RulesetStatus)999,
            DateTimeOffset.UtcNow);

        Assert.True(result.IsFailure);
        Assert.Equal("compendium.sources.ruleset-status.invalid", result.Error.Code);
    }
}
