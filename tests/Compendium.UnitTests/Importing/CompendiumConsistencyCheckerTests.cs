using Compendium.Domain.Importing;

namespace Compendium.UnitTests.Importing;

public sealed class CompendiumConsistencyCheckerTests
{
    [Fact]
    public void Check_ReturnsBlockers_WhenRequiredContentIsMissing()
    {
        var issues = new CompendiumConsistencyChecker().Check(new(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0));

        Assert.Contains(issues, x => x.Code == "MISSING_ABILITIES" && x.Severity == ValidationIssueSeverity.Blocker);
        Assert.Contains(issues, x => x.Code == "MISSING_CLASSES" && x.Severity == ValidationIssueSeverity.Blocker);
    }

    [Fact]
    public void Check_AllowsPublishing_WhenOnlyOptionalMvpCategoriesAreMissing()
    {
        var issues = new CompendiumConsistencyChecker().Check(new(6, 18, 10, 6, 4, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0));

        Assert.DoesNotContain(issues, x => x.Severity == ValidationIssueSeverity.Blocker);
        Assert.Contains(issues, x => x.Code == "MISSING_SPELLS" && x.Severity == ValidationIssueSeverity.Warning);
    }

    [Fact]
    public void Check_BlocksInvalidRelationalReferences()
    {
        var issues = new CompendiumConsistencyChecker().Check(new(6, 18, 10, 6, 4, 1, 1, 1, 0, 1, 0, 1, 1, 1, 1, 1));

        Assert.Equal(4, issues.Count(x => x.Severity == ValidationIssueSeverity.Blocker));
    }
}
