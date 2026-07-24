using Compendium.Domain.Features;
using Compendium.Domain.SharedKernel;

namespace Compendium.Domain.Origins;

public sealed class Species
{
    private readonly List<SpeciesFeature> features = [];

    private Species()
    {
        RuleSourceId = null!;
        SourceVersionId = null!;
        Code = null!;
        Name = null!;
    }

    private Species(
        CompendiumEntityId id,
        CompendiumEntityId ruleSourceId,
        CompendiumEntityId sourceVersionId,
        SpeciesCode code,
        SpeciesName name,
        SpeciesDescription? description)
    {
        Id = id;
        RuleSourceId = ruleSourceId;
        SourceVersionId = sourceVersionId;
        Code = code;
        Name = name;
        Description = description;
    }

    public CompendiumEntityId Id { get; private set; } = null!;
    public CompendiumEntityId RuleSourceId { get; private set; }
    public CompendiumEntityId SourceVersionId { get; private set; }
    public SpeciesCode Code { get; private set; }
    public SpeciesName Name { get; private set; }
    public SpeciesDescription? Description { get; private set; }
    public IReadOnlyCollection<SpeciesFeature> Features => features;
    public DateTimeOffset CreatedAtUtc { get; private set; }
    public DateTimeOffset UpdatedAtUtc { get; private set; }

    public static Result<Species> Create(
        CompendiumEntityId ruleSourceId,
        CompendiumEntityId sourceVersionId,
        SpeciesCode code,
        SpeciesName name,
        SpeciesDescription? description,
        DateTimeOffset now) =>
        Result<Species>.Success(new Species(CompendiumEntityId.New(), ruleSourceId, sourceVersionId, code, name, description)
        {
            CreatedAtUtc = now,
            UpdatedAtUtc = now
        });

    public Result LinkFeature(CompendiumEntityId featureId, CompendiumEntityId sourceVersionId, DateTimeOffset now)
    {
        if (features.Any(feature => feature.FeatureId == featureId))
        {
            return Result.Failure(OriginDomainErrors.DuplicateFeature(featureId.ToString()));
        }

        features.Add(SpeciesFeature.Create(Id, featureId, sourceVersionId, null));
        SourceVersionId = sourceVersionId;
        UpdatedAtUtc = now;
        return Result.Success();
    }
}
