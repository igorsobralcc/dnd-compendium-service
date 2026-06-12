using Compendium.Domain.Fundamentals;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Compendium.Infra.Persistence.Fundamentals;

internal sealed class LanguageConfiguration : IEntityTypeConfiguration<Language>
{
    public void Configure(EntityTypeBuilder<Language> builder)
    {
        builder.ToTable("languages", CompendiumDbContext.Schema);

        builder.HasKey(language => language.Id).HasName("pk_languages");
        builder.Property(language => language.Id)
            .HasConversion(FundamentalEfConversions.EntityId)
            .ValueGeneratedNever()
            .HasColumnName("id");

        builder.Property(language => language.RuleSourceId)
            .HasConversion(FundamentalEfConversions.EntityId)
            .HasColumnName("rule_source_id")
            .IsRequired();

        builder.Property(language => language.SourceVersionId)
            .HasConversion(FundamentalEfConversions.EntityId)
            .HasColumnName("source_version_id")
            .IsRequired();

        builder.Property(language => language.Code)
            .HasConversion(FundamentalEfConversions.LanguageCode)
            .HasMaxLength(LanguageCode.MaxLength)
            .HasColumnName("code")
            .IsRequired();

        builder.Property(language => language.Name)
            .HasConversion(FundamentalEfConversions.DisplayName)
            .HasMaxLength(DisplayName.MaxLength)
            .HasColumnName("name")
            .IsRequired();

        builder.Property(language => language.CreatedAtUtc).HasColumnName("created_at_utc");
        builder.Property(language => language.UpdatedAtUtc).HasColumnName("updated_at_utc");

        builder.HasIndex(language => language.Code)
            .IsUnique()
            .HasDatabaseName("ux_languages_code");

        builder.HasIndex(language => language.SourceVersionId)
            .HasDatabaseName("ix_languages_source_version_id");
    }
}
