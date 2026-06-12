using Compendium.Domain.Fundamentals;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Compendium.Infra.Persistence.Fundamentals;

internal sealed class ArmorTrainingCategoryConfiguration : IEntityTypeConfiguration<ArmorTrainingCategory>
{
    public void Configure(EntityTypeBuilder<ArmorTrainingCategory> builder)
    {
        builder.ToTable("armor_training_categories", CompendiumDbContext.Schema);

        builder.HasKey(category => category.Id).HasName("pk_armor_training_categories");
        builder.Property(category => category.Id)
            .HasConversion(FundamentalEfConversions.EntityId)
            .ValueGeneratedNever()
            .HasColumnName("id");

        builder.Property(category => category.RuleSourceId)
            .HasConversion(FundamentalEfConversions.EntityId)
            .HasColumnName("rule_source_id")
            .IsRequired();

        builder.Property(category => category.SourceVersionId)
            .HasConversion(FundamentalEfConversions.EntityId)
            .HasColumnName("source_version_id")
            .IsRequired();

        builder.Property(category => category.Code)
            .HasConversion(FundamentalEfConversions.ArmorTrainingCategoryCode)
            .HasMaxLength(ArmorTrainingCategoryCode.MaxLength)
            .HasColumnName("code")
            .IsRequired();

        builder.Property(category => category.Name)
            .HasConversion(FundamentalEfConversions.DisplayName)
            .HasMaxLength(DisplayName.MaxLength)
            .HasColumnName("name")
            .IsRequired();

        builder.Property(category => category.SortOrder).HasColumnName("sort_order");
        builder.Property(category => category.CreatedAtUtc).HasColumnName("created_at_utc");
        builder.Property(category => category.UpdatedAtUtc).HasColumnName("updated_at_utc");

        builder.HasIndex(category => category.Code)
            .IsUnique()
            .HasDatabaseName("ux_armor_training_categories_code");

        builder.HasIndex(category => category.SortOrder)
            .HasDatabaseName("ix_armor_training_categories_sort_order");
    }
}
