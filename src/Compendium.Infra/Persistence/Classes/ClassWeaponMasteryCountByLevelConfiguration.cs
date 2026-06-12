using Compendium.Domain.Classes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Compendium.Infra.Persistence.Classes;

internal sealed class ClassWeaponMasteryCountByLevelConfiguration : IEntityTypeConfiguration<ClassWeaponMasteryCountByLevel>
{
    public void Configure(EntityTypeBuilder<ClassWeaponMasteryCountByLevel> builder)
    {
        builder.ToTable("class_weapon_mastery_count_by_level", CompendiumDbContext.Schema);

        builder.HasKey(count => count.Id).HasName("pk_class_weapon_mastery_count_by_level");
        builder.Property(count => count.Id)
            .HasConversion(ClassEfConversions.EntityId)
            .ValueGeneratedNever()
            .HasColumnName("id");

        builder.Property(count => count.ClassLevelId)
            .HasConversion(ClassEfConversions.EntityId)
            .HasColumnName("class_level_id")
            .IsRequired();

        builder.Property(count => count.Count).HasColumnName("count");

        builder.HasIndex(count => count.ClassLevelId)
            .IsUnique()
            .HasDatabaseName("ux_class_weapon_mastery_count_by_level_level");
    }
}
