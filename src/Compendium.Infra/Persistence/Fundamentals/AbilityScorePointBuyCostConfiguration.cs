using Compendium.Domain.Fundamentals;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Compendium.Infra.Persistence.Fundamentals;

internal sealed class AbilityScorePointBuyCostConfiguration : IEntityTypeConfiguration<AbilityScorePointBuyCost>
{
    public void Configure(EntityTypeBuilder<AbilityScorePointBuyCost> builder)
    {
        builder.ToTable("ability_score_point_buy_costs", CompendiumDbContext.Schema);

        builder.HasKey(cost => cost.Id).HasName("pk_ability_score_point_buy_costs");
        builder.Property(cost => cost.Id)
            .HasConversion(FundamentalEfConversions.EntityId)
            .ValueGeneratedNever()
            .HasColumnName("id");

        builder.Property(cost => cost.AbilityScoreMethodId)
            .HasConversion(FundamentalEfConversions.EntityId)
            .HasColumnName("ability_score_method_id")
            .IsRequired();

        builder.Property(cost => cost.Score).HasColumnName("score");
        builder.Property(cost => cost.Cost).HasColumnName("cost");

        builder.HasIndex(cost => new { cost.AbilityScoreMethodId, cost.Score })
            .IsUnique()
            .HasDatabaseName("ux_ability_score_point_buy_costs_method_score");
    }
}
