using Compendium.Application.Contracts.Events;
using Compendium.Infra.Persistence.Integration;

namespace Compendium.IntegrationTests.Integration;

public sealed class IntegrationMessagingTests
{
    [Fact]
    public void Outbox_message_is_versioned_and_keeps_routing_metadata()
    {
        var now = DateTimeOffset.UtcNow;
        var message = CreateOutbox(now);

        Assert.Equal(1, message.EventVersion);
        Assert.Equal("translation", message.AggregateType);
        Assert.Equal(IntegrationMessageStatus.Pending, message.Status);
        Assert.False(string.IsNullOrWhiteSpace(message.CorrelationId));
    }

    [Fact]
    public void Broker_failure_schedules_retry_without_losing_message()
    {
        var now = DateTimeOffset.UtcNow;
        var message = CreateOutbox(now);

        message.MarkFailed("broker unavailable", now, 3, TimeSpan.FromSeconds(10));

        Assert.Equal(IntegrationMessageStatus.Failed, message.Status);
        Assert.Equal(1, message.RetryCount);
        Assert.Equal(now.AddSeconds(10), message.AvailableAtUtc);
        Assert.Null(message.PublishedAtUtc);
    }

    [Fact]
    public void Repeated_broker_failure_moves_message_to_dead_letter()
    {
        var now = DateTimeOffset.UtcNow;
        var message = CreateOutbox(now);

        message.MarkFailed("first", now, 2, TimeSpan.Zero);
        message.MarkFailed("second", now, 2, TimeSpan.Zero);

        Assert.Equal(IntegrationMessageStatus.DeadLetter, message.Status);
        Assert.Equal(2, message.RetryCount);
    }

    [Fact]
    public void Successful_publish_marks_message_as_published()
    {
        var now = DateTimeOffset.UtcNow;
        var message = CreateOutbox(now);

        message.MarkPublished(now.AddSeconds(1));

        Assert.Equal(IntegrationMessageStatus.Published, message.Status);
        Assert.Equal(now.AddSeconds(1), message.PublishedAtUtc);
        Assert.Null(message.LastError);
    }

    [Fact]
    public void Inbox_tracks_failure_retry_and_success()
    {
        var now = DateTimeOffset.UtcNow;
        var inbox = new IntegrationInbox(
            Guid.NewGuid().ToString(),
            "test-consumer",
            CompendiumEventNames.SourceVersionImportedV1,
            1,
            "correlation-456",
            now);

        inbox.MarkProcessing(now);
        inbox.MarkFailed("temporary failure", now, 3);
        Assert.True(inbox.CanProcess);
        Assert.Equal(1, inbox.RetryCount);

        inbox.MarkProcessing(now.AddSeconds(1));
        inbox.MarkProcessed(now.AddSeconds(2));

        Assert.Equal(IntegrationMessageStatus.Processed, inbox.Status);
        Assert.False(inbox.CanProcess);
    }

    private static IntegrationOutbox CreateOutbox(DateTimeOffset now) =>
        new(
            CompendiumEventNames.TranslationUpdatedV1,
            1,
            "translation",
            Guid.NewGuid().ToString(),
            Guid.NewGuid().ToString(),
            now);
}
