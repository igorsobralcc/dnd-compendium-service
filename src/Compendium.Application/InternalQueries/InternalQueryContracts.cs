namespace Compendium.Application.InternalQueries;

public sealed record CharacterCreationOptionsRequest(
    Guid RulesetId,
    Guid SourceVersionId,
    string Locale,
    int? Level);

public sealed record CharacterCreationOptionsV1(
    string ApiVersion,
    Guid RulesetId,
    Guid SourceVersionId,
    string Locale,
    int? Level,
    IReadOnlyCollection<OptionV1> Classes,
    IReadOnlyCollection<OptionV1> Species,
    IReadOnlyCollection<OptionV1> Backgrounds,
    IReadOnlyCollection<OptionV1> AbilityScoreMethods,
    IReadOnlyCollection<OptionV1> Proficiencies,
    IReadOnlyCollection<OptionV1> Languages,
    IReadOnlyCollection<EquipmentOptionV1> StartingEquipment,
    IReadOnlyCollection<OptionV1> SpellLists);

public sealed record OptionV1(Guid Id, string Code, string Name);

public sealed record EquipmentOptionV1(
    Guid Id,
    string Code,
    string Name,
    string Type,
    decimal? CostAmount,
    string? CostCurrency);

public sealed record MechanicalEntityDetailsV1(
    string ApiVersion,
    string EntityType,
    Guid EntityId,
    string Code,
    string Name,
    string? Description,
    SourceReferenceV1 Source,
    ClassMechanicsV1? Class,
    FeatureMechanicsV1? Feature,
    EquipmentMechanicsV1? Equipment,
    ChoiceSetMechanicsV1? ChoiceSet,
    IReadOnlyCollection<PrerequisiteV1> Prerequisites,
    IReadOnlyCollection<RelatedReferenceV1> RelatedReferences);

public sealed record SourceReferenceV1(Guid RuleSourceId, Guid SourceVersionId);

public sealed record ClassMechanicsV1(
    IReadOnlyCollection<ClassLevelMechanicsV1> Levels,
    IReadOnlyCollection<Guid> PrimaryAbilityIds);

public sealed record ClassLevelMechanicsV1(
    int Level,
    int? ProficiencyBonus,
    IReadOnlyCollection<SpellSlotV1> SpellSlots,
    IReadOnlyCollection<Guid> ProficiencyIds);

public sealed record SpellSlotV1(int SpellLevel, int Slots);

public sealed record FeatureMechanicsV1(
    int? LevelRequirement,
    IReadOnlyCollection<EffectV1> Effects);

public sealed record EffectV1(
    Guid Id,
    string Type,
    string Target,
    IReadOnlyCollection<TypedFieldV1> Fields,
    IReadOnlyCollection<TypedConditionV1> Conditions);

public sealed record TypedFieldV1(Guid SchemaFieldId, TypedValueV1 Value);
public sealed record TypedConditionV1(string Type, TypedValueV1 Value);
public sealed record TypedValueV1(
    string ValueType,
    string? Text,
    decimal? Number,
    bool? Boolean,
    Guid? ReferenceId,
    string? Enum);

public sealed record EquipmentMechanicsV1(
    string Type,
    decimal? CostAmount,
    string? CostCurrency,
    decimal? Weight,
    bool IsActive);

public sealed record ChoiceSetMechanicsV1(
    string SourceEntityType,
    Guid SourceEntityId,
    int MinimumChoices,
    int MaximumChoices,
    IReadOnlyCollection<ChoiceOptionV1> Options,
    IReadOnlyCollection<ChoiceFilterV1> Filters);

public sealed record ChoiceOptionV1(Guid Id, string Type, Guid? ReferenceId, string? DisplayText, int SortOrder);
public sealed record ChoiceFilterV1(Guid Id, string Type, TypedValueV1 Value);
public sealed record PrerequisiteV1(Guid Id, string Type, string Operator, string Target, TypedValueV1 Value);
public sealed record RelatedReferenceV1(string EntityType, Guid EntityId);

public sealed record CompendiumChangesRequest(
    Guid? SourceVersionId,
    string? EntityType,
    DateTimeOffset? ChangedSince,
    long? Revision,
    int Page,
    int PageSize);

public sealed record CompendiumChangesV1(
    string ApiVersion,
    IReadOnlyCollection<CompendiumChangeV1> Items,
    int Page,
    int PageSize,
    long TotalCount,
    long? NextRevision);

public sealed record CompendiumChangeV1(
    long Revision,
    Guid? SourceVersionId,
    string EntityType,
    Guid EntityId,
    string ChangeType,
    DateTimeOffset ChangedAtUtc);

