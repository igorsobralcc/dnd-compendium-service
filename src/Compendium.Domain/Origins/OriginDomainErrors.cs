using Compendium.Domain.SharedKernel;

namespace Compendium.Domain.Origins;

public static class OriginDomainErrors
{
    public static DomainError Required(string field) =>
        new($"compendium.origins.{field}.required", $"'{field}' is required.", DomainErrorKind.Validation);

    public static DomainError TooLong(string field, int maxLength) =>
        new($"compendium.origins.{field}.too-long", $"'{field}' cannot exceed {maxLength} characters.", DomainErrorKind.Validation);

    public static DomainError InvalidCode(string field) =>
        new($"compendium.origins.{field}.invalid", $"'{field}' must contain only uppercase letters, numbers, dots, underscores, or hyphens.", DomainErrorKind.Validation);

    public static DomainError InvalidEnum(string field) =>
        new($"compendium.origins.{field}.invalid", $"'{field}' has an unsupported value.", DomainErrorKind.Validation);

    public static DomainError DuplicateFeature(string featureId) =>
        new("compendium.origins.feature.duplicate", $"Feature '{featureId}' is already linked.", DomainErrorKind.Conflict);

    public static DomainError DuplicateReference(string field, string referenceId) =>
        new($"compendium.origins.{field}.duplicate", $"Reference '{referenceId}' is duplicated in '{field}'.", DomainErrorKind.Conflict);

    public static DomainError AbilityOptionsRequired() =>
        new("compendium.origins.background.ability-options.required", "A background must provide exactly three distinct ability options.", DomainErrorKind.Validation);

    public static DomainError InvalidAbilityBoostRules() =>
        new("compendium.origins.background.ability-boost-rules.invalid", "Ability boosts must be either one +2 and one +1, or three +1 boosts.", DomainErrorKind.Validation);

    public static DomainError FeatGrantRequired() =>
        new("compendium.origins.background.feat-grant.required", "A background must grant an origin feat.", DomainErrorKind.Validation);

    public static DomainError SkillProficienciesRequired() =>
        new("compendium.origins.background.skill-proficiencies.required", "A background must grant two distinct skill proficiencies.", DomainErrorKind.Validation);

    public static DomainError ToolProficiencyRequired() =>
        new("compendium.origins.background.tool-proficiency.required", "A background must grant one tool proficiency.", DomainErrorKind.Validation);

    public static DomainError StartingEquipmentRequired() =>
        new("compendium.origins.background.starting-equipment.required", "A background must reference at least one starting-equipment rule or group.", DomainErrorKind.Validation);
}
