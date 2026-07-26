using Compendium.Application.Contracts.Events;
using Compendium.Application.Integration;
using Microsoft.Extensions.Logging;

namespace Compendium.Infra.Integration;

/// <summary>
/// Default local transport. Production composition can replace it with a broker adapter.
/// </summary>
internal sealed class LoggingEventTransport(ILogger<LoggingEventTransport> logger) : IEventTransport
{
    public Task PublishAsync(IntegrationEventEnvelope message, CancellationToken cancellationToken)
    {
        logger.LogInformation(
            "Integration event delivered: {EventName} v{EventVersion}, aggregate {AggregateType}/{AggregateId}, correlation {CorrelationId}.",
            message.Name,
            message.Version,
            message.AggregateType,
            message.AggregateId,
            message.CorrelationId);
        return Task.CompletedTask;
    }
}
