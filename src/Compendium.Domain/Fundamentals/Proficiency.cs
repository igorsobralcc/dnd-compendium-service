using Compendium.Domain.SharedKernel;

namespace Compendium.Domain.Fundamentals;

public sealed class Proficiency
{
    private Proficiency()
    {
        RuleSourceId = null!;
        SourceVersionId = null!;
        Code = null!;
        Name = null!;
    }

    private Proficiency(
        CompendiumEntityId id,
        CompendiumEntityId ruleSourceId,
        CompendiumEntityId sourceVersionId,
        ProficiencyCode code,
        DisplayName name,
        ProficiencyType type,
        CompendiumEntityId? relatedEntityId)
    {
        Id = id;
        RuleSourceId = ruleSourceId;
        SourceVersionId = sourceVersionId;
        Code = code;
        Name = name;
        Type = type;
        RelatedEntityId = relatedEntityId;
    }

    public CompendiumEntityId Id { get; private set; } = null!;

    public CompendiumEntityId RuleSourceId { get; private set; }

    public CompendiumEntityId SourceVersionId { get; private set; }

    public ProficiencyCode Code { get; private set; }

    public DisplayName Name { get; private set; }

    public ProficiencyType Type { get; private set; }

    public CompendiumEntityId? RelatedEntityId { get; private set; }

    public DateTimeOffset CreatedAtUtc { get; private set; }

    public DateTimeOffset UpdatedAtUtc { get; private set; }

    public static Result<Proficiency> Create(
        CompendiumEntityId ruleSourceId,
        CompendiumEntityId sourceVersionId,
        ProficiencyCode code,
        DisplayName name,
        ProficiencyType type,
        CompendiumEntityId? relatedEntityId,
        DateTimeOffset now)
    {
        if (!Enum.IsDefined(type))
        {
            return Result<Proficiency>.Failure(FundamentalDomainErrors.InvalidStatus("proficiency-type"));
        }

        var proficiency = new Proficiency(CompendiumEntityId.New(), ruleSourceId, sourceVersionId, code, name, type, relatedEntityId)
        {
            CreatedAtUtc = now,
            UpdatedAtUtc = now
        };

        return Result<Proficiency>.Success(proficiency);
    }

    public Result Update(DisplayName name, CompendiumEntityId sourceVersionId, ProficiencyType type, CompendiumEntityId? relatedEntityId, DateTimeOffset now)
    {
        if (!Enum.IsDefined(type))
        {
            return Result.Failure(FundamentalDomainErrors.InvalidStatus("proficiency-type"));
        }

        Name = name;
        SourceVersionId = sourceVersionId;
        Type = type;
        RelatedEntityId = relatedEntityId;
        UpdatedAtUtc = now;
        return Result.Success();
    }
}
