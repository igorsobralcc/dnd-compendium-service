using Compendium.Domain.Features;
using Compendium.Domain.SharedKernel;

namespace Compendium.Domain.Origins;

public sealed class Feat
{
    private readonly List<FeatFeature> features = [];

    private Feat()
    {
        RuleSourceId = null!;
        SourceVersionId = null!;
        Code = null!;
        Name = null!;
    }

    private Feat(
        CompendiumEntityId id,
        CompendiumEntityId ruleSourceId,
        CompendiumEntityId sourceVersionId,
        FeatCode code,
        FeatName name,
        FeatDescription? description,
        FeatCategory category,
        bool repeatable)
    {
        Id = id;
        RuleSourceId = ruleSourceId;
        SourceVersionId = sourceVersionId;
        Code = code;
        Name = name;
        Description = description;
        Category = category;
        Repeatable = repeatable;
    }

    public CompendiumEntityId Id { get; private set; } = null!;
    public CompendiumEntityId RuleSourceId { get; private set; }
    public CompendiumEntityId SourceVersionId { get; private set; }
    public FeatCode Code { get; private set; }
    public FeatName Name { get; private set; }
    public FeatDescription? Description { get; private set; }
    public FeatCategory Category { get; private set; }
    public bool Repeatable { get; private set; }
    public IReadOnlyCollection<FeatFeature> Features => features;
    public DateTimeOffset CreatedAtUtc { get; private set; }
    public DateTimeOffset UpdatedAtUtc { get; private set; }

    public static Result<Feat> Create(
        CompendiumEntityId ruleSourceId,
        CompendiumEntityId sourceVersionId,
        FeatCode code,
        FeatName name,
        FeatDescription? description,
        FeatCategory category,
        bool repeatable,
        DateTimeOffset now)
    {
        if (!Enum.IsDefined(category))
        {
            return Result<Feat>.Failure(OriginDomainErrors.InvalidEnum("feat-category"));
        }

        return Result<Feat>.Success(new Feat(
            CompendiumEntityId.New(), ruleSourceId, sourceVersionId, code, name, description, category, repeatable)
        {
            CreatedAtUtc = now,
            UpdatedAtUtc = now
        });
    }

    public Result LinkFeature(CompendiumEntityId featureId, CompendiumEntityId sourceVersionId, DateTimeOffset now)
    {
        if (features.Any(feature => feature.FeatureId == featureId))
        {
            return Result.Failure(OriginDomainErrors.DuplicateFeature(featureId.ToString()));
        }

        features.Add(FeatFeature.Create(Id, featureId, sourceVersionId, null));
        SourceVersionId = sourceVersionId;
        UpdatedAtUtc = now;
        return Result.Success();
    }
}
