using Compendium.Domain.SharedKernel;
using Compendium.Domain.Sources;

namespace Compendium.UnitTests.Sources;

public sealed class SourceVersionTests
{
    [Fact]
    public void Create_records_source_version_created_event()
    {
        var sourceId = CompendiumEntityId.New();

        var version = SourceVersion.Create(
            sourceId,
            SourceVersionNumber.Create("5.2.1").Value,
            PublicationDate.Create(new DateOnly(2025, 4, 22), new DateOnly(2026, 6, 12)).Value,
            ImportStatus.Imported,
            true,
            DateTimeOffset.UtcNow);

        Assert.True(version.IsSuccess);
        var domainEvent = Assert.Single(version.Value.DomainEvents);
        Assert.Equal(sourceId.Value, domainEvent.RuleSourceId);
        Assert.Equal("5.2.1", domainEvent.VersionNumber);
    }

    [Fact]
    public void Publication_date_rejects_future_values()
    {
        var result = PublicationDate.Create(new DateOnly(2026, 6, 13), new DateOnly(2026, 6, 12));

        Assert.True(result.IsFailure);
        Assert.Equal("compendium.sources.publication-date.invalid", result.Error.Code);
    }

    [Fact]
    public void Mark_as_current_changes_current_flag()
    {
        var version = SourceVersion.Create(
            CompendiumEntityId.New(),
            SourceVersionNumber.Create("5.2.1").Value,
            PublicationDate.Create(new DateOnly(2025, 4, 22), new DateOnly(2026, 6, 12)).Value,
            ImportStatus.Imported,
            false,
            DateTimeOffset.UtcNow).Value;

        version.MarkAsCurrent(DateTimeOffset.UtcNow);

        Assert.True(version.IsCurrent);
    }
}
