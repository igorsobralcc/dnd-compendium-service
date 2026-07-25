namespace Compendium.Infra.Persistence.InternalQueries;

public sealed class CompendiumChange
{
    private CompendiumChange() { }

    public CompendiumChange(
        Guid? sourceVersionId,
        string entityType,
        Guid entityId,
        string changeType,
        DateTimeOffset changedAtUtc)
    {
        SourceVersionId = sourceVersionId;
        EntityType = entityType;
        EntityId = entityId;
        ChangeType = changeType;
        ChangedAtUtc = changedAtUtc;
    }

    public long Revision { get; private set; }
    public Guid? SourceVersionId { get; private set; }
    public string EntityType { get; private set; } = string.Empty;
    public Guid EntityId { get; private set; }
    public string ChangeType { get; private set; } = string.Empty;
    public DateTimeOffset ChangedAtUtc { get; private set; }
}

