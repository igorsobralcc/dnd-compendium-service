using Compendium.Domain.SharedKernel;

namespace Compendium.Domain.Sources;

public sealed class SourceVersion
{
    private readonly List<SourceVersionCreated> domainEvents = [];

    private SourceVersion()
    {
        RuleSourceId = null!;
        VersionNumber = null!;
        PublicationDate = null!;
    }

    private SourceVersion(
        CompendiumEntityId id,
        CompendiumEntityId ruleSourceId,
        SourceVersionNumber versionNumber,
        PublicationDate publicationDate,
        ImportStatus importStatus,
        bool isCurrent)
    {
        Id = id;
        RuleSourceId = ruleSourceId;
        VersionNumber = versionNumber;
        PublicationDate = publicationDate;
        ImportStatus = importStatus;
        IsCurrent = isCurrent;
    }

    public CompendiumEntityId Id { get; private set; } = null!;

    public CompendiumEntityId RuleSourceId { get; private set; }

    public SourceVersionNumber VersionNumber { get; private set; }

    public PublicationDate PublicationDate { get; private set; }

    public ImportStatus ImportStatus { get; private set; }

    public bool IsCurrent { get; private set; }

    public DateTimeOffset CreatedAtUtc { get; private set; }

    public DateTimeOffset UpdatedAtUtc { get; private set; }

    public IReadOnlyCollection<SourceVersionCreated> DomainEvents => domainEvents.AsReadOnly();

    public static Result<SourceVersion> Create(
        CompendiumEntityId ruleSourceId,
        SourceVersionNumber versionNumber,
        PublicationDate publicationDate,
        ImportStatus importStatus,
        bool isCurrent,
        DateTimeOffset now)
    {
        if (!Enum.IsDefined(importStatus))
        {
            return Result<SourceVersion>.Failure(SourceDomainErrors.InvalidStatus("import-status"));
        }

        var version = new SourceVersion(
            CompendiumEntityId.New(),
            ruleSourceId,
            versionNumber,
            publicationDate,
            importStatus,
            isCurrent)
        {
            CreatedAtUtc = now,
            UpdatedAtUtc = now
        };

        version.domainEvents.Add(new SourceVersionCreated(
            Guid.CreateVersion7(),
            version.Id.Value,
            ruleSourceId.Value,
            versionNumber.Value,
            now));

        return Result<SourceVersion>.Success(version);
    }

    public void MarkAsCurrent(DateTimeOffset now)
    {
        IsCurrent = true;
        UpdatedAtUtc = now;
    }

    public void MarkAsNotCurrent(DateTimeOffset now)
    {
        IsCurrent = false;
        UpdatedAtUtc = now;
    }

    public void MarkAsImported(DateTimeOffset now)
    {
        ImportStatus = ImportStatus.Imported;
        UpdatedAtUtc = now;
    }

    public void ClearDomainEvents() => domainEvents.Clear();
}
