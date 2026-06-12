using Compendium.Domain.Fundamentals;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Compendium.Infra.Persistence.Fundamentals;

internal sealed class AbilityScoreStandardValueConfiguration : IEntityTypeConfiguration<AbilityScoreStandardValue>
{
    public void Configure(EntityTypeBuilder<AbilityScoreStandardValue> builder)
    {
        builder.ToTable("ability_score_standard_values", CompendiumDbContext.Schema);

        builder.HasKey(value => value.Id).HasName("pk_ability_score_standard_values");
        builder.Property(value => value.Id)
            .HasConversion(FundamentalEfConversions.EntityId)
            .ValueGeneratedNever()
            .HasColumnName("id");

        builder.Property(value => value.AbilityScoreMethodId)
            .HasConversion(FundamentalEfConversions.EntityId)
            .HasColumnName("ability_score_method_id")
            .IsRequired();

        builder.Property(value => value.Position).HasColumnName("position");
        builder.Property(value => value.Score).HasColumnName("score");

        builder.HasIndex(value => new { value.AbilityScoreMethodId, value.Position })
            .IsUnique()
            .HasDatabaseName("ux_ability_score_standard_values_method_position");
    }
}
