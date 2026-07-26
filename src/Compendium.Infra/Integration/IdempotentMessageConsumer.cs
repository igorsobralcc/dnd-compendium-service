using Compendium.Application.Contracts.Events;
using Compendium.Application.Integration;
using Compendium.Infra.Persistence;
using Compendium.Infra.Persistence.Integration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Compendium.Infra.Integration;

internal sealed class IdempotentMessageConsumer(
    CompendiumDbContext db,
    IOptions<IntegrationMessagingOptions> options,
    ILogger<IdempotentMessageConsumer> logger) : IMessageConsumer
{
    public async Task<InboxProcessingResult> ConsumeAsync(
        IntegrationEventEnvelope message,
        string consumerName,
        Func<IntegrationEventEnvelope, CancellationToken, Task> handler,
        CancellationToken cancellationToken)
    {
        var eventId = message.EventId.ToString();
        var inbox = await db.IntegrationInbox.SingleOrDefaultAsync(
            x => x.EventId == eventId && x.ConsumerName == consumerName,
            cancellationToken);

        if (inbox?.Status == IntegrationMessageStatus.DeadLetter)
            return InboxProcessingResult.DeadLettered;
        if (inbox?.Status == IntegrationMessageStatus.Processed)
            return InboxProcessingResult.AlreadyProcessed;
        if (inbox?.Status == IntegrationMessageStatus.Processing)
            return InboxProcessingResult.AlreadyProcessed;

        var now = DateTimeOffset.UtcNow;
        if (inbox is null)
        {
            inbox = new IntegrationInbox(
                eventId,
                consumerName,
                message.Name,
                message.Version,
                message.CorrelationId,
                now);
            db.IntegrationInbox.Add(inbox);
        }

        inbox.MarkProcessing(now);
        try
        {
            await db.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException)
        {
            db.Entry(inbox).State = EntityState.Detached;
            return InboxProcessingResult.AlreadyProcessed;
        }

        using var logScope = logger.BeginScope(new Dictionary<string, object>
        {
            ["CorrelationId"] = message.CorrelationId,
            ["EventId"] = message.EventId,
            ["ConsumerName"] = consumerName
        });

        try
        {
            await handler(message, cancellationToken);
            inbox.MarkProcessed(DateTimeOffset.UtcNow);
            await db.SaveChangesAsync(cancellationToken);
            return InboxProcessingResult.Processed;
        }
        catch (Exception exception) when (exception is not OperationCanceledException)
        {
            inbox.MarkFailed(exception.Message, DateTimeOffset.UtcNow, options.Value.MaxRetries);
            await db.SaveChangesAsync(cancellationToken);
            logger.LogError(exception, "Integration event processing failed; status is {Status}.", inbox.Status);
            return inbox.Status == IntegrationMessageStatus.DeadLetter
                ? InboxProcessingResult.DeadLettered
                : InboxProcessingResult.Failed;
        }
        catch (OperationCanceledException)
        {
            inbox.MarkFailed("Processing was cancelled.", DateTimeOffset.UtcNow, options.Value.MaxRetries);
            await db.SaveChangesAsync(CancellationToken.None);
            throw;
        }
    }
}
