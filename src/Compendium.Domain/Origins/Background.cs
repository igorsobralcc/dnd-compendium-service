using Compendium.Domain.Features;
using Compendium.Domain.SharedKernel;

namespace Compendium.Domain.Origins;

public sealed class Background
{
    private readonly List<BackgroundAbilityOption> abilityOptions = [];
    private readonly List<BackgroundAbilityBoostRule> abilityBoostRules = [];
    private readonly List<BackgroundFeatGrant> featGrants = [];
    private readonly List<BackgroundSkillProficiency> skillProficiencies = [];
    private readonly List<BackgroundToolProficiency> toolProficiencies = [];
    private readonly List<BackgroundStartingEquipmentRule> startingEquipmentRules = [];
    private readonly List<BackgroundFeature> features = [];

    private Background()
    {
        RuleSourceId = null!;
        SourceVersionId = null!;
        Code = null!;
        Name = null!;
    }

    private Background(
        CompendiumEntityId id,
        CompendiumEntityId ruleSourceId,
        CompendiumEntityId sourceVersionId,
        BackgroundCode code,
        BackgroundName name,
        BackgroundDescription? description)
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
    public BackgroundCode Code { get; private set; }
    public BackgroundName Name { get; private set; }
    public BackgroundDescription? Description { get; private set; }
    public IReadOnlyCollection<BackgroundAbilityOption> AbilityOptions => abilityOptions;
    public IReadOnlyCollection<BackgroundAbilityBoostRule> AbilityBoostRules => abilityBoostRules;
    public IReadOnlyCollection<BackgroundFeatGrant> FeatGrants => featGrants;
    public IReadOnlyCollection<BackgroundSkillProficiency> SkillProficiencies => skillProficiencies;
    public IReadOnlyCollection<BackgroundToolProficiency> ToolProficiencies => toolProficiencies;
    public IReadOnlyCollection<BackgroundStartingEquipmentRule> StartingEquipmentRules => startingEquipmentRules;
    public IReadOnlyCollection<BackgroundFeature> Features => features;
    public DateTimeOffset CreatedAtUtc { get; private set; }
    public DateTimeOffset UpdatedAtUtc { get; private set; }

    public static Result<Background> Create(
        CompendiumEntityId ruleSourceId,
        CompendiumEntityId sourceVersionId,
        BackgroundCode code,
        BackgroundName name,
        BackgroundDescription? description,
        DateTimeOffset now) =>
        Result<Background>.Success(new Background(CompendiumEntityId.New(), ruleSourceId, sourceVersionId, code, name, description)
        {
            CreatedAtUtc = now,
            UpdatedAtUtc = now
        });

    public Result ConfigureMechanics(BackgroundMechanicsInput input, CompendiumEntityId sourceVersionId, DateTimeOffset now)
    {
        if (input.AbilityOptionIds.Count != 3 || input.AbilityOptionIds.Distinct().Count() != 3)
        {
            return Result.Failure(OriginDomainErrors.AbilityOptionsRequired());
        }

        var orderedRules = input.AbilityBoostRules.OrderBy(rule => rule.BoostAmount).ThenBy(rule => rule.AbilityCount).ToArray();
        var validTwoOne = orderedRules.Length == 2 &&
            orderedRules[0] == new BackgroundAbilityBoostRuleInput(1, 1) &&
            orderedRules[1] == new BackgroundAbilityBoostRuleInput(2, 1);
        var validThreeOnes = orderedRules.Length == 1 &&
            orderedRules[0] == new BackgroundAbilityBoostRuleInput(1, 3);
        if (!validTwoOne && !validThreeOnes)
        {
            return Result.Failure(OriginDomainErrors.InvalidAbilityBoostRules());
        }

        if (input.FeatIds.Count != 1) return Result.Failure(OriginDomainErrors.FeatGrantRequired());
        if (input.SkillProficiencyIds.Count != 2 || input.SkillProficiencyIds.Distinct().Count() != 2)
            return Result.Failure(OriginDomainErrors.SkillProficienciesRequired());
        if (input.ToolProficiencyIds.Count != 1) return Result.Failure(OriginDomainErrors.ToolProficiencyRequired());
        if (input.StartingEquipmentRules.Count == 0) return Result.Failure(OriginDomainErrors.StartingEquipmentRequired());
        if (input.StartingEquipmentRules.Any(rule => !Enum.IsDefined(rule.ReferenceType)))
            return Result.Failure(OriginDomainErrors.InvalidEnum("starting-equipment-reference-type"));

        abilityOptions.Clear();
        abilityOptions.AddRange(input.AbilityOptionIds.Select((id, index) => BackgroundAbilityOption.Create(Id, id, index + 1)));
        abilityBoostRules.Clear();
        abilityBoostRules.AddRange(input.AbilityBoostRules.Select(rule => BackgroundAbilityBoostRule.Create(Id, rule.BoostAmount, rule.AbilityCount)));
        featGrants.Clear();
        featGrants.AddRange(input.FeatIds.Select(id => BackgroundFeatGrant.Create(Id, id)));
        skillProficiencies.Clear();
        skillProficiencies.AddRange(input.SkillProficiencyIds.Select(id => BackgroundSkillProficiency.Create(Id, id)));
        toolProficiencies.Clear();
        toolProficiencies.AddRange(input.ToolProficiencyIds.Select(id => BackgroundToolProficiency.Create(Id, id)));
        startingEquipmentRules.Clear();
        startingEquipmentRules.AddRange(input.StartingEquipmentRules.Select(rule =>
            BackgroundStartingEquipmentRule.Create(Id, rule.ReferenceId, rule.ReferenceType)));
        SourceVersionId = sourceVersionId;
        UpdatedAtUtc = now;
        return Result.Success();
    }

    public Result LinkFeature(CompendiumEntityId featureId, CompendiumEntityId sourceVersionId, DateTimeOffset now)
    {
        if (features.Any(feature => feature.FeatureId == featureId))
        {
            return Result.Failure(OriginDomainErrors.DuplicateFeature(featureId.ToString()));
        }

        features.Add(BackgroundFeature.Create(Id, featureId, sourceVersionId, null));
        SourceVersionId = sourceVersionId;
        UpdatedAtUtc = now;
        return Result.Success();
    }
}

public sealed class BackgroundAbilityOption
{
    private BackgroundAbilityOption() { BackgroundId = null!; AbilityId = null!; }
    private BackgroundAbilityOption(CompendiumEntityId id, CompendiumEntityId backgroundId, CompendiumEntityId abilityId, int sortOrder)
        => (Id, BackgroundId, AbilityId, SortOrder) = (id, backgroundId, abilityId, sortOrder);
    public CompendiumEntityId Id { get; private set; } = null!;
    public CompendiumEntityId BackgroundId { get; private set; }
    public CompendiumEntityId AbilityId { get; private set; }
    public int SortOrder { get; private set; }
    public static BackgroundAbilityOption Create(CompendiumEntityId backgroundId, CompendiumEntityId abilityId, int sortOrder) =>
        new(CompendiumEntityId.New(), backgroundId, abilityId, sortOrder);
}

public sealed class BackgroundAbilityBoostRule
{
    private BackgroundAbilityBoostRule() { BackgroundId = null!; }
    private BackgroundAbilityBoostRule(CompendiumEntityId id, CompendiumEntityId backgroundId, int boostAmount, int abilityCount)
        => (Id, BackgroundId, BoostAmount, AbilityCount) = (id, backgroundId, boostAmount, abilityCount);
    public CompendiumEntityId Id { get; private set; } = null!;
    public CompendiumEntityId BackgroundId { get; private set; }
    public int BoostAmount { get; private set; }
    public int AbilityCount { get; private set; }
    public static BackgroundAbilityBoostRule Create(CompendiumEntityId backgroundId, int boostAmount, int abilityCount) =>
        new(CompendiumEntityId.New(), backgroundId, boostAmount, abilityCount);
}

public sealed class BackgroundFeatGrant
{
    private BackgroundFeatGrant() { BackgroundId = null!; FeatId = null!; }
    private BackgroundFeatGrant(CompendiumEntityId id, CompendiumEntityId backgroundId, CompendiumEntityId featId)
        => (Id, BackgroundId, FeatId) = (id, backgroundId, featId);
    public CompendiumEntityId Id { get; private set; } = null!;
    public CompendiumEntityId BackgroundId { get; private set; }
    public CompendiumEntityId FeatId { get; private set; }
    public static BackgroundFeatGrant Create(CompendiumEntityId backgroundId, CompendiumEntityId featId) =>
        new(CompendiumEntityId.New(), backgroundId, featId);
}

public sealed class BackgroundSkillProficiency
{
    private BackgroundSkillProficiency() { BackgroundId = null!; ProficiencyId = null!; }
    private BackgroundSkillProficiency(CompendiumEntityId id, CompendiumEntityId backgroundId, CompendiumEntityId proficiencyId)
        => (Id, BackgroundId, ProficiencyId) = (id, backgroundId, proficiencyId);
    public CompendiumEntityId Id { get; private set; } = null!;
    public CompendiumEntityId BackgroundId { get; private set; }
    public CompendiumEntityId ProficiencyId { get; private set; }
    public static BackgroundSkillProficiency Create(CompendiumEntityId backgroundId, CompendiumEntityId proficiencyId) =>
        new(CompendiumEntityId.New(), backgroundId, proficiencyId);
}

public sealed class BackgroundToolProficiency
{
    private BackgroundToolProficiency() { BackgroundId = null!; ProficiencyId = null!; }
    private BackgroundToolProficiency(CompendiumEntityId id, CompendiumEntityId backgroundId, CompendiumEntityId proficiencyId)
        => (Id, BackgroundId, ProficiencyId) = (id, backgroundId, proficiencyId);
    public CompendiumEntityId Id { get; private set; } = null!;
    public CompendiumEntityId BackgroundId { get; private set; }
    public CompendiumEntityId ProficiencyId { get; private set; }
    public static BackgroundToolProficiency Create(CompendiumEntityId backgroundId, CompendiumEntityId proficiencyId) =>
        new(CompendiumEntityId.New(), backgroundId, proficiencyId);
}

public sealed class BackgroundStartingEquipmentRule
{
    private BackgroundStartingEquipmentRule() { BackgroundId = null!; ReferenceId = null!; }
    private BackgroundStartingEquipmentRule(CompendiumEntityId id, CompendiumEntityId backgroundId, CompendiumEntityId referenceId, StartingEquipmentReferenceType referenceType)
        => (Id, BackgroundId, ReferenceId, ReferenceType) = (id, backgroundId, referenceId, referenceType);
    public CompendiumEntityId Id { get; private set; } = null!;
    public CompendiumEntityId BackgroundId { get; private set; }
    public CompendiumEntityId ReferenceId { get; private set; }
    public StartingEquipmentReferenceType ReferenceType { get; private set; }
    public static BackgroundStartingEquipmentRule Create(CompendiumEntityId backgroundId, CompendiumEntityId referenceId, StartingEquipmentReferenceType referenceType) =>
        new(CompendiumEntityId.New(), backgroundId, referenceId, referenceType);
}

public sealed record BackgroundMechanicsInput(
    IReadOnlyCollection<CompendiumEntityId> AbilityOptionIds,
    IReadOnlyCollection<BackgroundAbilityBoostRuleInput> AbilityBoostRules,
    IReadOnlyCollection<CompendiumEntityId> FeatIds,
    IReadOnlyCollection<CompendiumEntityId> SkillProficiencyIds,
    IReadOnlyCollection<CompendiumEntityId> ToolProficiencyIds,
    IReadOnlyCollection<BackgroundStartingEquipmentRuleInput> StartingEquipmentRules);

public sealed record BackgroundAbilityBoostRuleInput(int BoostAmount, int AbilityCount);
public sealed record BackgroundStartingEquipmentRuleInput(CompendiumEntityId ReferenceId, StartingEquipmentReferenceType ReferenceType);
