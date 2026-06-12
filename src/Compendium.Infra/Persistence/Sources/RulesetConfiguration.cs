using Compendium.Domain.Sources;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Compendium.Infra.Persistence.Sources;

internal sealed class RulesetConfiguration : IEntityTypeConfiguration<Ruleset>
{
    public void Configure(EntityTypeBuilder<Ruleset> builder)
    {
        builder.ToTable("rulesets", CompendiumDbContext.Schema);

        builder.HasKey(ruleset => ruleset.Id).HasName("pk_rulesets");
        builder.Property(ruleset => ruleset.Id)
            .HasConversion(SourceEfConversions.EntityId)
            .ValueGeneratedNever()
            .HasColumnName("id");

        builder.Property(ruleset => ruleset.Code)
            .HasConversion(SourceEfConversions.RulesetCode)
            .HasMaxLength(RulesetCode.MaxLength)
            .HasColumnName("code")
            .IsRequired();

        builder.Property(ruleset => ruleset.Name)
            .HasConversion(SourceEfConversions.RulesetName)
            .HasMaxLength(RulesetName.MaxLength)
            .HasColumnName("name")
            .IsRequired();

        builder.Property(ruleset => ruleset.Version)
            .HasConversion(SourceEfConversions.RulesetVersion)
            .HasMaxLength(RulesetVersion.MaxLength)
            .HasColumnName("version")
            .IsRequired();

        builder.Property(ruleset => ruleset.Status)
            .HasConversion<string>()
            .HasMaxLength(40)
            .HasColumnName("status")
            .IsRequired();

        builder.Property(ruleset => ruleset.CreatedAtUtc).HasColumnName("created_at_utc");
        builder.Property(ruleset => ruleset.UpdatedAtUtc).HasColumnName("updated_at_utc");

        builder.HasIndex(ruleset => ruleset.Code)
            .IsUnique()
            .HasDatabaseName("ux_rulesets_code");
    }
}
