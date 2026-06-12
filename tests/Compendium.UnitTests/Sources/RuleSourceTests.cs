using Compendium.Domain.SharedKernel;
using Compendium.Domain.Sources;

namespace Compendium.UnitTests.Sources;

public sealed class RuleSourceTests
{
    [Fact]
    public void Create_accepts_active_srd_source()
    {
        var source = RuleSource.Create(
            CompendiumEntityId.New(),
            SourceCode.Create("SRD").Value,
            SourceName.Create("System Reference Document").Value,
            SourceType.Srd,
            SourceStatus.Active,
            DateTimeOffset.UtcNow);

        Assert.True(source.IsSuccess);
        Assert.Equal(SourceType.Srd, source.Value.Type);
        Assert.Equal(SourceStatus.Active, source.Value.Status);
    }

    [Fact]
    public void Deactivate_marks_source_inactive()
    {
        var source = RuleSource.Create(
            CompendiumEntityId.New(),
            SourceCode.Create("SRD").Value,
            SourceName.Create("System Reference Document").Value,
            SourceType.Srd,
            SourceStatus.Active,
            DateTimeOffset.UtcNow).Value;

        source.Deactivate(DateTimeOffset.UtcNow);

        Assert.Equal(SourceStatus.Inactive, source.Status);
    }
}
