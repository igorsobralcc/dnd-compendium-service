using Compendium.Application.Contracts.Events;
using Compendium.Application.Integration;
using Compendium.Infra.Persistence;
using Compendium.Infra.Persistence.Integration;

namespace Compendium.Infra.Integration;

internal sealed class OutboxEventPublisher(CompendiumDbContext db) : IEventPublisher
{
    public async Task EnqueueAsync(
        string eventName,
        int eventVersion,
        string aggregateType,
        string aggregateId,
        string correlationId,
        DateTimeOffset occurredAtUtc,
        IReadOnlyCollection<IntegrationEventField> fields,
        CancellationToken cancellationToken)
    {
        var message = new IntegrationOutbox(
            eventName,
            eventVersion,
            aggregateType,
            aggregateId,
            correlationId,
            occurredAtUtc);

        foreach (var field in fields)
        {
            message.Fields.Add(new IntegrationOutboxField(
                message.Id,
                field.Name,
                field.Type,
                occurredAtUtc,
                textValue: field.TextValue,
                referenceValue: field.ReferenceValue,
                enumValue: field.EnumValue,
                numberValue: field.NumberValue,
                booleanValue: field.BooleanValue));
        }

        await db.IntegrationOutbox.AddAsync(message, cancellationToken);
    }
}
