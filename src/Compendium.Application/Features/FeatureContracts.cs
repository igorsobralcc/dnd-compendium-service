using Compendium.Domain.Features;

namespace Compendium.Application.Features;

public sealed record CreateFeatureCommand(Guid RuleSourceId, Guid SourceVersionId, string Code, string Name, string? Description, int? LevelRequirement);
public sealed record UpdateFeatureCommand(string Code, Guid RuleSourceId, Guid SourceVersionId, string Name, string? Description, int? LevelRequirement);
public sealed record CreateEffectSchemaCommand(string Code, string Name, EffectType Type, IReadOnlyCollection<EffectSchemaFieldCommand> Fields);
public sealed record AttachEffectToFeatureCommand(string FeatureCode, string EffectSchemaCode, EffectType Type, EffectTarget Target, IReadOnlyCollection<TypedValueFieldCommand> Fields, IReadOnlyCollection<FeatureEffectConditionCommand> Conditions);
public sealed record AddPrerequisiteToEntityCommand(CompendiumEntityKind EntityKind, Guid EntityId, PrerequisiteType Type, ComparisonOperator Operator, EffectTarget Target, EffectValueType ValueType, string? TextValue, decimal? NumericValue, bool? BooleanValue, Guid? ReferenceId, string? EnumValue);
public sealed record RemovePrerequisiteFromEntityCommand(Guid PrerequisiteId);
public sealed record CreateChoiceSetCommand(CompendiumEntityKind SourceEntityKind, Guid SourceEntityId, string Code, int MinimumChoices, int MaximumChoices);
public sealed record AddChoiceOptionCommand(string ChoiceSetCode, ChoiceOptionType Type, Guid? ReferenceId, string? DisplayText, int SortOrder);
public sealed record AddChoiceFilterCommand(string ChoiceSetCode, ChoiceFilterType Type, EffectValueType ValueType, string? TextValue, decimal? NumericValue, bool? BooleanValue, Guid? ReferenceId, string? EnumValue);

public sealed record EffectSchemaFieldCommand(string Code, EffectValueType ValueType, bool IsRequired);
public sealed record TypedValueFieldCommand(string FieldCode, string? TextValue, decimal? NumericValue, bool? BooleanValue, Guid? ReferenceId, string? EnumValue);
public sealed record FeatureEffectConditionCommand(ConditionType Type, EffectValueType ValueType, string? TextValue, decimal? NumericValue, bool? BooleanValue, Guid? ReferenceId, string? EnumValue);

public sealed record FeatureSummaryDto(Guid Id, Guid RuleSourceId, Guid SourceVersionId, string Code, string Name, string? Description, int? LevelRequirement);
public sealed record FeatureDetailsDto(Guid Id, Guid RuleSourceId, Guid SourceVersionId, string Code, string Name, string? Description, int? LevelRequirement, IReadOnlyCollection<FeatureEffectDto> Effects);
public sealed record EffectSchemaDto(Guid Id, string Code, string Name, EffectType Type, IReadOnlyCollection<EffectSchemaFieldDto> Fields);
public sealed record EffectSchemaFieldDto(Guid Id, string Code, EffectValueType ValueType, bool IsRequired, int SortOrder);
public sealed record FeatureEffectDto(Guid Id, Guid EffectSchemaId, EffectType Type, EffectTarget Target, IReadOnlyCollection<FeatureEffectFieldValueDto> FieldValues, IReadOnlyCollection<FeatureEffectConditionDto> Conditions);
public sealed record FeatureEffectFieldValueDto(Guid Id, Guid EffectSchemaFieldId, TypedMechanicalValueDto Value);
public sealed record FeatureEffectConditionDto(Guid Id, ConditionType Type, TypedMechanicalValueDto Value);
public sealed record EntityPrerequisiteDto(Guid Id, CompendiumEntityKind EntityKind, Guid EntityId, PrerequisiteType Type, ComparisonOperator Operator, EffectTarget Target, TypedMechanicalValueDto Value);
public sealed record ChoiceSetDto(Guid Id, CompendiumEntityKind SourceEntityKind, Guid SourceEntityId, string Code, int MinimumChoices, int MaximumChoices, IReadOnlyCollection<ChoiceSetFilterDto> Filters, IReadOnlyCollection<ChoiceOptionDto> Options);
public sealed record ChoiceSetFilterDto(Guid Id, ChoiceFilterType Type, TypedMechanicalValueDto Value);
public sealed record ChoiceOptionDto(Guid Id, ChoiceOptionType Type, Guid? ReferenceId, string? DisplayText, int SortOrder);
public sealed record TypedMechanicalValueDto(EffectValueType ValueType, string? TextValue, decimal? NumericValue, bool? BooleanValue, Guid? ReferenceId, string? EnumValue);
