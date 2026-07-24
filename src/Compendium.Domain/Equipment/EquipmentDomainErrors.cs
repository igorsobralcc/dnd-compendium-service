using Compendium.Domain.SharedKernel;

namespace Compendium.Domain.Equipment;

public static class EquipmentDomainErrors
{
    public static DomainError Required(string field) => new("equipment.required", $"{field} is required.");
    public static DomainError Invalid(string field) => new("equipment.invalid", $"{field} is invalid.");
    public static DomainError TooLong(string field, int max) => new("equipment.too-long", $"{field} must have at most {max} characters.");
    public static DomainError CategoryMismatch(string expected) => new("equipment.category-mismatch", $"Equipment item must have category {expected}.");
    public static DomainError EmptyPack() => new("equipment.pack.empty", "An equipment pack must contain at least one item.");
    public static DomainError EmptyStartingRule() => new("equipment.starting-rule.empty", "A starting equipment rule must contain at least one group.");
    public static DomainError InvalidCardinality() => new("equipment.starting-group.cardinality", "Group cardinality must be positive and cannot exceed its number of options.");
}
