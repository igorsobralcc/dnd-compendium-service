using Compendium.Domain.Classes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Compendium.Infra.Persistence.Classes;

internal sealed class ClassPrimaryAbilityConfiguration : IEntityTypeConfiguration<ClassPrimaryAbility>
{
    public void Configure(EntityTypeBuilder<ClassPrimaryAbility> builder)
    {
        builder.ToTable("class_primary_abilities", CompendiumDbContext.Schema);

        builder.HasKey(primaryAbility => primaryAbility.Id).HasName("pk_class_primary_abilities");
        builder.Property(primaryAbility => primaryAbility.Id)
            .HasConversion(ClassEfConversions.EntityId)
            .ValueGeneratedNever()
            .HasColumnName("id");

        builder.Property(primaryAbility => primaryAbility.CharacterClassId)
            .HasConversion(ClassEfConversions.EntityId)
            .HasColumnName("character_class_id")
            .IsRequired();

        builder.Property(primaryAbility => primaryAbility.AbilityId)
            .HasConversion(ClassEfConversions.EntityId)
            .HasColumnName("ability_id")
            .IsRequired();

        builder.Property(primaryAbility => primaryAbility.SortOrder).HasColumnName("sort_order");

        builder.HasIndex(primaryAbility => new { primaryAbility.CharacterClassId, primaryAbility.AbilityId })
            .IsUnique()
            .HasDatabaseName("ux_class_primary_abilities_class_ability");

        builder.HasIndex(primaryAbility => new { primaryAbility.CharacterClassId, primaryAbility.SortOrder })
            .IsUnique()
            .HasDatabaseName("ux_class_primary_abilities_class_sort");
    }
}
