namespace Compendium.Infra.Persistence.Integration;

public sealed class IntegrationOutbox
{
    private IntegrationOutbox()
    {
    }

    public IntegrationOutbox(
        string eventName,
        int eventVersion,
        string aggregateType,
        string aggregateId,
        string correlationId,
        DateTimeOffset occurredAtUtc)
    {
        Id = Guid.CreateVersion7();
        EventId = Guid.CreateVersion7();
        EventName = eventName;
        EventVersion = eventVersion;
        AggregateType = aggregateType;
        AggregateId = aggregateId;
        CorrelationId = correlationId;
        OccurredAtUtc = occurredAtUtc;
        AvailableAtUtc = occurredAtUtc;
        CreatedAtUtc = occurredAtUtc;
        UpdatedAtUtc = occurredAtUtc;
        Status = IntegrationMessageStatus.Pending;
    }

    public Guid Id { get; private set; }

    public Guid EventId { get; private set; }

    public string EventName { get; private set; } = string.Empty;

    public int EventVersion { get; private set; }

    public string AggregateType { get; private set; } = string.Empty;

    public string AggregateId { get; private set; } = string.Empty;

    public string CorrelationId { get; private set; } = string.Empty;

    public DateTimeOffset OccurredAtUtc { get; private set; }

    public DateTimeOffset AvailableAtUtc { get; private set; }

    public DateTimeOffset? PublishedAtUtc { get; private set; }

    public string Status { get; private set; } = IntegrationMessageStatus.Pending;

    public int RetryCount { get; private set; }

    public string? LastError { get; private set; }

    public Guid? ClaimToken { get; private set; }

    public string? ProcessingOwner { get; private set; }

    public DateTimeOffset? ProcessingStartedAtUtc { get; private set; }

    public DateTimeOffset? LeaseExpiresAtUtc { get; private set; }

    public DateTimeOffset CreatedAtUtc { get; private set; }

    public DateTimeOffset UpdatedAtUtc { get; private set; }

    public List<IntegrationOutboxField> Fields { get; private set; } = [];

    internal void MarkClaimed(Guid claimToken, string processingOwner, DateTimeOffset now, DateTimeOffset leaseExpiresAtUtc)
    {
        Status = IntegrationMessageStatus.Processing;
        ClaimToken = claimToken;
        ProcessingOwner = processingOwner;
        ProcessingStartedAtUtc = now;
        LeaseExpiresAtUtc = leaseExpiresAtUtc;
        UpdatedAtUtc = now;
    }

    public void MarkPublished(DateTimeOffset now)
    {
        Status = IntegrationMessageStatus.Published;
        PublishedAtUtc = now;
        LastError = null;
        ClearClaim();
        UpdatedAtUtc = now;
    }

    public void MarkFailed(string error, DateTimeOffset now, int maxRetries, TimeSpan retryDelay)
    {
        RetryCount++;
        LastError = error.Length <= 2000 ? error : error[..2000];
        Status = RetryCount >= maxRetries
            ? IntegrationMessageStatus.DeadLetter
            : IntegrationMessageStatus.Failed;
        AvailableAtUtc = now.Add(retryDelay);
        ClearClaim();
        UpdatedAtUtc = now;
    }

    private void ClearClaim()
    {
        ClaimToken = null;
        ProcessingOwner = null;
        ProcessingStartedAtUtc = null;
        LeaseExpiresAtUtc = null;
    }
}
