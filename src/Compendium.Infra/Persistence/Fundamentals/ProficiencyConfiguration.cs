using Compendium.Domain.Fundamentals;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Compendium.Infra.Persistence.Fundamentals;

internal sealed class ProficiencyConfiguration : IEntityTypeConfiguration<Proficiency>
{
    public void Configure(EntityTypeBuilder<Proficiency> builder)
    {
        builder.ToTable("proficiencies", CompendiumDbContext.Schema);

        builder.HasKey(proficiency => proficiency.Id).HasName("pk_proficiencies");
        builder.Property(proficiency => proficiency.Id)
            .HasConversion(FundamentalEfConversions.EntityId)
            .ValueGeneratedNever()
            .HasColumnName("id");

        builder.Property(proficiency => proficiency.RuleSourceId)
            .HasConversion(FundamentalEfConversions.EntityId)
            .HasColumnName("rule_source_id")
            .IsRequired();

        builder.Property(proficiency => proficiency.SourceVersionId)
            .HasConversion(FundamentalEfConversions.EntityId)
            .HasColumnName("source_version_id")
            .IsRequired();

        builder.Property(proficiency => proficiency.Code)
            .HasConversion(FundamentalEfConversions.ProficiencyCode)
            .HasMaxLength(ProficiencyCode.MaxLength)
            .HasColumnName("code")
            .IsRequired();

        builder.Property(proficiency => proficiency.Name)
            .HasConversion(FundamentalEfConversions.DisplayName)
            .HasMaxLength(DisplayName.MaxLength)
            .HasColumnName("name")
            .IsRequired();

        builder.Property(proficiency => proficiency.Type)
            .HasConversion<string>()
            .HasMaxLength(40)
            .HasColumnName("type")
            .IsRequired();

        builder.Property(proficiency => proficiency.RelatedEntityId)
            .HasConversion(FundamentalEfConversions.NullableEntityId)
            .HasColumnName("related_entity_id");

        builder.Property(proficiency => proficiency.CreatedAtUtc).HasColumnName("created_at_utc");
        builder.Property(proficiency => proficiency.UpdatedAtUtc).HasColumnName("updated_at_utc");

        builder.HasIndex(proficiency => proficiency.Code)
            .IsUnique()
            .HasDatabaseName("ux_proficiencies_code");

        builder.HasIndex(proficiency => proficiency.Type)
            .HasDatabaseName("ix_proficiencies_type");

        builder.HasIndex(proficiency => proficiency.RelatedEntityId)
            .HasDatabaseName("ix_proficiencies_related_entity_id");
    }
}
