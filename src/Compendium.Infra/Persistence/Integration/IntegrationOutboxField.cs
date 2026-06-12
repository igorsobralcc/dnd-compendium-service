namespace Compendium.Infra.Persistence.Integration;

public sealed class IntegrationOutboxField
{
    private IntegrationOutboxField()
    {
    }

    public IntegrationOutboxField(
        Guid outboxId,
        string fieldName,
        string fieldType,
        DateTimeOffset createdAtUtc)
    {
        Id = Guid.CreateVersion7();
        OutboxId = outboxId;
        FieldName = fieldName;
        FieldType = fieldType;
        CreatedAtUtc = createdAtUtc;
    }

    public Guid Id { get; private set; }

    public Guid OutboxId { get; private set; }

    public string FieldName { get; private set; } = string.Empty;

    public string FieldType { get; private set; } = string.Empty;

    public string? TextValue { get; private set; }

    public decimal? NumberValue { get; private set; }

    public bool? BooleanValue { get; private set; }

    public string? ReferenceValue { get; private set; }

    public string? EnumValue { get; private set; }

    public DateTimeOffset CreatedAtUtc { get; private set; }

    public IntegrationOutbox Outbox { get; private set; } = null!;
}
