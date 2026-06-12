using Compendium.Domain.SharedKernel;

namespace Compendium.UnitTests.SharedKernel;

public sealed class CompendiumEntityIdTests
{
    [Fact]
    public void Create_returns_success_for_non_empty_guid()
    {
        var value = Guid.NewGuid();

        var result = CompendiumEntityId.Create(value);

        Assert.True(result.IsSuccess);
        Assert.Equal(value, result.Value.Value);
    }

    [Fact]
    public void Create_returns_validation_error_for_empty_guid()
    {
        var result = CompendiumEntityId.Create(Guid.Empty);

        Assert.True(result.IsFailure);
        Assert.Equal("compendium.entity-id.invalid", result.Error.Code);
        Assert.Equal(DomainErrorKind.Validation, result.Error.Kind);
    }

    [Fact]
    public void Parse_returns_validation_error_for_invalid_text()
    {
        var result = CompendiumEntityId.Parse("not-a-guid");

        Assert.True(result.IsFailure);
        Assert.Equal("compendium.entity-id.invalid", result.Error.Code);
    }
}
