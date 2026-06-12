using Compendium.Domain.SharedKernel;

namespace Compendium.Domain.Sources;

public sealed class RuleSource
{
    private RuleSource()
    {
        RulesetId = null!;
        Code = null!;
        Name = null!;
    }

    private RuleSource(
        CompendiumEntityId id,
        CompendiumEntityId rulesetId,
        SourceCode code,
        SourceName name,
        SourceType type,
        SourceStatus status)
    {
        Id = id;
        RulesetId = rulesetId;
        Code = code;
        Name = name;
        Type = type;
        Status = status;
    }

    public CompendiumEntityId Id { get; private set; } = null!;

    public CompendiumEntityId RulesetId { get; private set; }

    public SourceCode Code { get; private set; }

    public SourceName Name { get; private set; }

    public SourceType Type { get; private set; }

    public SourceStatus Status { get; private set; }

    public DateTimeOffset CreatedAtUtc { get; private set; }

    public DateTimeOffset UpdatedAtUtc { get; private set; }

    public static Result<RuleSource> Create(
        CompendiumEntityId rulesetId,
        SourceCode code,
        SourceName name,
        SourceType type,
        SourceStatus status,
        DateTimeOffset now)
    {
        if (!Enum.IsDefined(type))
        {
            return Result<RuleSource>.Failure(SourceDomainErrors.InvalidStatus("source-type"));
        }

        if (!Enum.IsDefined(status))
        {
            return Result<RuleSource>.Failure(SourceDomainErrors.InvalidStatus("source-status"));
        }

        var source = new RuleSource(CompendiumEntityId.New(), rulesetId, code, name, type, status)
        {
            CreatedAtUtc = now,
            UpdatedAtUtc = now
        };

        return Result<RuleSource>.Success(source);
    }

    public void Activate(DateTimeOffset now)
    {
        Status = SourceStatus.Active;
        UpdatedAtUtc = now;
    }

    public void Deactivate(DateTimeOffset now)
    {
        Status = SourceStatus.Inactive;
        UpdatedAtUtc = now;
    }
}
