using Compendium.Domain.Classes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Compendium.Infra.Persistence.Classes;

internal sealed class ClassProficiencyGrantConfiguration : IEntityTypeConfiguration<ClassProficiencyGrant>
{
    public void Configure(EntityTypeBuilder<ClassProficiencyGrant> builder)
    {
        builder.ToTable("class_proficiency_grants", CompendiumDbContext.Schema);

        builder.HasKey(grant => grant.Id).HasName("pk_class_proficiency_grants");
        builder.Property(grant => grant.Id)
            .HasConversion(ClassEfConversions.EntityId)
            .ValueGeneratedNever()
            .HasColumnName("id");

        builder.Property(grant => grant.ClassLevelId)
            .HasConversion(ClassEfConversions.EntityId)
            .HasColumnName("class_level_id")
            .IsRequired();

        builder.Property(grant => grant.ProficiencyId)
            .HasConversion(ClassEfConversions.EntityId)
            .HasColumnName("proficiency_id")
            .IsRequired();

        builder.HasIndex(grant => new { grant.ClassLevelId, grant.ProficiencyId })
            .IsUnique()
            .HasDatabaseName("ux_class_proficiency_grants_level_proficiency");
    }
}
