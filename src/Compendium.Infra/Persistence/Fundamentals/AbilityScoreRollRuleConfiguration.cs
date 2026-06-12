using Compendium.Domain.Fundamentals;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Compendium.Infra.Persistence.Fundamentals;

internal sealed class AbilityScoreRollRuleConfiguration : IEntityTypeConfiguration<AbilityScoreRollRule>
{
    public void Configure(EntityTypeBuilder<AbilityScoreRollRule> builder)
    {
        builder.ToTable("ability_score_roll_rules", CompendiumDbContext.Schema);

        builder.HasKey(rule => rule.Id).HasName("pk_ability_score_roll_rules");
        builder.Property(rule => rule.Id)
            .HasConversion(FundamentalEfConversions.EntityId)
            .ValueGeneratedNever()
            .HasColumnName("id");

        builder.Property(rule => rule.AbilityScoreMethodId)
            .HasConversion(FundamentalEfConversions.EntityId)
            .HasColumnName("ability_score_method_id")
            .IsRequired();

        builder.Property(rule => rule.DiceQuantity).HasColumnName("dice_quantity");
        builder.Property(rule => rule.DieSize).HasColumnName("die_size");
        builder.Property(rule => rule.KeepHighestQuantity).HasColumnName("keep_highest_quantity");
        builder.Property(rule => rule.DropLowestQuantity).HasColumnName("drop_lowest_quantity");
        builder.Property(rule => rule.Repetitions).HasColumnName("repetitions");

        builder.HasIndex(rule => rule.AbilityScoreMethodId)
            .IsUnique()
            .HasDatabaseName("ux_ability_score_roll_rules_method");
    }
}
