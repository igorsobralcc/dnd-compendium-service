using Compendium.Domain.Equipment;

using Compendium.Domain.SharedKernel;

using Microsoft.EntityFrameworkCore;

using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Microsoft.EntityFrameworkCore.Storage.ValueConversion;


namespace Compendium.Infra.Persistence.Equipment;


internal static class EquipmentEf
{
    public static readonly ValueConverter<CompendiumEntityId, Guid> Id = new(x => x.Value, x => CompendiumEntityId.Create(x).Value);

    public static readonly ValueConverter<CompendiumEntityId?, Guid?> NullableId = new(x => x == null ? null : x.Value, x => x == null ? null : CompendiumEntityId.Create(x.Value).Value);

    public static readonly ValueConverter<EquipmentCode, string> Code = new(x => x.Value, x => EquipmentCode.Create(x).Value);

    public static readonly ValueConverter<EquipmentName, string> Name = new(x => x.Value, x => EquipmentName.Create(x).Value);

    public static void Key<T>(EntityTypeBuilder<T> b, string table) where T : class
    {
        b.ToTable(table, CompendiumDbContext.Schema);
        b
.HasKey("Id")
.HasName($"pk_{table}");
        b.Property("Id")
.HasConversion(Id)
.ValueGeneratedNever()
.HasColumnName("id");

    }
    public static PropertyBuilder<CompendiumEntityId> IdProperty<T>(this EntityTypeBuilder<T> b, System.Linq.Expressions.Expression<Func<T, CompendiumEntityId>> p, string name) where T : class =>
        b.Property(p)
.HasConversion(Id)
.HasColumnName(name)
.IsRequired();

}
internal sealed class EquipmentItemConfiguration : IEntityTypeConfiguration<EquipmentItem>
{
    public void Configure(EntityTypeBuilder<EquipmentItem> b)
    {
        EquipmentEf.Key(b, "equipment_items");
        b.IdProperty(x => x.RuleSourceId, "rule_source_id");
        b.IdProperty(x => x.SourceVersionId, "source_version_id");
        b.Property(x => x.Code)
.HasConversion(EquipmentEf.Code)
.HasMaxLength(80)
.HasColumnName("code");
        b.Property(x => x.Name)
.HasConversion(EquipmentEf.Name)
.HasMaxLength(180)
.HasColumnName("name");
        b.Property(x => x.Category)
.HasColumnName("category");
        b.Property(x => x.Weight)
.HasPrecision(10, 3)
.HasColumnName("weight");
        b.Property(x => x.CostAmount)
.HasPrecision(12, 2)
.HasColumnName("cost_amount");
        b.Property(x => x.CostCurrency)
.HasColumnName("cost_currency");
        b.Property(x => x.Description)
.HasMaxLength(4000)
.HasColumnName("description");
        b.Property(x => x.CreatedAtUtc)
.HasColumnName("created_at_utc");
        b.Property(x => x.UpdatedAtUtc)
.HasColumnName("updated_at_utc");
        b
.HasIndex(x => x.Code)
.IsUnique()
.HasDatabaseName("ux_equipment_items_code");
        b
.HasIndex(x => x.Category)
.HasDatabaseName("ix_equipment_items_category");
    }
}
internal sealed class WeaponConfiguration : IEntityTypeConfiguration<Weapon>
{
    public void Configure(EntityTypeBuilder<Weapon> b)
    {
        EquipmentEf.Key(b, "weapons");
        b.IdProperty(x => x.EquipmentItemId, "equipment_item_id");
        b.Property(x => x.Category)
.HasColumnName("category");
        b.Property(x => x.DamageDice)
.HasMaxLength(20)
.HasColumnName("damage_dice");
        b.Property(x => x.DamageType)
.HasColumnName("damage_type");
        b
.HasIndex(x => x.EquipmentItemId)
.IsUnique();
        b
.HasMany(x => x.PropertyLinks)
.WithOne()
.HasForeignKey(x => x.WeaponId)
.OnDelete(DeleteBehavior.Cascade);
    }
}
internal sealed class WeaponPropertyConfiguration : IEntityTypeConfiguration<WeaponProperty>
{
    public void Configure(EntityTypeBuilder<WeaponProperty> b)
    {
        EquipmentEf.Key(b, "weapon_properties");
        b.Property(x => x.Code)
.HasMaxLength(80)
.HasColumnName("code");
        b.Property(x => x.Name)
.HasMaxLength(180)
.HasColumnName("name");
        b.Property(x => x.ValueType)
.HasColumnName("value_type");
        b
.HasIndex(x => x.Code)
.IsUnique();
        b
.HasMany(x => x.Rules)
.WithOne()
.HasForeignKey(x => x.WeaponPropertyId)
.OnDelete(DeleteBehavior.Cascade);
    }
}
internal sealed class WeaponPropertyRuleConfiguration : IEntityTypeConfiguration<WeaponPropertyRule>
{
    public void Configure(EntityTypeBuilder<WeaponPropertyRule> b)
    {
        EquipmentEf.Key(b, "weapon_property_rules");
        b.IdProperty(x => x.WeaponPropertyId, "weapon_property_id");
        b.Property(x => x.Field)
.HasMaxLength(80)
.HasColumnName("field");
        b.Property(x => x.Operator)
.HasMaxLength(40)
.HasColumnName("operator");
        b.Property(x => x.Value)
.HasMaxLength(500)
.HasColumnName("value");
    }
}
internal sealed class WeaponPropertyLinkConfiguration : IEntityTypeConfiguration<WeaponPropertyLink>
{
    public void Configure(EntityTypeBuilder<WeaponPropertyLink> b)
    {
        EquipmentEf.Key(b, "weapon_property_links");
        b.IdProperty(x => x.WeaponId, "weapon_id");
        b.IdProperty(x => x.WeaponPropertyId, "weapon_property_id");
        b
.HasIndex(x => new { x.WeaponId, x.WeaponPropertyId })
.IsUnique();
        b
.HasMany(x => x.Values)
.WithOne()
.HasForeignKey(x => x.WeaponPropertyLinkId)
.OnDelete(DeleteBehavior.Cascade);
    }
}
internal sealed class WeaponPropertyLinkValueConfiguration : IEntityTypeConfiguration<WeaponPropertyLinkValue>
{
    public void Configure(EntityTypeBuilder<WeaponPropertyLinkValue> b)
    {
        EquipmentEf.Key(b, "weapon_property_link_values");
        b.IdProperty(x => x.WeaponPropertyLinkId, "weapon_property_link_id");
        b.Property(x => x.Ordinal)
.HasColumnName("ordinal");
        b.Property(x => x.Value)
.HasMaxLength(500)
.HasColumnName("value");
    }
}
internal sealed class WeaponMasteryPropertyConfiguration : IEntityTypeConfiguration<WeaponMasteryProperty>
{
    public void Configure(EntityTypeBuilder<WeaponMasteryProperty> b)
    {
        EquipmentEf.Key(b, "weapon_mastery_properties");
        b.Property(x => x.Code)
.HasMaxLength(80)
.HasColumnName("code");
        b.Property(x => x.Name)
.HasMaxLength(180)
.HasColumnName("name");
        b
.HasIndex(x => x.Code)
.IsUnique();
        b
.HasMany(x => x.Effects)
.WithOne()
.HasForeignKey(x => x.WeaponMasteryPropertyId)
.OnDelete(DeleteBehavior.Cascade);
        b
.HasMany(x => x.Requirements)
.WithOne()
.HasForeignKey(x => x.WeaponMasteryPropertyId)
.OnDelete(DeleteBehavior.Cascade);
    }
}
internal sealed class WeaponMasteryEffectConfiguration : IEntityTypeConfiguration<WeaponMasteryEffect>
{
    public void Configure(EntityTypeBuilder<WeaponMasteryEffect> b)
    {
        EquipmentEf.Key(b, "weapon_mastery_effects");
        b.IdProperty(x => x.WeaponMasteryPropertyId, "weapon_mastery_property_id");
        b.Property(x => x.Type)
.HasColumnName("type");
        b.Property(x => x.Value)
.HasMaxLength(1000)
.HasColumnName("value");
    }
}
internal sealed class WeaponMasteryRequirementConfiguration : IEntityTypeConfiguration<WeaponMasteryRequirement>
{
    public void Configure(EntityTypeBuilder<WeaponMasteryRequirement> b)
    {
        EquipmentEf.Key(b, "weapon_mastery_requirements");
        b.IdProperty(x => x.WeaponMasteryPropertyId, "weapon_mastery_property_id");
        b.Property(x => x.Type)
.HasColumnName("type");
        b.Property(x => x.Value)
.HasMaxLength(500)
.HasColumnName("value");
    }
}
internal sealed class ArmorConfiguration : IEntityTypeConfiguration<Armor>
{
    public void Configure(EntityTypeBuilder<Armor> b)
    {
        EquipmentEf.Key(b, "armors");
        b.IdProperty(x => x.EquipmentItemId, "equipment_item_id");
        b.IdProperty(x => x.ArmorTrainingCategoryId, "armor_training_category_id");
        b
.HasIndex(x => x.EquipmentItemId)
.IsUnique();
        b
.HasMany(x => x.AcRules)
.WithOne()
.HasForeignKey(x => x.ArmorId)
.OnDelete(DeleteBehavior.Cascade);
        b
.HasMany(x => x.Drawbacks)
.WithOne()
.HasForeignKey(x => x.ArmorId)
.OnDelete(DeleteBehavior.Cascade);
    }
}
internal sealed class ArmorAcRuleConfiguration : IEntityTypeConfiguration<ArmorAcRule>
{
    public void Configure(EntityTypeBuilder<ArmorAcRule> b)
    {
        EquipmentEf.Key(b, "armor_ac_rules");
        b.IdProperty(x => x.ArmorId, "armor_id");
        b.Property(x => x.BaseAc)
.HasColumnName("base_ac");
        b.Property(x => x.AddsDexterity)
.HasColumnName("adds_dexterity");
        b.Property(x => x.MaximumDexterityBonus)
.HasColumnName("maximum_dexterity_bonus");
        b.Property(x => x.Bonus)
.HasColumnName("bonus");
    }
}
internal sealed class ArmorDrawbackConfiguration : IEntityTypeConfiguration<ArmorDrawback>
{
    public void Configure(EntityTypeBuilder<ArmorDrawback> b)
    {
        EquipmentEf.Key(b, "armor_drawbacks");
        b.IdProperty(x => x.ArmorId, "armor_id");
        b.Property(x => x.Type)
.HasColumnName("type");
        b.Property(x => x.Threshold)
.HasColumnName("threshold");
        b.Property(x => x.Description)
.HasMaxLength(1000)
.HasColumnName("description");
    }
}
internal sealed class ToolConfiguration : IEntityTypeConfiguration<Tool>
{
    public void Configure(EntityTypeBuilder<Tool> b)
    {
        EquipmentEf.Key(b, "tools");
        b.IdProperty(x => x.EquipmentItemId, "equipment_item_id");
        b.Property(x => x.ProficiencyId)
.HasConversion(EquipmentEf.NullableId)
.HasColumnName("proficiency_id");
        b.Property(x => x.AbilityCode)
.HasMaxLength(40)
.HasColumnName("ability_code");
        b
.HasIndex(x => x.EquipmentItemId)
.IsUnique();
    }
}
internal sealed class EquipmentPackConfiguration : IEntityTypeConfiguration<EquipmentPack>
{
    public void Configure(EntityTypeBuilder<EquipmentPack> b)
    {
        EquipmentEf.Key(b, "equipment_packs");
        b.IdProperty(x => x.EquipmentItemId, "equipment_item_id");
        b
.HasIndex(x => x.EquipmentItemId)
.IsUnique();
        b
.HasMany(x => x.Items)
.WithOne()
.HasForeignKey(x => x.EquipmentPackId)
.OnDelete(DeleteBehavior.Cascade);
    }
}
internal sealed class EquipmentPackItemConfiguration : IEntityTypeConfiguration<EquipmentPackItem>
{
    public void Configure(EntityTypeBuilder<EquipmentPackItem> b)
    {
        EquipmentEf.Key(b, "equipment_pack_items");
        b.IdProperty(x => x.EquipmentPackId, "equipment_pack_id");
        b.IdProperty(x => x.EquipmentItemId, "equipment_item_id");
        b.Property(x => x.Quantity)
.HasColumnName("quantity");
        b
.HasIndex(x => new { x.EquipmentPackId, x.EquipmentItemId })
.IsUnique();
    }
}
internal sealed class StartingEquipmentRuleConfiguration : IEntityTypeConfiguration<StartingEquipmentRule>
{
    public void Configure(EntityTypeBuilder<StartingEquipmentRule> b)
    {
        EquipmentEf.Key(b, "starting_equipment_rules");
        b.Property(x => x.OwnerType)
.HasColumnName("owner_type");
        b.IdProperty(x => x.OwnerEntityId, "owner_entity_id");
        b
.HasIndex(x => new { x.OwnerType, x.OwnerEntityId })
.IsUnique();
        b
.HasMany(x => x.Groups)
.WithOne()
.HasForeignKey(x => x.StartingEquipmentRuleId)
.OnDelete(DeleteBehavior.Cascade);
    }
}
internal sealed class StartingEquipmentGroupConfiguration : IEntityTypeConfiguration<StartingEquipmentGroup>
{
    public void Configure(EntityTypeBuilder<StartingEquipmentGroup> b)
    {
        EquipmentEf.Key(b, "starting_equipment_groups");
        b.IdProperty(x => x.StartingEquipmentRuleId, "starting_equipment_rule_id");
        b.Property(x => x.Ordinal)
.HasColumnName("ordinal");
        b.Property(x => x.SelectionCount)
.HasColumnName("selection_count");
        b
.HasMany(x => x.Options)
.WithOne()
.HasForeignKey(x => x.StartingEquipmentGroupId)
.OnDelete(DeleteBehavior.Cascade);
    }
}
internal sealed class StartingEquipmentOptionConfiguration : IEntityTypeConfiguration<StartingEquipmentOption>
{
    public void Configure(EntityTypeBuilder<StartingEquipmentOption> b)
    {
        EquipmentEf.Key(b, "starting_equipment_options");
        b.IdProperty(x => x.StartingEquipmentGroupId, "starting_equipment_group_id");
        b.Property(x => x.Ordinal)
.HasColumnName("ordinal");
        b.Property(x => x.Type)
.HasColumnName("type");
        b.IdProperty(x => x.ReferenceId, "reference_id");
        b.Property(x => x.Quantity)
.HasColumnName("quantity");
    }
}
