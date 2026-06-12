using Compendium.Domain.Fundamentals;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Compendium.Infra.Persistence.Fundamentals;

internal sealed class SkillConfiguration : IEntityTypeConfiguration<Skill>
{
    public void Configure(EntityTypeBuilder<Skill> builder)
    {
        builder.ToTable("skills", CompendiumDbContext.Schema);

        builder.HasKey(skill => skill.Id).HasName("pk_skills");
        builder.Property(skill => skill.Id)
            .HasConversion(FundamentalEfConversions.EntityId)
            .ValueGeneratedNever()
            .HasColumnName("id");

        builder.Property(skill => skill.RuleSourceId)
            .HasConversion(FundamentalEfConversions.EntityId)
            .HasColumnName("rule_source_id")
            .IsRequired();

        builder.Property(skill => skill.SourceVersionId)
            .HasConversion(FundamentalEfConversions.EntityId)
            .HasColumnName("source_version_id")
            .IsRequired();

        builder.Property(skill => skill.Code)
            .HasConversion(FundamentalEfConversions.SkillCode)
            .HasMaxLength(SkillCode.MaxLength)
            .HasColumnName("code")
            .IsRequired();

        builder.Property(skill => skill.Name)
            .HasConversion(FundamentalEfConversions.DisplayName)
            .HasMaxLength(DisplayName.MaxLength)
            .HasColumnName("name")
            .IsRequired();

        builder.Property(skill => skill.DefaultAbilityId)
            .HasConversion(FundamentalEfConversions.NullableEntityId)
            .HasColumnName("default_ability_id");

        builder.Property(skill => skill.CreatedAtUtc).HasColumnName("created_at_utc");
        builder.Property(skill => skill.UpdatedAtUtc).HasColumnName("updated_at_utc");

        builder.HasIndex(skill => skill.Code)
            .IsUnique()
            .HasDatabaseName("ux_skills_code");

        builder.HasIndex(skill => skill.DefaultAbilityId)
            .HasDatabaseName("ix_skills_default_ability_id");

        builder.HasIndex(skill => skill.SourceVersionId)
            .HasDatabaseName("ix_skills_source_version_id");
    }
}
