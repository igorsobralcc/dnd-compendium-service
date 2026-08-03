using System.Diagnostics;
using Compendium.Application.Observability;
using Compendium.Infra.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Compendium.Infra.Integration;

internal sealed class OutboxCleanupService(
    IServiceScopeFactory scopeFactory,
    IOptions<IntegrationMessagingOptions> options,
    TimeProvider timeProvider,
    ILogger<OutboxCleanupService> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (!options.Value.CleanupEnabled) return;

        while (!stoppingToken.IsCancellationRequested)
        {
            await RunCleanupAsync(stoppingToken);

            try
            {
                await Task.Delay(options.Value.CleanupInterval, timeProvider, stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
        }
    }

    internal async Task<int> RunCleanupAsync(CancellationToken cancellationToken)
    {
        if (!options.Value.CleanupEnabled) return 0;

        var stopwatch = Stopwatch.StartNew();
        var cutoff = timeProvider.GetUtcNow().Subtract(options.Value.PublishedRetention);
        var totalDeleted = 0;
        var possiblyMoreRows = false;

        try
        {
            for (var batch = 0; batch < options.Value.CleanupMaxBatchesPerRun; batch++)
            {
                using var scope = scopeFactory.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<CompendiumDbContext>();
                var deleted = await db.Database.ExecuteSqlInterpolatedAsync($$"""
                    WITH candidates AS (
                        SELECT id
                        FROM compendium.integration_outbox
                        WHERE status = 'PUBLISHED'
                          AND published_at_utc < {{cutoff}}
                        ORDER BY published_at_utc
                        FOR UPDATE SKIP LOCKED
                        LIMIT {{options.Value.CleanupBatchSize}}
                    )
                    DELETE FROM compendium.integration_outbox AS outbox
                    USING candidates
                    WHERE outbox.id = candidates.id;
                    """, cancellationToken);

                totalDeleted += deleted;
                if (deleted < options.Value.CleanupBatchSize)
                {
                    possiblyMoreRows = false;
                    break;
                }

                possiblyMoreRows = batch + 1 == options.Value.CleanupMaxBatchesPerRun;
                if (options.Value.CleanupInterBatchDelay > TimeSpan.Zero)
                {
                    await Task.Delay(options.Value.CleanupInterBatchDelay, timeProvider, cancellationToken);
                }
            }

            CompendiumTelemetry.OutboxCleanupDeleted.Add(totalDeleted);
            logger.LogInformation(
                "Cleaned published Outbox messages before {Cutoff}; deleted {DeletedCount} in batches of {BatchSize} over {ElapsedMilliseconds} ms; more eligible rows may remain: {PossiblyMoreRows}.",
                cutoff,
                totalDeleted,
                options.Value.CleanupBatchSize,
                stopwatch.Elapsed.TotalMilliseconds,
                possiblyMoreRows);
            return totalDeleted;
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch (Exception exception)
        {
            CompendiumTelemetry.OutboxCleanupFailures.Add(1);
            logger.LogError(
                exception,
                "Failed to clean published Outbox messages before {Cutoff} after deleting {DeletedCount} rows.",
                cutoff,
                totalDeleted);
            return totalDeleted;
        }
    }
}
