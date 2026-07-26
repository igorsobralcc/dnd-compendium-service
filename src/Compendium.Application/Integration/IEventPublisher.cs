using Compendium.Application.Contracts.Events;

namespace Compendium.Application.Integration;

/// <summary>Persists an event alongside the aggregate transaction.</summary>
public interface IEventPublisher
{
    Task EnqueueAsync(
        string eventName,
        int eventVersion,
        string aggregateType,
        string aggregateId,
        string correlationId,
        DateTimeOffset occurredAtUtc,
        IReadOnlyCollection<IntegrationEventField> fields,
        CancellationToken cancellationToken);
}

/// <summary>Delivers an already committed event to the configured broker.</summary>
public interface IEventTransport
{
    Task PublishAsync(IntegrationEventEnvelope message, CancellationToken cancellationToken);
}

public interface IMessageConsumer
{
    Task<InboxProcessingResult> ConsumeAsync(
        IntegrationEventEnvelope message,
        string consumerName,
        Func<IntegrationEventEnvelope, CancellationToken, Task> handler,
        CancellationToken cancellationToken);
}

public enum InboxProcessingResult
{
    Processed,
    AlreadyProcessed,
    Failed,
    DeadLettered
}
