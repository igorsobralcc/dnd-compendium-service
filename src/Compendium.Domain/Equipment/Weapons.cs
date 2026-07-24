using Compendium.Domain.SharedKernel;

namespace Compendium.Domain.Equipment;

public sealed class Weapon
{
    private readonly List<WeaponPropertyLink> propertyLinks = [];
    private Weapon() { EquipmentItemId = null!; DamageDice = null!; }
    private Weapon(CompendiumEntityId id, CompendiumEntityId itemId, WeaponCategory category, string dice, DamageType damageType)
    { Id = id; EquipmentItemId = itemId; Category = category; DamageDice = dice; DamageType = damageType; }
    public CompendiumEntityId Id { get; private set; } = null!;
    public CompendiumEntityId EquipmentItemId { get; private set; }
    public WeaponCategory Category { get; private set; }
    public string DamageDice { get; private set; }
    public DamageType DamageType { get; private set; }
    public IReadOnlyCollection<WeaponPropertyLink> PropertyLinks => propertyLinks;
    public static Result<Weapon> Create(CompendiumEntityId itemId, WeaponCategory category, string damageDice, DamageType damageType)
    {
        if (!Enum.IsDefined(category) || !Enum.IsDefined(damageType) || !DiceNotation.IsValid(damageDice))
            return Result<Weapon>.Failure(EquipmentDomainErrors.Invalid("weapon"));
        return Result<Weapon>.Success(new Weapon(CompendiumEntityId.New(), itemId, category, damageDice.Trim().ToUpperInvariant(), damageType));
    }
    public Result AttachProperty(CompendiumEntityId propertyId, IReadOnlyCollection<string> values)
    {
        if (propertyLinks.Any(x => x.WeaponPropertyId == propertyId)) return Result.Failure(EquipmentDomainErrors.Invalid("duplicate-property"));
        propertyLinks.Add(WeaponPropertyLink.Create(Id, propertyId, values)); return Result.Success();
    }
}

public sealed class WeaponProperty
{
    private readonly List<WeaponPropertyRule> rules = [];
    private WeaponProperty() { Code = Name = null!; }
    private WeaponProperty(CompendiumEntityId id, string code, string name, WeaponPropertyValueType valueType)
    { Id = id; Code = code; Name = name; ValueType = valueType; }
    public CompendiumEntityId Id { get; private set; } = null!;
    public string Code { get; private set; }
    public string Name { get; private set; }
    public WeaponPropertyValueType ValueType { get; private set; }
    public IReadOnlyCollection<WeaponPropertyRule> Rules => rules;
    public static Result<WeaponProperty> Create(string code, string name, WeaponPropertyValueType valueType, IReadOnlyCollection<(string Field, string Operator, string Value)> inputs)
    {
        if (string.IsNullOrWhiteSpace(code) || string.IsNullOrWhiteSpace(name) || !Enum.IsDefined(valueType))
            return Result<WeaponProperty>.Failure(EquipmentDomainErrors.Invalid("weapon-property"));
        var result = new WeaponProperty(CompendiumEntityId.New(), code.Trim().ToUpperInvariant(), name.Trim(), valueType);
        foreach (var i in inputs) result.rules.Add(WeaponPropertyRule.Create(result.Id, i.Field, i.Operator, i.Value));
        return Result<WeaponProperty>.Success(result);
    }
}

public sealed class WeaponPropertyRule
{
    private WeaponPropertyRule() { WeaponPropertyId = null!; Field = Operator = Value = null!; }
    private WeaponPropertyRule(CompendiumEntityId id, CompendiumEntityId propertyId, string field, string op, string value)
    { Id = id; WeaponPropertyId = propertyId; Field = field; Operator = op; Value = value; }
    public CompendiumEntityId Id { get; private set; } = null!;
    public CompendiumEntityId WeaponPropertyId { get; private set; }
    public string Field { get; private set; }
    public string Operator { get; private set; }
    public string Value { get; private set; }
    internal static WeaponPropertyRule Create(CompendiumEntityId propertyId, string field, string op, string value) =>
        new(CompendiumEntityId.New(), propertyId, field.Trim(), op.Trim(), value.Trim());
}

public sealed class WeaponPropertyLink
{
    private readonly List<WeaponPropertyLinkValue> values = [];
    private WeaponPropertyLink() { WeaponId = WeaponPropertyId = null!; }
    private WeaponPropertyLink(CompendiumEntityId id, CompendiumEntityId weaponId, CompendiumEntityId propertyId)
    { Id = id; WeaponId = weaponId; WeaponPropertyId = propertyId; }
    public CompendiumEntityId Id { get; private set; } = null!;
    public CompendiumEntityId WeaponId { get; private set; }
    public CompendiumEntityId WeaponPropertyId { get; private set; }
    public IReadOnlyCollection<WeaponPropertyLinkValue> Values => values;
    internal static WeaponPropertyLink Create(CompendiumEntityId weaponId, CompendiumEntityId propertyId, IReadOnlyCollection<string> inputs)
    {
        var link = new WeaponPropertyLink(CompendiumEntityId.New(), weaponId, propertyId);
        var ordinal = 0; foreach (var value in inputs) link.values.Add(WeaponPropertyLinkValue.Create(link.Id, ordinal++, value));
        return link;
    }
}

public sealed class WeaponPropertyLinkValue
{
    private WeaponPropertyLinkValue() { WeaponPropertyLinkId = null!; Value = null!; }
    private WeaponPropertyLinkValue(CompendiumEntityId id, CompendiumEntityId linkId, int ordinal, string value)
    { Id = id; WeaponPropertyLinkId = linkId; Ordinal = ordinal; Value = value; }
    public CompendiumEntityId Id { get; private set; } = null!;
    public CompendiumEntityId WeaponPropertyLinkId { get; private set; }
    public int Ordinal { get; private set; }
    public string Value { get; private set; }
    internal static WeaponPropertyLinkValue Create(CompendiumEntityId linkId, int ordinal, string value) =>
        new(CompendiumEntityId.New(), linkId, ordinal, value.Trim());
}

public sealed class WeaponMasteryProperty
{
    private readonly List<WeaponMasteryEffect> effects = [];
    private readonly List<WeaponMasteryRequirement> requirements = [];
    private WeaponMasteryProperty() { Code = Name = null!; }
    private WeaponMasteryProperty(CompendiumEntityId id, string code, string name) { Id = id; Code = code; Name = name; }
    public CompendiumEntityId Id { get; private set; } = null!;
    public string Code { get; private set; }
    public string Name { get; private set; }
    public IReadOnlyCollection<WeaponMasteryEffect> Effects => effects;
    public IReadOnlyCollection<WeaponMasteryRequirement> Requirements => requirements;
    public static Result<WeaponMasteryProperty> Create(string code, string name,
        IReadOnlyCollection<(WeaponMasteryEffectType Type, string Value)> effectInputs,
        IReadOnlyCollection<(WeaponMasteryRequirementType Type, string Value)> requirementInputs)
    {
        if (string.IsNullOrWhiteSpace(code) || string.IsNullOrWhiteSpace(name) || effectInputs.Count == 0)
            return Result<WeaponMasteryProperty>.Failure(EquipmentDomainErrors.Invalid("weapon-mastery"));
        var mastery = new WeaponMasteryProperty(CompendiumEntityId.New(), code.Trim().ToUpperInvariant(), name.Trim());
        foreach (var x in effectInputs) { if (!Enum.IsDefined(x.Type)) return Result<WeaponMasteryProperty>.Failure(EquipmentDomainErrors.Invalid("mastery-effect")); mastery.effects.Add(WeaponMasteryEffect.Create(mastery.Id, x.Type, x.Value)); }
        foreach (var x in requirementInputs) { if (!Enum.IsDefined(x.Type)) return Result<WeaponMasteryProperty>.Failure(EquipmentDomainErrors.Invalid("mastery-requirement")); mastery.requirements.Add(WeaponMasteryRequirement.Create(mastery.Id, x.Type, x.Value)); }
        return Result<WeaponMasteryProperty>.Success(mastery);
    }
}

public sealed class WeaponMasteryEffect
{
    private WeaponMasteryEffect() { WeaponMasteryPropertyId = null!; Value = null!; }
    private WeaponMasteryEffect(CompendiumEntityId id, CompendiumEntityId masteryId, WeaponMasteryEffectType type, string value) { Id=id; WeaponMasteryPropertyId=masteryId; Type=type; Value=value; }
    public CompendiumEntityId Id { get; private set; }=null!; public CompendiumEntityId WeaponMasteryPropertyId { get; private set; } public WeaponMasteryEffectType Type { get; private set; } public string Value { get; private set; }
    internal static WeaponMasteryEffect Create(CompendiumEntityId id, WeaponMasteryEffectType type, string value)=>new(CompendiumEntityId.New(),id,type,value.Trim());
}
public sealed class WeaponMasteryRequirement
{
    private WeaponMasteryRequirement() { WeaponMasteryPropertyId=null!; Value=null!; }
    private WeaponMasteryRequirement(CompendiumEntityId id, CompendiumEntityId masteryId, WeaponMasteryRequirementType type,string value){Id=id;WeaponMasteryPropertyId=masteryId;Type=type;Value=value;}
    public CompendiumEntityId Id{get;private set;}=null!; public CompendiumEntityId WeaponMasteryPropertyId{get;private set;} public WeaponMasteryRequirementType Type{get;private set;} public string Value{get;private set;}
    internal static WeaponMasteryRequirement Create(CompendiumEntityId id,WeaponMasteryRequirementType type,string value)=>new(CompendiumEntityId.New(),id,type,value.Trim());
}
