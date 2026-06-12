using Compendium.Domain.SharedKernel;

namespace Compendium.Domain.Sources;

public sealed class Ruleset
{
    private Ruleset()
    {
        Code = null!;
        Name = null!;
        Version = null!;
    }

    private Ruleset(CompendiumEntityId id, RulesetCode code, RulesetName name, RulesetVersion version, RulesetStatus status)
    {
        Id = id;
        Code = code;
        Name = name;
        Version = version;
        Status = status;
    }

    public CompendiumEntityId Id { get; private set; } = null!;

    public RulesetCode Code { get; private set; }

    public RulesetName Name { get; private set; }

    public RulesetVersion Version { get; private set; }

    public RulesetStatus Status { get; private set; }

    public DateTimeOffset CreatedAtUtc { get; private set; }

    public DateTimeOffset UpdatedAtUtc { get; private set; }

    public static Result<Ruleset> Create(
        RulesetCode code,
        RulesetName name,
        RulesetVersion version,
        RulesetStatus status,
        DateTimeOffset now)
    {
        if (!Enum.IsDefined(status))
        {
            return Result<Ruleset>.Failure(SourceDomainErrors.InvalidStatus("ruleset-status"));
        }

        var ruleset = new Ruleset(CompendiumEntityId.New(), code, name, version, status)
        {
            CreatedAtUtc = now,
            UpdatedAtUtc = now
        };

        return Result<Ruleset>.Success(ruleset);
    }

    public Result Update(RulesetName name, RulesetVersion version, RulesetStatus status, DateTimeOffset now)
    {
        if (!Enum.IsDefined(status))
        {
            return Result.Failure(SourceDomainErrors.InvalidStatus("ruleset-status"));
        }

        Name = name;
        Version = version;
        Status = status;
        UpdatedAtUtc = now;
        return Result.Success();
    }
}
