using Compendium.Domain.SharedKernel;

namespace Compendium.Domain.Fundamentals;

public sealed class HitDie
{
    private HitDie()
    {
        RuleSourceId = null!;
        SourceVersionId = null!;
        Code = null!;
        Name = null!;
    }

    private HitDie(
        CompendiumEntityId id,
        CompendiumEntityId ruleSourceId,
        CompendiumEntityId sourceVersionId,
        HitDieCode code,
        DisplayName name,
        int die)
    {
        Id = id;
        RuleSourceId = ruleSourceId;
        SourceVersionId = sourceVersionId;
        Code = code;
        Name = name;
        Die = die;
    }

    public CompendiumEntityId Id { get; private set; } = null!;

    public CompendiumEntityId RuleSourceId { get; private set; }

    public CompendiumEntityId SourceVersionId { get; private set; }

    public HitDieCode Code { get; private set; }

    public DisplayName Name { get; private set; }

    public int Die { get; private set; }

    public DateTimeOffset CreatedAtUtc { get; private set; }

    public DateTimeOffset UpdatedAtUtc { get; private set; }

    public static Result<HitDie> Create(
        CompendiumEntityId ruleSourceId,
        CompendiumEntityId sourceVersionId,
        int die,
        DateTimeOffset now)
    {
        var code = HitDieCode.Create(die);
        if (code.IsFailure)
        {
            return Result<HitDie>.Failure(code.Error);
        }

        var name = DisplayName.Create($"d{die}");
        if (name.IsFailure)
        {
            return Result<HitDie>.Failure(name.Error);
        }

        var hitDie = new HitDie(CompendiumEntityId.New(), ruleSourceId, sourceVersionId, code.Value, name.Value, die)
        {
            CreatedAtUtc = now,
            UpdatedAtUtc = now
        };

        return Result<HitDie>.Success(hitDie);
    }
}
