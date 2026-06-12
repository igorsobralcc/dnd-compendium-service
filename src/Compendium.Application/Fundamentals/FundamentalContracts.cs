using Compendium.Domain.Fundamentals;

namespace Compendium.Application.Fundamentals;

public sealed record CreateAbilityCommand(Guid RuleSourceId, Guid SourceVersionId, string Code, string Name);

public sealed record UpdateAbilityCommand(string Code, Guid RuleSourceId, Guid SourceVersionId, string Name);

public sealed record AbilityDto(Guid Id, Guid RuleSourceId, Guid SourceVersionId, string Code, string Name);

public sealed record CreateSkillCommand(Guid RuleSourceId, Guid SourceVersionId, string Code, string Name, Guid? DefaultAbilityId);

public sealed record UpdateSkillCommand(string Code, Guid RuleSourceId, Guid SourceVersionId, string Name, Guid? DefaultAbilityId);

public sealed record SkillDto(Guid Id, Guid RuleSourceId, Guid SourceVersionId, string Code, string Name, Guid? DefaultAbilityId);

public sealed record CreateLanguageCommand(Guid RuleSourceId, Guid SourceVersionId, string Code, string Name);

public sealed record UpdateLanguageCommand(string Code, Guid RuleSourceId, Guid SourceVersionId, string Name);

public sealed record LanguageDto(Guid Id, Guid RuleSourceId, Guid SourceVersionId, string Code, string Name);

public sealed record CreateProficiencyCommand(
    Guid RuleSourceId,
    Guid SourceVersionId,
    string Code,
    string Name,
    ProficiencyType Type,
    Guid? RelatedEntityId);

public sealed record UpdateProficiencyCommand(
    string Code,
    Guid RuleSourceId,
    Guid SourceVersionId,
    string Name,
    ProficiencyType Type,
    Guid? RelatedEntityId);

public sealed record ProficiencyDto(
    Guid Id,
    Guid RuleSourceId,
    Guid SourceVersionId,
    string Code,
    string Name,
    ProficiencyType Type,
    Guid? RelatedEntityId);

public sealed record CreateArmorTrainingCategoryCommand(
    Guid RuleSourceId,
    Guid SourceVersionId,
    string Code,
    string Name,
    int SortOrder);

public sealed record ArmorTrainingCategoryDto(
    Guid Id,
    Guid RuleSourceId,
    Guid SourceVersionId,
    string Code,
    string Name,
    int SortOrder);

public sealed record CreateHitDieCommand(Guid RuleSourceId, Guid SourceVersionId, int Die);

public sealed record HitDieDto(Guid Id, Guid RuleSourceId, Guid SourceVersionId, string Code, string Name, int Die);

public sealed record CreateAbilityScoreMethodCommand(
    Guid RuleSourceId,
    Guid SourceVersionId,
    string Code,
    string Name,
    AbilityScoreMethodType Type,
    IReadOnlyCollection<CreateAbilityScoreMethodRuleCommand> Rules,
    IReadOnlyCollection<int> StandardValues,
    IReadOnlyCollection<CreateAbilityScorePointBuyCostCommand> PointBuyCosts,
    CreateAbilityScoreRollRuleCommand? RollRule);

public sealed record CreateAbilityScoreMethodRuleCommand(string Code, int? NumericValue, string? TextValue);

public sealed record CreateAbilityScorePointBuyCostCommand(int Score, int Cost);

public sealed record CreateAbilityScoreRollRuleCommand(
    int DiceQuantity,
    int DieSize,
    int KeepHighestQuantity,
    int? DropLowestQuantity,
    int Repetitions);

public sealed record AbilityScoreMethodDto(
    Guid Id,
    Guid RuleSourceId,
    Guid SourceVersionId,
    string Code,
    string Name,
    AbilityScoreMethodType Type,
    IReadOnlyCollection<AbilityScoreMethodRuleDto> Rules,
    IReadOnlyCollection<AbilityScoreStandardValueDto> StandardValues,
    IReadOnlyCollection<AbilityScorePointBuyCostDto> PointBuyCosts,
    AbilityScoreRollRuleDto? RollRule);

public sealed record AbilityScoreMethodRuleDto(Guid Id, string Code, int? NumericValue, string? TextValue);

public sealed record AbilityScoreStandardValueDto(Guid Id, int Position, int Score);

public sealed record AbilityScorePointBuyCostDto(Guid Id, int Score, int Cost);

public sealed record AbilityScoreRollRuleDto(
    Guid Id,
    int DiceQuantity,
    int DieSize,
    int KeepHighestQuantity,
    int? DropLowestQuantity,
    int Repetitions);
