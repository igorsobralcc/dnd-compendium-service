using Compendium.Domain.SharedKernel;

namespace Compendium.Domain.Features;

public sealed class Feature
{
    private readonly List<FeatureEffect> effects = [];

    private Feature()
    {
        RuleSourceId = null!;
        SourceVersionId = null!;
        Code = null!;
        Name = null!;
    }

    private Feature(
        CompendiumEntityId id,
        CompendiumEntityId ruleSourceId,
        CompendiumEntityId sourceVersionId,
        FeatureCode code,
        FeatureName name,
        FeatureDescription? description,
        int? levelRequirement)
    {
        Id = id;
        RuleSourceId = ruleSourceId;
        SourceVersionId = sourceVersionId;
        Code = code;
        Name = name;
        Description = description;
        LevelRequirement = levelRequirement;
    }

    public CompendiumEntityId Id { get; private set; } = null!;
    public CompendiumEntityId RuleSourceId { get; private set; }
    public CompendiumEntityId SourceVersionId { get; private set; }
    public FeatureCode Code { get; private set; }
    public FeatureName Name { get; private set; }
    public FeatureDescription? Description { get; private set; }
    public int? LevelRequirement { get; private set; }
    public IReadOnlyCollection<FeatureEffect> Effects => effects;
    public DateTimeOffset CreatedAtUtc { get; private set; }
    public DateTimeOffset UpdatedAtUtc { get; private set; }

    public static Result<Feature> Create(
        CompendiumEntityId ruleSourceId,
        CompendiumEntityId sourceVersionId,
        FeatureCode code,
        FeatureName name,
        FeatureDescription? description,
        int? levelRequirement,
        DateTimeOffset now)
    {
        if (levelRequirement < 0)
        {
            return Result<Feature>.Failure(FeatureDomainErrors.InvalidLevelRequirement());
        }

        return Result<Feature>.Success(new Feature(
            CompendiumEntityId.New(),
            ruleSourceId,
            sourceVersionId,
            code,
            name,
            description,
            levelRequirement)
        {
            CreatedAtUtc = now,
            UpdatedAtUtc = now
        });
    }

    public Result Update(CompendiumEntityId sourceVersionId, FeatureName name, FeatureDescription? description, int? levelRequirement, DateTimeOffset now)
    {
        if (levelRequirement < 0)
        {
            return Result.Failure(FeatureDomainErrors.InvalidLevelRequirement());
        }

        SourceVersionId = sourceVersionId;
        Name = name;
        Description = description;
        LevelRequirement = levelRequirement;
        UpdatedAtUtc = now;
        return Result.Success();
    }

    public Result AttachEffect(EffectSchema schema, FeatureEffectInput input, DateTimeOffset now)
    {
        var effect = FeatureEffect.Create(Id, schema, input, now);
        if (effect.IsFailure)
        {
            return Result.Failure(effect.Error);
        }

        effects.Add(effect.Value);
        UpdatedAtUtc = now;
        return Result.Success();
    }
}

public abstract class FeatureLink
{
    protected FeatureLink()
    {
        SourceEntityId = null!;
        FeatureId = null!;
        SourceVersionId = null!;
    }

    protected FeatureLink(CompendiumEntityId id, CompendiumEntityId sourceEntityId, CompendiumEntityId featureId, CompendiumEntityId sourceVersionId, int? level)
    {
        Id = id;
        SourceEntityId = sourceEntityId;
        FeatureId = featureId;
        SourceVersionId = sourceVersionId;
        Level = level;
    }

    public CompendiumEntityId Id { get; protected set; } = null!;
    public CompendiumEntityId SourceEntityId { get; protected set; }
    public CompendiumEntityId FeatureId { get; protected set; }
    public CompendiumEntityId SourceVersionId { get; protected set; }
    public int? Level { get; protected set; }
}

public sealed class ClassLevelFeature : FeatureLink
{
    private ClassLevelFeature() { }
    private ClassLevelFeature(CompendiumEntityId id, CompendiumEntityId sourceEntityId, CompendiumEntityId featureId, CompendiumEntityId sourceVersionId, int? level)
        : base(id, sourceEntityId, featureId, sourceVersionId, level) { }
    public static ClassLevelFeature Create(CompendiumEntityId sourceEntityId, CompendiumEntityId featureId, CompendiumEntityId sourceVersionId, int? level) =>
        new(CompendiumEntityId.New(), sourceEntityId, featureId, sourceVersionId, level);
}

public sealed class SpeciesFeature : FeatureLink
{
    private SpeciesFeature() { }
    private SpeciesFeature(CompendiumEntityId id, CompendiumEntityId sourceEntityId, CompendiumEntityId featureId, CompendiumEntityId sourceVersionId, int? level)
        : base(id, sourceEntityId, featureId, sourceVersionId, level) { }
    public static SpeciesFeature Create(CompendiumEntityId sourceEntityId, CompendiumEntityId featureId, CompendiumEntityId sourceVersionId, int? level) =>
        new(CompendiumEntityId.New(), sourceEntityId, featureId, sourceVersionId, level);
}

public sealed class BackgroundFeature : FeatureLink
{
    private BackgroundFeature() { }
    private BackgroundFeature(CompendiumEntityId id, CompendiumEntityId sourceEntityId, CompendiumEntityId featureId, CompendiumEntityId sourceVersionId, int? level)
        : base(id, sourceEntityId, featureId, sourceVersionId, level) { }
    public static BackgroundFeature Create(CompendiumEntityId sourceEntityId, CompendiumEntityId featureId, CompendiumEntityId sourceVersionId, int? level) =>
        new(CompendiumEntityId.New(), sourceEntityId, featureId, sourceVersionId, level);
}

public sealed class FeatFeature : FeatureLink
{
    private FeatFeature() { }
    private FeatFeature(CompendiumEntityId id, CompendiumEntityId sourceEntityId, CompendiumEntityId featureId, CompendiumEntityId sourceVersionId, int? level)
        : base(id, sourceEntityId, featureId, sourceVersionId, level) { }
    public static FeatFeature Create(CompendiumEntityId sourceEntityId, CompendiumEntityId featureId, CompendiumEntityId sourceVersionId, int? level) =>
        new(CompendiumEntityId.New(), sourceEntityId, featureId, sourceVersionId, level);
}

public sealed record FeatureEffectInput(
    EffectType Type,
    EffectTarget Target,
    IReadOnlyCollection<FeatureEffectFieldInput> Fields,
    IReadOnlyCollection<FeatureEffectConditionInput> Conditions);

public sealed record FeatureEffectFieldInput(
    string FieldCode,
    string? TextValue,
    decimal? NumericValue,
    bool? BooleanValue,
    CompendiumEntityId? ReferenceId,
    string? EnumValue);

public sealed record FeatureEffectConditionInput(
    ConditionType Type,
    EffectValueType ValueType,
    string? TextValue,
    decimal? NumericValue,
    bool? BooleanValue,
    CompendiumEntityId? ReferenceId,
    string? EnumValue);
