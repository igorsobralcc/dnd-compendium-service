namespace Compendium.Infra.Persistence.Integration;

public sealed class IntegrationInbox
{
    private IntegrationInbox()
    {
    }

    public IntegrationInbox(
        string eventId,
        string consumerName,
        string eventName,
        int eventVersion,
        string correlationId,
        DateTimeOffset receivedAtUtc)
    {
        Id = Guid.CreateVersion7();
        EventId = eventId;
        ConsumerName = consumerName;
        EventName = eventName;
        EventVersion = eventVersion;
        CorrelationId = correlationId;
        ReceivedAtUtc = receivedAtUtc;
        CreatedAtUtc = receivedAtUtc;
        UpdatedAtUtc = receivedAtUtc;
        Status = IntegrationMessageStatus.Received;
    }

    public Guid Id { get; private set; }

    public string EventId { get; private set; } = string.Empty;

    public string ConsumerName { get; private set; } = string.Empty;

    public string EventName { get; private set; } = string.Empty;

    public int EventVersion { get; private set; }

    public string CorrelationId { get; private set; } = string.Empty;

    public DateTimeOffset ReceivedAtUtc { get; private set; }

    public DateTimeOffset? ProcessedAtUtc { get; private set; }

    public string Status { get; private set; } = IntegrationMessageStatus.Received;

    public int RetryCount { get; private set; }

    public string? LastError { get; private set; }

    public DateTimeOffset CreatedAtUtc { get; private set; }

    public DateTimeOffset UpdatedAtUtc { get; private set; }
}
