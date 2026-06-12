using Compendium.Domain.SharedKernel;

namespace Compendium.Domain.Fundamentals;

public sealed class Skill
{
    private Skill()
    {
        RuleSourceId = null!;
        SourceVersionId = null!;
        Code = null!;
        Name = null!;
    }

    private Skill(
        CompendiumEntityId id,
        CompendiumEntityId ruleSourceId,
        CompendiumEntityId sourceVersionId,
        SkillCode code,
        DisplayName name,
        CompendiumEntityId? defaultAbilityId)
    {
        Id = id;
        RuleSourceId = ruleSourceId;
        SourceVersionId = sourceVersionId;
        Code = code;
        Name = name;
        DefaultAbilityId = defaultAbilityId;
    }

    public CompendiumEntityId Id { get; private set; } = null!;

    public CompendiumEntityId RuleSourceId { get; private set; }

    public CompendiumEntityId SourceVersionId { get; private set; }

    public SkillCode Code { get; private set; }

    public DisplayName Name { get; private set; }

    public CompendiumEntityId? DefaultAbilityId { get; private set; }

    public DateTimeOffset CreatedAtUtc { get; private set; }

    public DateTimeOffset UpdatedAtUtc { get; private set; }

    public static Result<Skill> Create(
        CompendiumEntityId ruleSourceId,
        CompendiumEntityId sourceVersionId,
        SkillCode code,
        DisplayName name,
        CompendiumEntityId? defaultAbilityId,
        DateTimeOffset now)
    {
        var skill = new Skill(CompendiumEntityId.New(), ruleSourceId, sourceVersionId, code, name, defaultAbilityId)
        {
            CreatedAtUtc = now,
            UpdatedAtUtc = now
        };

        return Result<Skill>.Success(skill);
    }

    public void Update(DisplayName name, CompendiumEntityId sourceVersionId, CompendiumEntityId? defaultAbilityId, DateTimeOffset now)
    {
        Name = name;
        SourceVersionId = sourceVersionId;
        DefaultAbilityId = defaultAbilityId;
        UpdatedAtUtc = now;
    }
}
