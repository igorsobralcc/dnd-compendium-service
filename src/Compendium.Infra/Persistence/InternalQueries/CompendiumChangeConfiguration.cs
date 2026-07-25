using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Compendium.Infra.Persistence.InternalQueries;

internal sealed class CompendiumChangeConfiguration : IEntityTypeConfiguration<CompendiumChange>
{
    public void Configure(EntityTypeBuilder<CompendiumChange> builder)
    {
        builder.ToTable("compendium_changes", CompendiumDbContext.Schema);
        builder.HasKey(x => x.Revision).HasName("pk_compendium_changes");
        builder.Property(x => x.Revision).HasColumnName("revision").ValueGeneratedOnAdd();
        builder.Property(x => x.SourceVersionId).HasColumnName("source_version_id");
        builder.Property(x => x.EntityType).HasColumnName("entity_type").HasMaxLength(80).IsRequired();
        builder.Property(x => x.EntityId).HasColumnName("entity_id").IsRequired();
        builder.Property(x => x.ChangeType).HasColumnName("change_type").HasMaxLength(20).IsRequired();
        builder.Property(x => x.ChangedAtUtc).HasColumnName("changed_at_utc").IsRequired();
        builder.HasIndex(x => new { x.SourceVersionId, x.Revision }).HasDatabaseName("ix_compendium_changes_source_revision");
        builder.HasIndex(x => new { x.EntityType, x.Revision }).HasDatabaseName("ix_compendium_changes_type_revision");
        builder.HasIndex(x => x.ChangedAtUtc).HasDatabaseName("ix_compendium_changes_changed_at");
    }
}
