using Compendium.Domain.Classes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Compendium.Infra.Persistence.Classes;

internal sealed class ClassLevelSpellSlotConfiguration : IEntityTypeConfiguration<ClassLevelSpellSlot>
{
    public void Configure(EntityTypeBuilder<ClassLevelSpellSlot> builder)
    {
        builder.ToTable("class_level_spell_slots", CompendiumDbContext.Schema);

        builder.HasKey(slot => slot.Id).HasName("pk_class_level_spell_slots");
        builder.Property(slot => slot.Id)
            .HasConversion(ClassEfConversions.EntityId)
            .ValueGeneratedNever()
            .HasColumnName("id");

        builder.Property(slot => slot.ClassLevelId)
            .HasConversion(ClassEfConversions.EntityId)
            .HasColumnName("class_level_id")
            .IsRequired();

        builder.Property(slot => slot.SpellLevel).HasColumnName("spell_level");
        builder.Property(slot => slot.Slots).HasColumnName("slots");

        builder.HasIndex(slot => new { slot.ClassLevelId, slot.SpellLevel })
            .IsUnique()
            .HasDatabaseName("ux_class_level_spell_slots_level_spell_level");
    }
}
