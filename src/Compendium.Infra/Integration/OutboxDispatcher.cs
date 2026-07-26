using Compendium.Application.Contracts.Events;
using Compendium.Application.Integration;
using Compendium.Infra.Persistence;
using Compendium.Infra.Persistence.Integration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Compendium.Infra.Integration;

internal sealed class OutboxDispatcher(
    IServiceScopeFactory scopeFactory,
    IOptions<IntegrationMessagingOptions> options,
    ILogger<OutboxDispatcher> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await DispatchBatchAsync(stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception exception)
            {
                logger.LogError(exception, "Unexpected failure while polling the integration outbox.");
            }

            await Task.Delay(options.Value.PollingInterval, stoppingToken);
        }
    }

    internal async Task DispatchBatchAsync(CancellationToken cancellationToken)
    {
        using var scope = scopeFactory.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<CompendiumDbContext>();
        var transport = scope.ServiceProvider.GetRequiredService<IEventTransport>();
        var now = DateTimeOffset.UtcNow;
        var messages = await db.IntegrationOutbox
            .Include(x => x.Fields)
            .Where(x => (x.Status == IntegrationMessageStatus.Pending || x.Status == IntegrationMessageStatus.Failed)
                && x.AvailableAtUtc <= now)
            .OrderBy(x => x.CreatedAtUtc)
            .Take(options.Value.BatchSize)
            .ToArrayAsync(cancellationToken);

        foreach (var message in messages)
        {
            using var logScope = logger.BeginScope(new Dictionary<string, object>
            {
                ["CorrelationId"] = message.CorrelationId,
                ["EventId"] = message.EventId
            });

            try
            {
                await transport.PublishAsync(ToEnvelope(message), cancellationToken);
                message.MarkPublished(DateTimeOffset.UtcNow);
                await db.SaveChangesAsync(cancellationToken);
                logger.LogInformation("Published integration event {EventName} v{EventVersion}.", message.EventName, message.EventVersion);
            }
            catch (Exception exception) when (exception is not OperationCanceledException)
            {
                message.MarkFailed(
                    exception.Message,
                    DateTimeOffset.UtcNow,
                    options.Value.MaxRetries,
                    options.Value.RetryDelay);
                await db.SaveChangesAsync(cancellationToken);
                logger.LogError(exception, "Failed to publish integration event {EventName}; status is {Status}.", message.EventName, message.Status);
            }
        }
    }

    private static IntegrationEventEnvelope ToEnvelope(IntegrationOutbox message) =>
        new(
            message.EventId,
            message.EventName,
            message.EventVersion,
            message.AggregateType,
            message.AggregateId,
            message.CorrelationId,
            message.OccurredAtUtc,
            message.Fields.Select(field => new IntegrationEventField(
                field.FieldName,
                field.FieldType,
                field.TextValue,
                field.NumberValue,
                field.BooleanValue,
                field.ReferenceValue,
                field.EnumValue)).ToArray());
}
