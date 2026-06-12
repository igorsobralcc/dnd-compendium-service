using Compendium.Domain.Sources;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Compendium.Infra.Persistence.Sources;

internal sealed class RuleSourceConfiguration : IEntityTypeConfiguration<RuleSource>
{
    public void Configure(EntityTypeBuilder<RuleSource> builder)
    {
        builder.ToTable("rule_sources", CompendiumDbContext.Schema);

        builder.HasKey(source => source.Id).HasName("pk_rule_sources");
        builder.Property(source => source.Id)
            .HasConversion(SourceEfConversions.EntityId)
            .ValueGeneratedNever()
            .HasColumnName("id");

        builder.Property(source => source.RulesetId)
            .HasConversion(SourceEfConversions.EntityId)
            .HasColumnName("ruleset_id")
            .IsRequired();

        builder.Property(source => source.Code)
            .HasConversion(SourceEfConversions.SourceCode)
            .HasMaxLength(SourceCode.MaxLength)
            .HasColumnName("code")
            .IsRequired();

        builder.Property(source => source.Name)
            .HasConversion(SourceEfConversions.SourceName)
            .HasMaxLength(SourceName.MaxLength)
            .HasColumnName("name")
            .IsRequired();

        builder.Property(source => source.Type)
            .HasConversion<string>()
            .HasMaxLength(40)
            .HasColumnName("type")
            .IsRequired();

        builder.Property(source => source.Status)
            .HasConversion<string>()
            .HasMaxLength(40)
            .HasColumnName("status")
            .IsRequired();

        builder.Property(source => source.CreatedAtUtc).HasColumnName("created_at_utc");
        builder.Property(source => source.UpdatedAtUtc).HasColumnName("updated_at_utc");

        builder.HasIndex(source => source.RulesetId)
            .HasDatabaseName("ix_rule_sources_ruleset_id");

        builder.HasIndex(source => new { source.RulesetId, source.Code })
            .IsUnique()
            .HasDatabaseName("ux_rule_sources_ruleset_code");
    }
}
