using Compendium.Domain.Classes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Compendium.Infra.Persistence.Classes;

internal sealed class ClassSpellcastingLevelRuleConfiguration : IEntityTypeConfiguration<ClassSpellcastingLevelRule>
{
    public void Configure(EntityTypeBuilder<ClassSpellcastingLevelRule> builder)
    {
        builder.ToTable("class_spellcasting_level_rules", CompendiumDbContext.Schema);

        builder.HasKey(rule => rule.Id).HasName("pk_class_spellcasting_level_rules");
        builder.Property(rule => rule.Id)
            .HasConversion(ClassEfConversions.EntityId)
            .ValueGeneratedNever()
            .HasColumnName("id");

        builder.Property(rule => rule.ClassSpellcastingProgressionId)
            .HasConversion(ClassEfConversions.EntityId)
            .HasColumnName("class_spellcasting_progression_id")
            .IsRequired();

        builder.Property(rule => rule.ClassLevel).HasColumnName("class_level");
        builder.Property(rule => rule.CasterLevel).HasColumnName("caster_level");

        builder.HasIndex(rule => new { rule.ClassSpellcastingProgressionId, rule.ClassLevel })
            .IsUnique()
            .HasDatabaseName("ux_class_spellcasting_level_rules_progression_level");
    }
}
