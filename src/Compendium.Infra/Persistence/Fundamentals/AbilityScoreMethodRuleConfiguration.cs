using Compendium.Domain.Fundamentals;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Compendium.Infra.Persistence.Fundamentals;

internal sealed class AbilityScoreMethodRuleConfiguration : IEntityTypeConfiguration<AbilityScoreMethodRule>
{
    public void Configure(EntityTypeBuilder<AbilityScoreMethodRule> builder)
    {
        builder.ToTable("ability_score_method_rules", CompendiumDbContext.Schema);

        builder.HasKey(rule => rule.Id).HasName("pk_ability_score_method_rules");
        builder.Property(rule => rule.Id)
            .HasConversion(FundamentalEfConversions.EntityId)
            .ValueGeneratedNever()
            .HasColumnName("id");

        builder.Property(rule => rule.AbilityScoreMethodId)
            .HasConversion(FundamentalEfConversions.EntityId)
            .HasColumnName("ability_score_method_id")
            .IsRequired();

        builder.Property(rule => rule.Code)
            .HasConversion(FundamentalEfConversions.AbilityScoreMethodRuleCode)
            .HasMaxLength(AbilityScoreMethodRuleCode.MaxLength)
            .HasColumnName("code")
            .IsRequired();

        builder.Property(rule => rule.NumericValue).HasColumnName("numeric_value");
        builder.Property(rule => rule.TextValue)
            .HasMaxLength(240)
            .HasColumnName("text_value");

        builder.HasIndex(rule => new { rule.AbilityScoreMethodId, rule.Code })
            .IsUnique()
            .HasDatabaseName("ux_ability_score_method_rules_method_code");
    }
}
