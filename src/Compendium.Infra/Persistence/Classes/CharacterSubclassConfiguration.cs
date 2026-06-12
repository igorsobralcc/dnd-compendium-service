using Compendium.Domain.Classes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Compendium.Infra.Persistence.Classes;

internal sealed class CharacterSubclassConfiguration : IEntityTypeConfiguration<CharacterSubclass>
{
    public void Configure(EntityTypeBuilder<CharacterSubclass> builder)
    {
        builder.ToTable("subclasses", CompendiumDbContext.Schema);

        builder.HasKey(subclass => subclass.Id).HasName("pk_subclasses");
        builder.Property(subclass => subclass.Id)
            .HasConversion(ClassEfConversions.EntityId)
            .ValueGeneratedNever()
            .HasColumnName("id");

        builder.Property(subclass => subclass.CharacterClassId)
            .HasConversion(ClassEfConversions.EntityId)
            .HasColumnName("character_class_id")
            .IsRequired();

        builder.Property(subclass => subclass.RuleSourceId)
            .HasConversion(ClassEfConversions.EntityId)
            .HasColumnName("rule_source_id")
            .IsRequired();

        builder.Property(subclass => subclass.SourceVersionId)
            .HasConversion(ClassEfConversions.EntityId)
            .HasColumnName("source_version_id")
            .IsRequired();

        builder.Property(subclass => subclass.Code)
            .HasConversion(ClassEfConversions.ClassCode)
            .HasMaxLength(ClassCode.MaxLength)
            .HasColumnName("code")
            .IsRequired();

        builder.Property(subclass => subclass.Name)
            .HasConversion(ClassEfConversions.ClassName)
            .HasMaxLength(ClassName.MaxLength)
            .HasColumnName("name")
            .IsRequired();

        builder.Property(subclass => subclass.Description)
            .HasConversion(ClassEfConversions.NullableClassDescription)
            .HasMaxLength(ClassDescription.MaxLength)
            .HasColumnName("description");

        builder.Property(subclass => subclass.CreatedAtUtc).HasColumnName("created_at_utc");
        builder.Property(subclass => subclass.UpdatedAtUtc).HasColumnName("updated_at_utc");

        builder.HasMany(subclass => subclass.Features)
            .WithOne()
            .HasForeignKey(feature => feature.CharacterSubclassId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(subclass => new { subclass.CharacterClassId, subclass.Code })
            .IsUnique()
            .HasDatabaseName("ux_subclasses_class_code");
    }
}
