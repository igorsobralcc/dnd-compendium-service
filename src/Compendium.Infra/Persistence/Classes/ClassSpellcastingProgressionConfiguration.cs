using Compendium.Domain.Classes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Compendium.Infra.Persistence.Classes;

internal sealed class ClassSpellcastingProgressionConfiguration : IEntityTypeConfiguration<ClassSpellcastingProgression>
{
    public void Configure(EntityTypeBuilder<ClassSpellcastingProgression> builder)
    {
        builder.ToTable("class_spellcasting_progressions", CompendiumDbContext.Schema);

        builder.HasKey(progression => progression.Id).HasName("pk_class_spellcasting_progressions");
        builder.Property(progression => progression.Id)
            .HasConversion(ClassEfConversions.EntityId)
            .ValueGeneratedNever()
            .HasColumnName("id");

        builder.Property(progression => progression.CharacterClassId)
            .HasConversion(ClassEfConversions.EntityId)
            .HasColumnName("character_class_id")
            .IsRequired();

        builder.Property(progression => progression.Type)
            .HasConversion<string>()
            .HasMaxLength(40)
            .HasColumnName("type")
            .IsRequired();

        builder.Property(progression => progression.SpellcastingAbilityId)
            .HasConversion(ClassEfConversions.NullableEntityId)
            .HasColumnName("spellcasting_ability_id");

        builder.HasMany(progression => progression.LevelRules)
            .WithOne()
            .HasForeignKey(rule => rule.ClassSpellcastingProgressionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(progression => progression.CharacterClassId)
            .IsUnique()
            .HasDatabaseName("ux_class_spellcasting_progressions_class");
    }
}
