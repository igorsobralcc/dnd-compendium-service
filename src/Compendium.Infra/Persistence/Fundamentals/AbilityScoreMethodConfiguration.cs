using Compendium.Domain.Fundamentals;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Compendium.Infra.Persistence.Fundamentals;

internal sealed class AbilityScoreMethodConfiguration : IEntityTypeConfiguration<AbilityScoreMethod>
{
    public void Configure(EntityTypeBuilder<AbilityScoreMethod> builder)
    {
        builder.ToTable("ability_score_methods", CompendiumDbContext.Schema);

        builder.HasKey(method => method.Id).HasName("pk_ability_score_methods");
        builder.Property(method => method.Id)
            .HasConversion(FundamentalEfConversions.EntityId)
            .ValueGeneratedNever()
            .HasColumnName("id");

        builder.Property(method => method.RuleSourceId)
            .HasConversion(FundamentalEfConversions.EntityId)
            .HasColumnName("rule_source_id")
            .IsRequired();

        builder.Property(method => method.SourceVersionId)
            .HasConversion(FundamentalEfConversions.EntityId)
            .HasColumnName("source_version_id")
            .IsRequired();

        builder.Property(method => method.Code)
            .HasConversion(FundamentalEfConversions.AbilityScoreMethodCode)
            .HasMaxLength(AbilityScoreMethodCode.MaxLength)
            .HasColumnName("code")
            .IsRequired();

        builder.Property(method => method.Name)
            .HasConversion(FundamentalEfConversions.DisplayName)
            .HasMaxLength(DisplayName.MaxLength)
            .HasColumnName("name")
            .IsRequired();

        builder.Property(method => method.Type)
            .HasConversion<string>()
            .HasMaxLength(40)
            .HasColumnName("type")
            .IsRequired();

        builder.Property(method => method.CreatedAtUtc).HasColumnName("created_at_utc");
        builder.Property(method => method.UpdatedAtUtc).HasColumnName("updated_at_utc");

        builder.HasMany(method => method.Rules)
            .WithOne()
            .HasForeignKey(rule => rule.AbilityScoreMethodId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(method => method.StandardValues)
            .WithOne()
            .HasForeignKey(value => value.AbilityScoreMethodId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(method => method.PointBuyCosts)
            .WithOne()
            .HasForeignKey(cost => cost.AbilityScoreMethodId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(method => method.RollRules)
            .WithOne()
            .HasForeignKey(rule => rule.AbilityScoreMethodId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(method => method.Code)
            .IsUnique()
            .HasDatabaseName("ux_ability_score_methods_code");

        builder.HasIndex(method => method.SourceVersionId)
            .HasDatabaseName("ix_ability_score_methods_source_version_id");
    }
}
