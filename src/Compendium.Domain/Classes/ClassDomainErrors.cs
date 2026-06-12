using Compendium.Domain.SharedKernel;

namespace Compendium.Domain.Classes;

public static class ClassDomainErrors
{
    public static DomainError Required(string field) =>
        new($"compendium.classes.{field}.required", $"The {field} is required.");

    public static DomainError TooLong(string field, int maxLength) =>
        new($"compendium.classes.{field}.too-long", $"The {field} must have at most {maxLength} characters.");

    public static DomainError InvalidCode(string field) =>
        new($"compendium.classes.{field}.invalid", $"The {field} must contain only uppercase letters, numbers, dot, underscore or hyphen.");

    public static DomainError InvalidLevel(int level) =>
        new("compendium.classes.level.invalid", $"Class level '{level}' is outside the supported range.");

    public static DomainError DuplicateLevel(int level) =>
        new("compendium.classes.level.duplicate", $"Class level '{level}' is duplicated.");

    public static DomainError LevelsRequired() =>
        new("compendium.classes.levels.required", "At least one class level is required.");

    public static DomainError CoreTraitsRequired() =>
        new("compendium.classes.core-traits.required", "Class core traits are required.");

    public static DomainError PrimaryAbilitiesRequired() =>
        new("compendium.classes.primary-abilities.required", "At least one primary ability is required.");

    public static DomainError DuplicatePrimaryAbility(string abilityId) =>
        new("compendium.classes.primary-ability.duplicate", $"Primary ability '{abilityId}' is duplicated.");

    public static DomainError InvalidSkillChoiceCount() =>
        new("compendium.classes.skill-choice-count.invalid", "Skill choice count cannot be negative.");

    public static DomainError InvalidProficiencyBonus(int level) =>
        new("compendium.classes.proficiency-bonus.invalid", $"Proficiency bonus for level '{level}' must be positive when provided.");

    public static DomainError InvalidSpellSlotLevel(int spellLevel) =>
        new("compendium.classes.spell-slot-level.invalid", $"Spell slot level '{spellLevel}' is invalid.");

    public static DomainError InvalidSpellSlotCount(int spellLevel) =>
        new("compendium.classes.spell-slot-count.invalid", $"Spell slot count for spell level '{spellLevel}' must be positive.");

    public static DomainError DuplicateSpellSlotLevel(int classLevel, int spellLevel) =>
        new("compendium.classes.spell-slot-level.duplicate", $"Spell slot level '{spellLevel}' is duplicated for class level '{classLevel}'.");

    public static DomainError DuplicateProficiencyGrant(int classLevel, string proficiencyId) =>
        new("compendium.classes.proficiency-grant.duplicate", $"Proficiency grant '{proficiencyId}' is duplicated for class level '{classLevel}'.");

    public static DomainError InvalidWeaponMasteryCount(int level) =>
        new("compendium.classes.weapon-mastery-count.invalid", $"Weapon mastery count for level '{level}' cannot be negative.");

    public static DomainError InvalidSpellcastingProgressionType() =>
        new("compendium.classes.spellcasting-progression-type.invalid", "The spellcasting progression type is not supported.");

    public static DomainError InvalidSpellcastingLevelRule(int classLevel) =>
        new("compendium.classes.spellcasting-level-rule.invalid", $"Spellcasting level rule for class level '{classLevel}' is invalid.");

    public static DomainError DuplicateSpellcastingLevelRule(int classLevel) =>
        new("compendium.classes.spellcasting-level-rule.duplicate", $"Spellcasting level rule for class level '{classLevel}' is duplicated.");

    public static DomainError InvalidSubclassFeatureLevel(int level) =>
        new("compendium.classes.subclass-feature-level.invalid", $"Subclass feature level '{level}' is outside the supported range.");

    public static DomainError DuplicateSubclassFeature(string featureId, int level) =>
        new("compendium.classes.subclass-feature.duplicate", $"Subclass feature '{featureId}' is already linked at level '{level}'.");
}
