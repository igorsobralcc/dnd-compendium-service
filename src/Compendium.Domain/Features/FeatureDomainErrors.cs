using Compendium.Domain.SharedKernel;

namespace Compendium.Domain.Features;

public static class FeatureDomainErrors
{
    public static DomainError Required(string field) =>
        new($"compendium.features.{field}.required", $"The {field} is required.");

    public static DomainError TooLong(string field, int maxLength) =>
        new($"compendium.features.{field}.too-long", $"The {field} must have at most {maxLength} characters.");

    public static DomainError InvalidCode(string field) =>
        new($"compendium.features.{field}.invalid", $"The {field} must contain only uppercase letters, numbers, dot, underscore or hyphen.");

    public static DomainError InvalidEnum(string field) =>
        new($"compendium.features.{field}.invalid", $"The {field} value is not supported.");

    public static DomainError InvalidLevelRequirement() =>
        new("compendium.features.level-requirement.invalid", "Feature level requirement cannot be negative.");

    public static DomainError MissingRequiredEffectField(string code) =>
        new("compendium.features.effect-field.required", $"Required effect field '{code}' is missing.");

    public static DomainError UnknownEffectField(string code) =>
        new("compendium.features.effect-field.unknown", $"Effect field '{code}' is not defined in the schema.");

    public static DomainError DuplicateEffectField(string code) =>
        new("compendium.features.effect-field.duplicate", $"Effect field '{code}' is duplicated.");

    public static DomainError EffectFieldTypeMismatch(string code) =>
        new("compendium.features.effect-field.type-mismatch", $"Effect field '{code}' does not match the schema value type.");

    public static DomainError TypedValueRequired(string owner) =>
        new($"compendium.features.{owner}.typed-value.required", $"A typed value is required for {owner}.");

    public static DomainError ChoiceCardinalityInvalid() =>
        new("compendium.features.choice-cardinality.invalid", "Choice minimum quantity cannot be greater than maximum quantity.");
}
