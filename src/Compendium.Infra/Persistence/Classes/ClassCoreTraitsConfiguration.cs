using Compendium.Domain.Classes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Compendium.Infra.Persistence.Classes;

internal sealed class ClassCoreTraitsConfiguration : IEntityTypeConfiguration<ClassCoreTraits>
{
    public void Configure(EntityTypeBuilder<ClassCoreTraits> builder)
    {
        builder.ToTable("class_core_traits", CompendiumDbContext.Schema);

        builder.HasKey(coreTraits => coreTraits.Id).HasName("pk_class_core_traits");
        builder.Property(coreTraits => coreTraits.Id)
            .HasConversion(ClassEfConversions.EntityId)
            .ValueGeneratedNever()
            .HasColumnName("id");

        builder.Property(coreTraits => coreTraits.CharacterClassId)
            .HasConversion(ClassEfConversions.EntityId)
            .HasColumnName("character_class_id")
            .IsRequired();

        builder.Property(coreTraits => coreTraits.HitDieId)
            .HasConversion(ClassEfConversions.EntityId)
            .HasColumnName("hit_die_id")
            .IsRequired();

        builder.Property(coreTraits => coreTraits.ArmorTrainingCategoryId)
            .HasConversion(ClassEfConversions.NullableEntityId)
            .HasColumnName("armor_training_category_id");

        builder.Property(coreTraits => coreTraits.SkillChoiceCount).HasColumnName("skill_choice_count");

        builder.HasIndex(coreTraits => coreTraits.CharacterClassId)
            .IsUnique()
            .HasDatabaseName("ux_class_core_traits_class");
    }
}
