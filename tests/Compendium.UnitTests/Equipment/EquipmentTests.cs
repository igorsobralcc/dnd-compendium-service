using Compendium.Domain.Equipment;
using Compendium.Domain.SharedKernel;

namespace Compendium.UnitTests.Equipment;

public sealed class EquipmentTests
{
    [Fact]
    public void Equipment_item_rejects_negative_weight()
    {
        Assert.True(Weight.Create(-0.1m).IsFailure);
    }

    [Fact]
    public void Weapon_requires_valid_damage_dice()
    {
        var result=Weapon.Create(CompendiumEntityId.New(),WeaponCategory.MartialMelee,"banana",DamageType.Slashing);
        Assert.True(result.IsFailure);
    }

    [Fact]
    public void Pack_requires_at_least_one_item()
    {
        var result=EquipmentPack.Create(CompendiumEntityId.New(),[]);
        Assert.True(result.IsFailure);
    }

    [Fact]
    public void Starting_equipment_enforces_group_cardinality()
    {
        var input=new StartingEquipmentGroupInput(2,[new StartingEquipmentOptionInput(StartingEquipmentOptionType.Item,CompendiumEntityId.New(),1)]);
        var result=StartingEquipmentRule.Create(StartingEquipmentOwnerType.Class,CompendiumEntityId.New(),[input]);
        Assert.True(result.IsFailure);
    }

    [Fact]
    public void Armor_ac_rule_is_structured()
    {
        var armor=Armor.Create(CompendiumEntityId.New(),CompendiumEntityId.New());
        Assert.True(armor.ConfigureAcRule(14,true,2,0).IsSuccess);
        Assert.Equal(2,armor.AcRules.Single().MaximumDexterityBonus);
    }
}
