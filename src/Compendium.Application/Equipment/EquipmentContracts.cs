using Compendium.Domain.Equipment;

namespace Compendium.Application.Equipment;

public sealed record CreateEquipmentItemCommand(Guid RuleSourceId,Guid SourceVersionId,string Code,string Name,EquipmentCategory Category,decimal Weight,decimal CostAmount,Currency CostCurrency,string? Description);
public sealed record UpdateEquipmentItemCommand(string Code,Guid RuleSourceId,Guid SourceVersionId,string Name,EquipmentCategory Category,decimal Weight,decimal CostAmount,Currency CostCurrency,string? Description);
public sealed record EquipmentItemDto(Guid Id,Guid RuleSourceId,Guid SourceVersionId,string Code,string Name,EquipmentCategory Category,decimal Weight,decimal CostAmount,Currency CostCurrency,string? Description);
public sealed record CreateWeaponCommand(Guid EquipmentItemId,WeaponCategory Category,string DamageDice,DamageType DamageType);
public sealed record AttachWeaponPropertyCommand(Guid EquipmentItemId,Guid WeaponPropertyId,IReadOnlyCollection<string> Values);
public sealed record CreateWeaponPropertyCommand(string Code,string Name,WeaponPropertyValueType ValueType,IReadOnlyCollection<WeaponPropertyRuleCommand> Rules);
public sealed record WeaponPropertyRuleCommand(string Field,string Operator,string Value);
public sealed record ConfigureWeaponMasteryCommand(string Code,string Name,IReadOnlyCollection<WeaponMasteryEffectCommand> Effects,IReadOnlyCollection<WeaponMasteryRequirementCommand> Requirements);
public sealed record WeaponMasteryEffectCommand(WeaponMasteryEffectType Type,string Value);
public sealed record WeaponMasteryRequirementCommand(WeaponMasteryRequirementType Type,string Value);
public sealed record WeaponDto(Guid Id,EquipmentItemDto Item,WeaponCategory Category,string DamageDice,DamageType DamageType,IReadOnlyCollection<WeaponPropertyDto> Properties);
public sealed record WeaponPropertyDto(Guid Id,string Code,string Name,WeaponPropertyValueType ValueType,IReadOnlyCollection<string> Values);
public sealed record CreateArmorCommand(Guid EquipmentItemId,Guid ArmorTrainingCategoryId);
public sealed record ConfigureArmorAcRuleCommand(Guid EquipmentItemId,int BaseAc,bool AddsDexterity,int? MaximumDexterityBonus,int Bonus,IReadOnlyCollection<ArmorDrawbackCommand> Drawbacks);
public sealed record ArmorDrawbackCommand(ArmorDrawbackType Type,int? Threshold,string? Description);
public sealed record ArmorDto(Guid Id,EquipmentItemDto Item,Guid ArmorTrainingCategoryId,IReadOnlyCollection<ArmorAcRuleDto> AcRules,IReadOnlyCollection<ArmorDrawbackDto> Drawbacks);
public sealed record ArmorAcRuleDto(int BaseAc,bool AddsDexterity,int? MaximumDexterityBonus,int Bonus);
public sealed record ArmorDrawbackDto(ArmorDrawbackType Type,int? Threshold,string? Description);
public sealed record CreateToolCommand(Guid EquipmentItemId,Guid? ProficiencyId,string? AbilityCode);
public sealed record CreateEquipmentPackCommand(Guid EquipmentItemId,IReadOnlyCollection<EquipmentPackItemCommand> Items);
public sealed record EquipmentPackItemCommand(Guid EquipmentItemId,int Quantity);
public sealed record CreateStartingEquipmentRuleCommand(StartingEquipmentOwnerType OwnerType,Guid OwnerEntityId,IReadOnlyCollection<StartingEquipmentGroupCommand> Groups);
public sealed record StartingEquipmentGroupCommand(int SelectionCount,IReadOnlyCollection<StartingEquipmentOptionCommand> Options);
public sealed record StartingEquipmentOptionCommand(StartingEquipmentOptionType Type,Guid ReferenceId,int Quantity);
public sealed record StartingEquipmentRuleDto(Guid Id,StartingEquipmentOwnerType OwnerType,Guid OwnerEntityId,IReadOnlyCollection<StartingEquipmentGroupDto> Groups);
public sealed record StartingEquipmentGroupDto(int Ordinal,int SelectionCount,IReadOnlyCollection<StartingEquipmentOptionDto> Options);
public sealed record StartingEquipmentOptionDto(int Ordinal,StartingEquipmentOptionType Type,Guid ReferenceId,int Quantity);

internal static class EquipmentMappings
{
    public static EquipmentItemDto ToDto(this EquipmentItem x)=>new(x.Id.Value,x.RuleSourceId.Value,x.SourceVersionId.Value,x.Code.Value,x.Name.Value,x.Category,x.Weight,x.CostAmount,x.CostCurrency,x.Description);
    public static StartingEquipmentRuleDto ToDto(this StartingEquipmentRule x)=>new(x.Id.Value,x.OwnerType,x.OwnerEntityId.Value,x.Groups.OrderBy(g=>g.Ordinal).Select(g=>new StartingEquipmentGroupDto(g.Ordinal,g.SelectionCount,g.Options.OrderBy(o=>o.Ordinal).Select(o=>new StartingEquipmentOptionDto(o.Ordinal,o.Type,o.ReferenceId.Value,o.Quantity)).ToArray())).ToArray());
}
