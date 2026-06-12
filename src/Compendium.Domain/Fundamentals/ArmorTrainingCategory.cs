using Compendium.Domain.SharedKernel;

namespace Compendium.Domain.Fundamentals;

public sealed class ArmorTrainingCategory
{
    private ArmorTrainingCategory()
    {
        RuleSourceId = null!;
        SourceVersionId = null!;
        Code = null!;
        Name = null!;
    }

    private ArmorTrainingCategory(
        CompendiumEntityId id,
        CompendiumEntityId ruleSourceId,
        CompendiumEntityId sourceVersionId,
        ArmorTrainingCategoryCode code,
        DisplayName name,
        int sortOrder)
    {
        Id = id;
        RuleSourceId = ruleSourceId;
        SourceVersionId = sourceVersionId;
        Code = code;
        Name = name;
        SortOrder = sortOrder;
    }

    public CompendiumEntityId Id { get; private set; } = null!;

    public CompendiumEntityId RuleSourceId { get; private set; }

    public CompendiumEntityId SourceVersionId { get; private set; }

    public ArmorTrainingCategoryCode Code { get; private set; }

    public DisplayName Name { get; private set; }

    public int SortOrder { get; private set; }

    public DateTimeOffset CreatedAtUtc { get; private set; }

    public DateTimeOffset UpdatedAtUtc { get; private set; }

    public static Result<ArmorTrainingCategory> Create(
        CompendiumEntityId ruleSourceId,
        CompendiumEntityId sourceVersionId,
        ArmorTrainingCategoryCode code,
        DisplayName name,
        int sortOrder,
        DateTimeOffset now)
    {
        var category = new ArmorTrainingCategory(CompendiumEntityId.New(), ruleSourceId, sourceVersionId, code, name, sortOrder)
        {
            CreatedAtUtc = now,
            UpdatedAtUtc = now
        };

        return Result<ArmorTrainingCategory>.Success(category);
    }
}
