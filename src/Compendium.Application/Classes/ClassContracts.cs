using Compendium.Domain.Classes;

namespace Compendium.Application.Classes;

public sealed record CreateClassCommand(
    Guid RuleSourceId,
    Guid SourceVersionId,
    string Code,
    string Name,
    string? Description,
    ClassCoreTraitsCommand CoreTraits,
    IReadOnlyCollection<Guid> PrimaryAbilityIds,
    IReadOnlyCollection<ClassLevelCommand> Levels);

public sealed record UpdateClassCommand(
    string Code,
    Guid RuleSourceId,
    Guid SourceVersionId,
    string Name,
    string? Description,
    ClassCoreTraitsCommand CoreTraits,
    IReadOnlyCollection<Guid> PrimaryAbilityIds);

public sealed record ConfigureClassProgressionCommand(
    string Code,
    Guid RuleSourceId,
    Guid SourceVersionId,
    IReadOnlyCollection<ClassLevelCommand> Levels,
    ClassSpellcastingProgressionCommand? SpellcastingProgression);

public sealed record CreateSubclassCommand(
    string ClassCode,
    Guid RuleSourceId,
    Guid SourceVersionId,
    string Code,
    string Name,
    string? Description);

public sealed record LinkSubclassFeatureCommand(
    string ClassCode,
    string SubclassCode,
    Guid RuleSourceId,
    Guid SourceVersionId,
    Guid FeatureId,
    int Level);

public sealed record ClassCoreTraitsCommand(Guid HitDieId, Guid? ArmorTrainingCategoryId, int SkillChoiceCount);

public sealed record ClassLevelCommand(
    int Level,
    int? ProficiencyBonus,
    IReadOnlyCollection<ClassLevelSpellSlotCommand> SpellSlots,
    IReadOnlyCollection<Guid> ProficiencyGrantIds,
    int? WeaponMasteryCount);

public sealed record ClassLevelSpellSlotCommand(int SpellLevel, int Slots);

public sealed record ClassSpellcastingProgressionCommand(
    ClassSpellcastingProgressionType Type,
    Guid? SpellcastingAbilityId,
    IReadOnlyCollection<ClassSpellcastingLevelRuleCommand> LevelRules);

public sealed record ClassSpellcastingLevelRuleCommand(int ClassLevel, int CasterLevel);

public sealed record ClassSummaryDto(
    Guid Id,
    Guid RuleSourceId,
    Guid SourceVersionId,
    string Code,
    string Name,
    string? Description);

public sealed record ClassDetailsDto(
    Guid Id,
    Guid RuleSourceId,
    Guid SourceVersionId,
    string Code,
    string Name,
    string? Description,
    ClassCoreTraitsDto CoreTraits,
    IReadOnlyCollection<ClassPrimaryAbilityDto> PrimaryAbilities,
    IReadOnlyCollection<ClassLevelDto> Levels,
    ClassSpellcastingProgressionDto? SpellcastingProgression);

public sealed record ClassCoreTraitsDto(Guid Id, Guid HitDieId, Guid? ArmorTrainingCategoryId, int SkillChoiceCount);

public sealed record ClassPrimaryAbilityDto(Guid Id, Guid AbilityId, int SortOrder);

public sealed record ClassLevelDto(
    Guid Id,
    int Level,
    int? ProficiencyBonus,
    IReadOnlyCollection<ClassLevelSpellSlotDto> SpellSlots,
    IReadOnlyCollection<ClassProficiencyGrantDto> ProficiencyGrants,
    int? WeaponMasteryCount);

public sealed record ClassProgressionDto(
    IReadOnlyCollection<ClassLevelDto> Levels,
    ClassSpellcastingProgressionDto? SpellcastingProgression);

public sealed record ClassLevelSpellSlotDto(Guid Id, int SpellLevel, int Slots);

public sealed record ClassProficiencyGrantDto(Guid Id, Guid ProficiencyId);

public sealed record ClassSpellcastingProgressionDto(
    Guid Id,
    ClassSpellcastingProgressionType Type,
    Guid? SpellcastingAbilityId,
    IReadOnlyCollection<ClassSpellcastingLevelRuleDto> LevelRules);

public sealed record ClassSpellcastingLevelRuleDto(Guid Id, int ClassLevel, int CasterLevel);

public sealed record SubclassSummaryDto(
    Guid Id,
    Guid CharacterClassId,
    Guid RuleSourceId,
    Guid SourceVersionId,
    string Code,
    string Name,
    string? Description);

public sealed record SubclassDetailsDto(
    Guid Id,
    Guid CharacterClassId,
    Guid RuleSourceId,
    Guid SourceVersionId,
    string Code,
    string Name,
    string? Description,
    IReadOnlyCollection<SubclassFeatureDto> Features);

public sealed record SubclassFeatureDto(Guid Id, Guid SourceVersionId, Guid FeatureId, int Level);
