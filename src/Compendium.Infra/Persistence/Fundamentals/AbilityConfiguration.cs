using Compendium.Domain.Fundamentals;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Compendium.Infra.Persistence.Fundamentals;

internal sealed class AbilityConfiguration : IEntityTypeConfiguration<Ability>
{
    public void Configure(EntityTypeBuilder<Ability> builder)
    {
        builder.ToTable("abilities", CompendiumDbContext.Schema);

        builder.HasKey(ability => ability.Id).HasName("pk_abilities");
        builder.Property(ability => ability.Id)
            .HasConversion(FundamentalEfConversions.EntityId)
            .ValueGeneratedNever()
            .HasColumnName("id");

        builder.Property(ability => ability.RuleSourceId)
            .HasConversion(FundamentalEfConversions.EntityId)
            .HasColumnName("rule_source_id")
            .IsRequired();

        builder.Property(ability => ability.SourceVersionId)
            .HasConversion(FundamentalEfConversions.EntityId)
            .HasColumnName("source_version_id")
            .IsRequired();

        builder.Property(ability => ability.Code)
            .HasConversion(FundamentalEfConversions.AbilityCode)
            .HasMaxLength(AbilityCode.MaxLength)
            .HasColumnName("code")
            .IsRequired();

        builder.Property(ability => ability.Name)
            .HasConversion(FundamentalEfConversions.DisplayName)
            .HasMaxLength(DisplayName.MaxLength)
            .HasColumnName("name")
            .IsRequired();

        builder.Property(ability => ability.CreatedAtUtc).HasColumnName("created_at_utc");
        builder.Property(ability => ability.UpdatedAtUtc).HasColumnName("updated_at_utc");

        builder.HasIndex(ability => ability.Code)
            .IsUnique()
            .HasDatabaseName("ux_abilities_code");

        builder.HasIndex(ability => ability.SourceVersionId)
            .HasDatabaseName("ix_abilities_source_version_id");
    }
}
