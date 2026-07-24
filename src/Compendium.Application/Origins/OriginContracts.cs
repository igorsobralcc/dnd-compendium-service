using Compendium.Application.Features;
using Compendium.Domain.Features;
using Compendium.Domain.Origins;

namespace Compendium.Application.Origins;

public sealed record CreateSpeciesCommand(Guid RuleSourceId, Guid SourceVersionId, string Code, string Name, string? Description);
public sealed record LinkSpeciesFeatureCommand(string SpeciesCode, Guid RuleSourceId, Guid SourceVersionId, Guid FeatureId);
public sealed record CreateBackgroundCommand(Guid RuleSourceId, Guid SourceVersionId, string Code, string Name, string? Description);
public sealed record ConfigureBackgroundMechanicsCommand(
    string BackgroundCode,
    Guid RuleSourceId,
    Guid SourceVersionId,
    IReadOnlyCollection<Guid> AbilityOptionIds,
    IReadOnlyCollection<BackgroundAbilityBoostRuleCommand> AbilityBoostRules,
    IReadOnlyCollection<Guid> FeatIds,
    IReadOnlyCollection<Guid> SkillProficiencyIds,
    IReadOnlyCollection<Guid> ToolProficiencyIds,
    IReadOnlyCollection<BackgroundStartingEquipmentRuleCommand> StartingEquipmentRules);
public sealed record LinkBackgroundFeatureCommand(string BackgroundCode, Guid RuleSourceId, Guid SourceVersionId, Guid FeatureId);
public sealed record CreateFeatCommand(Guid RuleSourceId, Guid SourceVersionId, string Code, string Name, string? Description, FeatCategory Category, bool Repeatable);
public sealed record LinkFeatFeatureCommand(string FeatCode, Guid RuleSourceId, Guid SourceVersionId, Guid FeatureId);
public sealed record AddFeatPrerequisiteCommand(
    string FeatCode,
    PrerequisiteType Type,
    ComparisonOperator Operator,
    EffectTarget Target,
    EffectValueType ValueType,
    string? TextValue,
    decimal? NumericValue,
    bool? BooleanValue,
    Guid? ReferenceId,
    string? EnumValue);

public sealed record BackgroundAbilityBoostRuleCommand(int BoostAmount, int AbilityCount);
public sealed record BackgroundStartingEquipmentRuleCommand(Guid ReferenceId, StartingEquipmentReferenceType ReferenceType);

public sealed record SpeciesSummaryDto(Guid Id, Guid RuleSourceId, Guid SourceVersionId, string Code, string Name, string? Description);
public sealed record SpeciesDetailsDto(
    Guid Id, Guid RuleSourceId, Guid SourceVersionId, string Code, string Name, string? Description,
    IReadOnlyCollection<OriginFeatureDto> Features, IReadOnlyCollection<ChoiceSetDto> Choices);

public sealed record BackgroundSummaryDto(Guid Id, Guid RuleSourceId, Guid SourceVersionId, string Code, string Name, string? Description);
public sealed record BackgroundDetailsDto(
    Guid Id, Guid RuleSourceId, Guid SourceVersionId, string Code, string Name, string? Description,
    IReadOnlyCollection<BackgroundAbilityOptionDto> AbilityOptions,
    IReadOnlyCollection<BackgroundAbilityBoostRuleDto> AbilityBoostRules,
    IReadOnlyCollection<BackgroundFeatGrantDto> FeatGrants,
    IReadOnlyCollection<BackgroundProficiencyDto> SkillProficiencies,
    IReadOnlyCollection<BackgroundProficiencyDto> ToolProficiencies,
    IReadOnlyCollection<BackgroundStartingEquipmentRuleDto> StartingEquipmentRules,
    IReadOnlyCollection<OriginFeatureDto> Features,
    IReadOnlyCollection<ChoiceSetDto> Choices);

public sealed record FeatSummaryDto(Guid Id, Guid RuleSourceId, Guid SourceVersionId, string Code, string Name, string? Description, FeatCategory Category, bool Repeatable);
public sealed record FeatDetailsDto(
    Guid Id, Guid RuleSourceId, Guid SourceVersionId, string Code, string Name, string? Description, FeatCategory Category, bool Repeatable,
    IReadOnlyCollection<OriginFeatureDto> Features,
    IReadOnlyCollection<EntityPrerequisiteDto> Prerequisites,
    IReadOnlyCollection<ChoiceSetDto> Choices);

public sealed record OriginFeatureDto(Guid Id, Guid FeatureId, Guid SourceVersionId);
public sealed record BackgroundAbilityOptionDto(Guid Id, Guid AbilityId, int SortOrder);
public sealed record BackgroundAbilityBoostRuleDto(Guid Id, int BoostAmount, int AbilityCount);
public sealed record BackgroundFeatGrantDto(Guid Id, Guid FeatId);
public sealed record BackgroundProficiencyDto(Guid Id, Guid ProficiencyId);
public sealed record BackgroundStartingEquipmentRuleDto(Guid Id, Guid ReferenceId, StartingEquipmentReferenceType ReferenceType);
