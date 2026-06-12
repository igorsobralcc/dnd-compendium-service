using Compendium.Domain.Sources;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Compendium.Infra.Persistence.Sources;

internal sealed class SourceVersionConfiguration : IEntityTypeConfiguration<SourceVersion>
{
    public void Configure(EntityTypeBuilder<SourceVersion> builder)
    {
        builder.ToTable("source_versions", CompendiumDbContext.Schema);

        builder.HasKey(version => version.Id).HasName("pk_source_versions");
        builder.Property(version => version.Id)
            .HasConversion(SourceEfConversions.EntityId)
            .ValueGeneratedNever()
            .HasColumnName("id");

        builder.Property(version => version.RuleSourceId)
            .HasConversion(SourceEfConversions.EntityId)
            .HasColumnName("rule_source_id")
            .IsRequired();

        builder.Property(version => version.VersionNumber)
            .HasConversion(SourceEfConversions.SourceVersionNumber)
            .HasMaxLength(SourceVersionNumber.MaxLength)
            .HasColumnName("version_number")
            .IsRequired();

        builder.Property(version => version.PublicationDate)
            .HasConversion(SourceEfConversions.PublicationDate)
            .HasColumnName("publication_date")
            .IsRequired();

        builder.Property(version => version.ImportStatus)
            .HasConversion<string>()
            .HasMaxLength(40)
            .HasColumnName("import_status")
            .IsRequired();

        builder.Property(version => version.IsCurrent).HasColumnName("is_current");
        builder.Property(version => version.CreatedAtUtc).HasColumnName("created_at_utc");
        builder.Property(version => version.UpdatedAtUtc).HasColumnName("updated_at_utc");

        builder.Ignore(version => version.DomainEvents);

        builder.HasIndex(version => version.RuleSourceId)
            .HasDatabaseName("ix_source_versions_rule_source_id");

        builder.HasIndex(version => new { version.RuleSourceId, version.VersionNumber })
            .IsUnique()
            .HasDatabaseName("ux_source_versions_source_version");

        builder.HasIndex(version => new { version.RuleSourceId, version.IsCurrent })
            .HasFilter("is_current = true")
            .IsUnique()
            .HasDatabaseName("ux_source_versions_current_per_source");
    }
}
