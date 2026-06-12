using Compendium.Domain.Fundamentals;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Compendium.Infra.Persistence.Fundamentals;

internal sealed class HitDieConfiguration : IEntityTypeConfiguration<HitDie>
{
    public void Configure(EntityTypeBuilder<HitDie> builder)
    {
        builder.ToTable("hit_dice", CompendiumDbContext.Schema);

        builder.HasKey(hitDie => hitDie.Id).HasName("pk_hit_dice");
        builder.Property(hitDie => hitDie.Id)
            .HasConversion(FundamentalEfConversions.EntityId)
            .ValueGeneratedNever()
            .HasColumnName("id");

        builder.Property(hitDie => hitDie.RuleSourceId)
            .HasConversion(FundamentalEfConversions.EntityId)
            .HasColumnName("rule_source_id")
            .IsRequired();

        builder.Property(hitDie => hitDie.SourceVersionId)
            .HasConversion(FundamentalEfConversions.EntityId)
            .HasColumnName("source_version_id")
            .IsRequired();

        builder.Property(hitDie => hitDie.Code)
            .HasConversion(FundamentalEfConversions.HitDieCode)
            .HasMaxLength(HitDieCode.MaxLength)
            .HasColumnName("code")
            .IsRequired();

        builder.Property(hitDie => hitDie.Name)
            .HasConversion(FundamentalEfConversions.DisplayName)
            .HasMaxLength(DisplayName.MaxLength)
            .HasColumnName("name")
            .IsRequired();

        builder.Property(hitDie => hitDie.Die).HasColumnName("die");
        builder.Property(hitDie => hitDie.CreatedAtUtc).HasColumnName("created_at_utc");
        builder.Property(hitDie => hitDie.UpdatedAtUtc).HasColumnName("updated_at_utc");

        builder.HasIndex(hitDie => hitDie.Die)
            .IsUnique()
            .HasDatabaseName("ux_hit_dice_die");
    }
}
