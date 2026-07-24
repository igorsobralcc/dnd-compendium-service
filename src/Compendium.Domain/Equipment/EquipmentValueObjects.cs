using System.Text.RegularExpressions;
using Compendium.Domain.SharedKernel;

namespace Compendium.Domain.Equipment;

public sealed record EquipmentCode
{
    public const int MaxLength = 80;
    private EquipmentCode(string value) => Value = value;
    public string Value { get; }
    public static Result<EquipmentCode> Create(string? value)
    {
        var normalized = value?.Trim().ToUpperInvariant();
        if (string.IsNullOrWhiteSpace(normalized)) return Result<EquipmentCode>.Failure(EquipmentDomainErrors.Required("equipment-code"));
        if (normalized.Length > MaxLength) return Result<EquipmentCode>.Failure(EquipmentDomainErrors.TooLong("equipment-code", MaxLength));
        return Regex.IsMatch(normalized, "^[A-Z0-9._-]+$")
            ? Result<EquipmentCode>.Success(new EquipmentCode(normalized))
            : Result<EquipmentCode>.Failure(EquipmentDomainErrors.Invalid("equipment-code"));
    }
    public override string ToString() => Value;
}

public sealed record EquipmentName
{
    public const int MaxLength = 180;
    private EquipmentName(string value) => Value = value;
    public string Value { get; }
    public static Result<EquipmentName> Create(string? value)
    {
        var normalized = value?.Trim();
        if (string.IsNullOrWhiteSpace(normalized)) return Result<EquipmentName>.Failure(EquipmentDomainErrors.Required("equipment-name"));
        return normalized.Length <= MaxLength
            ? Result<EquipmentName>.Success(new EquipmentName(normalized))
            : Result<EquipmentName>.Failure(EquipmentDomainErrors.TooLong("equipment-name", MaxLength));
    }
    public override string ToString() => Value;
}

public readonly record struct Weight(decimal Pounds)
{
    public static Result<Weight> Create(decimal pounds) => pounds >= 0
        ? Result<Weight>.Success(new Weight(decimal.Round(pounds, 3)))
        : Result<Weight>.Failure(EquipmentDomainErrors.Invalid("weight"));
}

public readonly record struct Cost(decimal Amount, Currency Currency)
{
    public static Result<Cost> Create(decimal amount, Currency currency) =>
        amount >= 0 && Enum.IsDefined(currency)
            ? Result<Cost>.Success(new Cost(decimal.Round(amount, 2), currency))
            : Result<Cost>.Failure(EquipmentDomainErrors.Invalid("cost"));
}

public enum Currency { Copper = 1, Silver = 2, Electrum = 3, Gold = 4, Platinum = 5 }
public enum EquipmentCategory { Weapon = 1, Armor = 2, Tool = 3, Pack = 4, AdventuringGear = 5, Other = 6 }
public enum WeaponCategory { SimpleMelee = 1, SimpleRanged = 2, MartialMelee = 3, MartialRanged = 4 }
public enum DamageType { Bludgeoning = 1, Piercing = 2, Slashing = 3 }
public enum WeaponPropertyValueType { None = 1, Integer = 2, Decimal = 3, Text = 4, Dice = 5, Distance = 6 }
public enum WeaponMasteryEffectType { AttackModifier = 1, DamageModifier = 2, Movement = 3, Condition = 4, ExtraAttack = 5, Other = 6 }
public enum WeaponMasteryRequirementType { WeaponCategory = 1, DamageType = 2, Property = 3, Other = 4 }
public enum ArmorDrawbackType { StrengthRequirement = 1, StealthDisadvantage = 2, SpeedPenalty = 3, Other = 4 }
public enum StartingEquipmentOwnerType { Class = 1, Background = 2, Species = 3, Other = 4 }
public enum StartingEquipmentOptionType { Item = 1, Pack = 2 }

public static class DiceNotation
{
    public static bool IsValid(string? value) => value is not null && Regex.IsMatch(value.Trim().ToUpperInvariant(), @"^\d+D(4|6|8|10|12|20|100)([+-]\d+)?$");
}
