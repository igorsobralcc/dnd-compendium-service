using Compendium.Application.Contracts.Events;
using Compendium.Application.Integration;
using Compendium.Infra.Persistence;
using Compendium.Infra.Persistence.Integration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Compendium.Application.Observability;

namespace Compendium.Infra.Integration;

internal sealed class OutboxDispatcher(
    IServiceScopeFactory scopeFactory,
    IOptions<IntegrationMessagingOptions> options,
    TimeProvider timeProvider,
    OutboxWorkerIdentity workerIdentity,
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
        var now = timeProvider.GetUtcNow();
        var claimToken = Guid.CreateVersion7();
        var claimedIds = await ClaimBatchAsync(db, claimToken, now, cancellationToken);
        if (claimedIds.Length == 0) return;

        var messages = await db.IntegrationOutbox
            .Include(x => x.Fields)
            .Where(x => claimedIds.Contains(x.Id)
                && x.Status == IntegrationMessageStatus.Processing
                && x.ClaimToken == claimToken)
            .OrderBy(x => x.CreatedAtUtc)
            .ToArrayAsync(cancellationToken);

        var unfinishedIds = messages.Select(message => message.Id).ToHashSet();
        var leaseExpiresAtUtc = now.Add(options.Value.ProcessingLeaseDuration);
        foreach (var message in messages)
        {
            if (leaseExpiresAtUtc - timeProvider.GetUtcNow() < options.Value.ProcessingLeaseDuration / 2)
            {
                var renewed = await RenewLeaseAsync(
                    db,
                    unfinishedIds,
                    claimToken,
                    timeProvider.GetUtcNow(),
                    cancellationToken);
                if (!renewed) return;
                leaseExpiresAtUtc = timeProvider.GetUtcNow().Add(options.Value.ProcessingLeaseDuration);
            }

            using var logScope = logger.BeginScope(new Dictionary<string, object>
            {
                ["CorrelationId"] = message.CorrelationId,
                ["EventId"] = message.EventId,
                ["ClaimToken"] = claimToken,
                ["ProcessingOwner"] = workerIdentity.Value
            });

            var publishFailure = await TryPublishAsync(transport, message, cancellationToken);
            if (publishFailure is null)
            {
                message.MarkPublished(timeProvider.GetUtcNow());
                if (!await TrySaveCompletionAsync(db, message, claimToken, cancellationToken)) return;
                CompendiumTelemetry.OutboxPublished.Add(1,
                    new KeyValuePair<string, object?>("event.name", message.EventName));
                logger.LogInformation("Published integration event {EventName} v{EventVersion}.", message.EventName, message.EventVersion);
            }
            else
            {
                message.MarkFailed(
                    publishFailure.Message,
                    timeProvider.GetUtcNow(),
                    options.Value.MaxRetries,
                    options.Value.RetryDelay);
                if (!await TrySaveCompletionAsync(db, message, claimToken, cancellationToken)) return;
                CompendiumTelemetry.OutboxPublicationFailures.Add(1,
                    new KeyValuePair<string, object?>("event.name", message.EventName));
                logger.LogError(publishFailure, "Failed to publish integration event {EventName}; status is {Status}.", message.EventName, message.Status);
            }

            unfinishedIds.Remove(message.Id);
        }
    }

    private async Task<Guid[]> ClaimBatchAsync(
        CompendiumDbContext db,
        Guid claimToken,
        DateTimeOffset now,
        CancellationToken cancellationToken)
    {
        var leaseExpiry = now.Add(options.Value.ProcessingLeaseDuration);
        var claimedIds = await db.Database.SqlQuery<Guid>($$"""
            WITH candidates AS (
                SELECT id
                FROM compendium.integration_outbox
                WHERE (status IN ('PENDING', 'FAILED') AND available_at_utc <= {{now}})
                   OR (status = 'PROCESSING' AND lease_expires_at_utc <= {{now}})
                ORDER BY created_at_utc
                FOR UPDATE SKIP LOCKED
                LIMIT {{options.Value.BatchSize}}
            )
            UPDATE compendium.integration_outbox AS outbox
            SET status = 'PROCESSING',
                claim_token = {{claimToken}},
                processing_owner = {{workerIdentity.Value}},
                processing_started_at_utc = {{now}},
                lease_expires_at_utc = {{leaseExpiry}},
                updated_at_utc = {{now}}
            FROM candidates
            WHERE outbox.id = candidates.id
            RETURNING outbox.id AS "Value"
            """).ToArrayAsync(cancellationToken);

        logger.LogDebug(
            "Claimed {ClaimedCount} Outbox messages with claim {ClaimToken} for worker {ProcessingOwner}.",
            claimedIds.Length,
            claimToken,
            workerIdentity.Value);
        return claimedIds;
    }

    private async Task<bool> RenewLeaseAsync(
        CompendiumDbContext db,
        IReadOnlySet<Guid> unfinishedIds,
        Guid claimToken,
        DateTimeOffset now,
        CancellationToken cancellationToken)
    {
        if (unfinishedIds.Count == 0) return true;

        var leaseExpiry = now.Add(options.Value.ProcessingLeaseDuration);
        var affected = await db.IntegrationOutbox
            .Where(message => unfinishedIds.Contains(message.Id)
                && message.Status == IntegrationMessageStatus.Processing
                && message.ClaimToken == claimToken)
            .ExecuteUpdateAsync(
                setters => setters
                    .SetProperty(message => message.LeaseExpiresAtUtc, leaseExpiry)
                    .SetProperty(message => message.UpdatedAtUtc, now),
                cancellationToken);

        if (affected == unfinishedIds.Count) return true;

        logger.LogWarning(
            "Stopped Outbox claim {ClaimToken} for worker {ProcessingOwner}: renewed {RenewedCount} of {ExpectedCount} unfinished leases.",
            claimToken,
            workerIdentity.Value,
            affected,
            unfinishedIds.Count);
        return false;
    }

    private async Task<Exception?> TryPublishAsync(
        IEventTransport transport,
        IntegrationOutbox message,
        CancellationToken cancellationToken)
    {
        using var timeout = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        timeout.CancelAfter(options.Value.PublishAttemptTimeout);

        try
        {
            await transport.PublishAsync(ToEnvelope(message), timeout.Token);
            return null;
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch (OperationCanceledException exception)
        {
            return new TimeoutException(
                $"Publishing exceeded {options.Value.PublishAttemptTimeout}.",
                exception);
        }
        catch (Exception exception)
        {
            return exception;
        }
    }

    private async Task<bool> TrySaveCompletionAsync(
        CompendiumDbContext db,
        IntegrationOutbox message,
        Guid claimToken,
        CancellationToken cancellationToken)
    {
        try
        {
            await db.SaveChangesAsync(cancellationToken);
            return true;
        }
        catch (DbUpdateConcurrencyException exception)
        {
            logger.LogWarning(
                exception,
                "Ignored stale completion for Outbox event {EventId}, claim {ClaimToken}, worker {ProcessingOwner}.",
                message.EventId,
                claimToken,
                workerIdentity.Value);
            return false;
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
