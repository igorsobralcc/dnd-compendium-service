using Compendium.Domain.Classes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Compendium.Infra.Persistence.Classes;

internal sealed class CharacterClassConfiguration : IEntityTypeConfiguration<CharacterClass>
{
    public void Configure(EntityTypeBuilder<CharacterClass> builder)
    {
        builder.ToTable("classes", CompendiumDbContext.Schema);

        builder.HasKey(characterClass => characterClass.Id).HasName("pk_classes");
        builder.Property(characterClass => characterClass.Id)
            .HasConversion(ClassEfConversions.EntityId)
            .ValueGeneratedNever()
            .HasColumnName("id");

        builder.Property(characterClass => characterClass.RuleSourceId)
            .HasConversion(ClassEfConversions.EntityId)
            .HasColumnName("rule_source_id")
            .IsRequired();

        builder.Property(characterClass => characterClass.SourceVersionId)
            .HasConversion(ClassEfConversions.EntityId)
            .HasColumnName("source_version_id")
            .IsRequired();

        builder.Property(characterClass => characterClass.Code)
            .HasConversion(ClassEfConversions.ClassCode)
            .HasMaxLength(ClassCode.MaxLength)
            .HasColumnName("code")
            .IsRequired();

        builder.Property(characterClass => characterClass.Name)
            .HasConversion(ClassEfConversions.ClassName)
            .HasMaxLength(ClassName.MaxLength)
            .HasColumnName("name")
            .IsRequired();

        builder.Property(characterClass => characterClass.Description)
            .HasConversion(ClassEfConversions.NullableClassDescription)
            .HasMaxLength(ClassDescription.MaxLength)
            .HasColumnName("description");

        builder.Property(characterClass => characterClass.CreatedAtUtc).HasColumnName("created_at_utc");
        builder.Property(characterClass => characterClass.UpdatedAtUtc).HasColumnName("updated_at_utc");

        builder.HasOne(characterClass => characterClass.CoreTraits)
            .WithOne()
            .HasForeignKey<ClassCoreTraits>(coreTraits => coreTraits.CharacterClassId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(characterClass => characterClass.PrimaryAbilities)
            .WithOne()
            .HasForeignKey(primaryAbility => primaryAbility.CharacterClassId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(characterClass => characterClass.Levels)
            .WithOne()
            .HasForeignKey(level => level.CharacterClassId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(characterClass => characterClass.SpellcastingProgression)
            .WithOne()
            .HasForeignKey<ClassSpellcastingProgression>(progression => progression.CharacterClassId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(characterClass => characterClass.Code)
            .IsUnique()
            .HasDatabaseName("ux_classes_code");

        builder.HasIndex(characterClass => characterClass.SourceVersionId)
            .HasDatabaseName("ix_classes_source_version_id");
    }
}
