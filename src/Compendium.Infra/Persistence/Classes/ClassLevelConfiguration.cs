using Compendium.Domain.Classes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Compendium.Infra.Persistence.Classes;

internal sealed class ClassLevelConfiguration : IEntityTypeConfiguration<ClassLevel>
{
    public void Configure(EntityTypeBuilder<ClassLevel> builder)
    {
        builder.ToTable("class_levels", CompendiumDbContext.Schema);

        builder.HasKey(level => level.Id).HasName("pk_class_levels");
        builder.Property(level => level.Id)
            .HasConversion(ClassEfConversions.EntityId)
            .ValueGeneratedNever()
            .HasColumnName("id");

        builder.Property(level => level.CharacterClassId)
            .HasConversion(ClassEfConversions.EntityId)
            .HasColumnName("character_class_id")
            .IsRequired();

        builder.Property(level => level.Level).HasColumnName("level");
        builder.Property(level => level.ProficiencyBonus).HasColumnName("proficiency_bonus");

        builder.HasMany(level => level.SpellSlots)
            .WithOne()
            .HasForeignKey(slot => slot.ClassLevelId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(level => level.ProficiencyGrants)
            .WithOne()
            .HasForeignKey(grant => grant.ClassLevelId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(level => level.WeaponMasteryCounts)
            .WithOne()
            .HasForeignKey(count => count.ClassLevelId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(level => new { level.CharacterClassId, level.Level })
            .IsUnique()
            .HasDatabaseName("ux_class_levels_class_level");
    }
}
