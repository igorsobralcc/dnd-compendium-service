using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Compendium.Infra.Persistence.Importing;

internal sealed class SourceVersionImportRecordConfiguration : IEntityTypeConfiguration<SourceVersionImportRecord>
{
    public void Configure(EntityTypeBuilder<SourceVersionImportRecord> builder)
    {
        builder.ToTable("source_version_imports");
        builder.HasKey(x => x.Id).HasName("pk_source_version_imports");
        builder.Property(x => x.Id).HasColumnName("id").ValueGeneratedNever();
        builder.Property(x => x.SourceVersionId).HasColumnName("source_version_id").IsRequired();
        builder.Property(x => x.ContentHash).HasColumnName("content_hash").HasMaxLength(64).IsRequired();
        builder.Property(x => x.ImportedEntityCount).HasColumnName("imported_entity_count").IsRequired();
        builder.Property(x => x.ImportedAtUtc).HasColumnName("imported_at_utc").IsRequired();
        builder.HasIndex(x => x.SourceVersionId).IsUnique().HasDatabaseName("ux_source_version_imports_source_version_id");
    }
}

internal sealed class SourceVersionValidationIssueConfiguration : IEntityTypeConfiguration<SourceVersionValidationIssue>
{
    public void Configure(EntityTypeBuilder<SourceVersionValidationIssue> builder)
    {
        builder.ToTable("source_version_validation_issues");
        builder.HasKey(x => x.Id).HasName("pk_source_version_validation_issues");
        builder.Property(x => x.Id).HasColumnName("id").ValueGeneratedNever();
        builder.Property(x => x.SourceVersionId).HasColumnName("source_version_id").IsRequired();
        builder.Property(x => x.Code).HasColumnName("code").HasMaxLength(100).IsRequired();
        builder.Property(x => x.Severity).HasColumnName("severity").IsRequired();
        builder.Property(x => x.Message).HasColumnName("message").HasMaxLength(1000).IsRequired();
        builder.Property(x => x.CreatedAtUtc).HasColumnName("created_at_utc").IsRequired();
        builder.HasIndex(x => x.SourceVersionId).HasDatabaseName("ix_source_version_validation_issues_source_version_id");
        builder.HasIndex(x => new { x.SourceVersionId, x.Code }).IsUnique().HasDatabaseName("ux_source_version_validation_issues_version_code");
    }
}
