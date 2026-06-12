using Compendium.Domain.SharedKernel;

namespace Compendium.Domain.Classes;

public sealed class CharacterSubclass
{
    private readonly List<SubclassFeature> features = [];

    private CharacterSubclass()
    {
        CharacterClassId = null!;
        RuleSourceId = null!;
        SourceVersionId = null!;
        Code = null!;
        Name = null!;
    }

    private CharacterSubclass(
        CompendiumEntityId id,
        CompendiumEntityId characterClassId,
        CompendiumEntityId ruleSourceId,
        CompendiumEntityId sourceVersionId,
        ClassCode code,
        ClassName name,
        ClassDescription? description)
    {
        Id = id;
        CharacterClassId = characterClassId;
        RuleSourceId = ruleSourceId;
        SourceVersionId = sourceVersionId;
        Code = code;
        Name = name;
        Description = description;
    }

    public CompendiumEntityId Id { get; private set; } = null!;

    public CompendiumEntityId CharacterClassId { get; private set; }

    public CompendiumEntityId RuleSourceId { get; private set; }

    public CompendiumEntityId SourceVersionId { get; private set; }

    public ClassCode Code { get; private set; }

    public ClassName Name { get; private set; }

    public ClassDescription? Description { get; private set; }

    public IReadOnlyCollection<SubclassFeature> Features => features;

    public DateTimeOffset CreatedAtUtc { get; private set; }

    public DateTimeOffset UpdatedAtUtc { get; private set; }

    public static Result<CharacterSubclass> Create(
        CompendiumEntityId characterClassId,
        CompendiumEntityId ruleSourceId,
        CompendiumEntityId sourceVersionId,
        ClassCode code,
        ClassName name,
        ClassDescription? description,
        DateTimeOffset now)
    {
        var subclass = new CharacterSubclass(
            CompendiumEntityId.New(),
            characterClassId,
            ruleSourceId,
            sourceVersionId,
            code,
            name,
            description)
        {
            CreatedAtUtc = now,
            UpdatedAtUtc = now
        };

        return Result<CharacterSubclass>.Success(subclass);
    }

    public Result LinkFeature(CompendiumEntityId featureId, CompendiumEntityId sourceVersionId, int level, DateTimeOffset now)
    {
        if (!CharacterClass.IsSupportedLevel(level))
        {
            return Result.Failure(ClassDomainErrors.InvalidSubclassFeatureLevel(level));
        }

        if (features.Any(feature => feature.FeatureId == featureId && feature.Level == level))
        {
            return Result.Failure(ClassDomainErrors.DuplicateSubclassFeature(featureId.ToString(), level));
        }

        features.Add(SubclassFeature.Create(CompendiumEntityId.New(), Id, sourceVersionId, featureId, level));
        SourceVersionId = sourceVersionId;
        UpdatedAtUtc = now;
        return Result.Success();
    }
}

public sealed class SubclassFeature
{
    private SubclassFeature()
    {
        CharacterSubclassId = null!;
        SourceVersionId = null!;
        FeatureId = null!;
    }

    private SubclassFeature(
        CompendiumEntityId id,
        CompendiumEntityId characterSubclassId,
        CompendiumEntityId sourceVersionId,
        CompendiumEntityId featureId,
        int level)
    {
        Id = id;
        CharacterSubclassId = characterSubclassId;
        SourceVersionId = sourceVersionId;
        FeatureId = featureId;
        Level = level;
    }

    public CompendiumEntityId Id { get; private set; } = null!;

    public CompendiumEntityId CharacterSubclassId { get; private set; }

    public CompendiumEntityId SourceVersionId { get; private set; }

    public CompendiumEntityId FeatureId { get; private set; }

    public int Level { get; private set; }

    public static SubclassFeature Create(
        CompendiumEntityId id,
        CompendiumEntityId characterSubclassId,
        CompendiumEntityId sourceVersionId,
        CompendiumEntityId featureId,
        int level) =>
        new(id, characterSubclassId, sourceVersionId, featureId, level);
}
