using System.Diagnostics;
using Compendium.Application.Observability;
using Compendium.Infra.Persistence;
using Compendium.Infra.Persistence.Integration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Compendium.Infra.Integration;

internal sealed class OutboxBacklogCollector(
    IServiceScopeFactory scopeFactory,
    IOptions<IntegrationMessagingOptions> options,
    TimeProvider timeProvider,
    ILogger<OutboxBacklogCollector> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await CollectAsync(stoppingToken);

            try
            {
                await Task.Delay(options.Value.BacklogMetricsInterval, timeProvider, stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
        }
    }

    internal async Task CollectAsync(CancellationToken cancellationToken)
    {
        var stopwatch = Stopwatch.StartNew();

        try
        {
            using var scope = scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<CompendiumDbContext>();
            var unresolved = await db.IntegrationOutbox.LongCountAsync(
                message => message.Status == IntegrationMessageStatus.Pending
                    || message.Status == IntegrationMessageStatus.Failed
                    || message.Status == IntegrationMessageStatus.Processing,
                cancellationToken);

            CompendiumTelemetry.SetPendingOutboxMessages(unresolved);
            CompendiumTelemetry.OutboxBacklogCollectionDuration.Record(stopwatch.Elapsed.TotalMilliseconds);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch (Exception exception)
        {
            CompendiumTelemetry.OutboxBacklogCollectionFailures.Add(1);
            logger.LogError(exception, "Failed to collect the integration Outbox backlog.");
        }
    }
}
