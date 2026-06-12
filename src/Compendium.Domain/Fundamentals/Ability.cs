using Compendium.Domain.SharedKernel;

namespace Compendium.Domain.Fundamentals;

public sealed class Ability
{
    private Ability()
    {
        RuleSourceId = null!;
        SourceVersionId = null!;
        Code = null!;
        Name = null!;
    }

    private Ability(
        CompendiumEntityId id,
        CompendiumEntityId ruleSourceId,
        CompendiumEntityId sourceVersionId,
        AbilityCode code,
        DisplayName name)
    {
        Id = id;
        RuleSourceId = ruleSourceId;
        SourceVersionId = sourceVersionId;
        Code = code;
        Name = name;
    }

    public CompendiumEntityId Id { get; private set; } = null!;

    public CompendiumEntityId RuleSourceId { get; private set; }

    public CompendiumEntityId SourceVersionId { get; private set; }

    public AbilityCode Code { get; private set; }

    public DisplayName Name { get; private set; }

    public DateTimeOffset CreatedAtUtc { get; private set; }

    public DateTimeOffset UpdatedAtUtc { get; private set; }

    public static Result<Ability> Create(
        CompendiumEntityId ruleSourceId,
        CompendiumEntityId sourceVersionId,
        AbilityCode code,
        DisplayName name,
        DateTimeOffset now)
    {
        var ability = new Ability(CompendiumEntityId.New(), ruleSourceId, sourceVersionId, code, name)
        {
            CreatedAtUtc = now,
            UpdatedAtUtc = now
        };

        return Result<Ability>.Success(ability);
    }

    public void Update(DisplayName name, CompendiumEntityId sourceVersionId, DateTimeOffset now)
    {
        Name = name;
        SourceVersionId = sourceVersionId;
        UpdatedAtUtc = now;
    }
}
