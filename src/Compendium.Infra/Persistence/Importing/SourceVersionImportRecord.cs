using Compendium.Domain.Importing;

namespace Compendium.Infra.Persistence.Importing;

public sealed class SourceVersionImportRecord
{
    private SourceVersionImportRecord() { }
    public SourceVersionImportRecord(Guid sourceVersionId, string contentHash, int importedEntityCount, DateTimeOffset now)
    {
        Id = Guid.CreateVersion7();
        SourceVersionId = sourceVersionId;
        ContentHash = contentHash;
        ImportedEntityCount = importedEntityCount;
        ImportedAtUtc = now;
    }

    public Guid Id { get; private set; }
    public Guid SourceVersionId { get; private set; }
    public string ContentHash { get; private set; } = string.Empty;
    public int ImportedEntityCount { get; private set; }
    public DateTimeOffset ImportedAtUtc { get; private set; }
}

public sealed class SourceVersionValidationIssue
{
    private SourceVersionValidationIssue() { }
    public SourceVersionValidationIssue(Guid sourceVersionId, ConsistencyIssue issue, DateTimeOffset now)
    {
        Id = Guid.CreateVersion7();
        SourceVersionId = sourceVersionId;
        Code = issue.Code;
        Severity = issue.Severity;
        Message = issue.Message;
        CreatedAtUtc = now;
    }

    public Guid Id { get; private set; }
    public Guid SourceVersionId { get; private set; }
    public string Code { get; private set; } = string.Empty;
    public ValidationIssueSeverity Severity { get; private set; }
    public string Message { get; private set; } = string.Empty;
    public DateTimeOffset CreatedAtUtc { get; private set; }
}
