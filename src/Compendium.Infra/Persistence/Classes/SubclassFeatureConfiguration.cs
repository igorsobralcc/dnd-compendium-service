using Compendium.Domain.Classes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Compendium.Infra.Persistence.Classes;

internal sealed class SubclassFeatureConfiguration : IEntityTypeConfiguration<SubclassFeature>
{
    public void Configure(EntityTypeBuilder<SubclassFeature> builder)
    {
        builder.ToTable("subclass_features", CompendiumDbContext.Schema);

        builder.HasKey(feature => feature.Id).HasName("pk_subclass_features");
        builder.Property(feature => feature.Id)
            .HasConversion(ClassEfConversions.EntityId)
            .ValueGeneratedNever()
            .HasColumnName("id");

        builder.Property(feature => feature.CharacterSubclassId)
            .HasConversion(ClassEfConversions.EntityId)
            .HasColumnName("character_subclass_id")
            .IsRequired();

        builder.Property(feature => feature.SourceVersionId)
            .HasConversion(ClassEfConversions.EntityId)
            .HasColumnName("source_version_id")
            .IsRequired();

        builder.Property(feature => feature.FeatureId)
            .HasConversion(ClassEfConversions.EntityId)
            .HasColumnName("feature_id")
            .IsRequired();

        builder.Property(feature => feature.Level).HasColumnName("level");

        builder.HasIndex(feature => new { feature.CharacterSubclassId, feature.FeatureId, feature.Level })
            .IsUnique()
            .HasDatabaseName("ux_subclass_features_subclass_feature_level");
    }
}
