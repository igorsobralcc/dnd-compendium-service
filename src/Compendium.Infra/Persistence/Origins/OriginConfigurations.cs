using Compendium.Domain.Origins;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Compendium.Infra.Persistence.Origins;

internal sealed class SpeciesConfiguration : IEntityTypeConfiguration<Species>
{
    public void Configure(EntityTypeBuilder<Species> builder)
    {
        builder.ToTable("species", CompendiumDbContext.Schema);
        builder.HasKey(entity => entity.Id).HasName("pk_species");
        builder.Property(entity => entity.Id).HasConversion(OriginEfConversions.EntityId).ValueGeneratedNever().HasColumnName("id");
        builder.Property(entity => entity.RuleSourceId).HasConversion(OriginEfConversions.EntityId).HasColumnName("rule_source_id").IsRequired();
        builder.Property(entity => entity.SourceVersionId).HasConversion(OriginEfConversions.EntityId).HasColumnName("source_version_id").IsRequired();
        builder.Property(entity => entity.Code).HasConversion(OriginEfConversions.SpeciesCode).HasMaxLength(SpeciesCode.MaxLength).HasColumnName("code").IsRequired();
        builder.Property(entity => entity.Name).HasConversion(OriginEfConversions.SpeciesName).HasMaxLength(SpeciesName.MaxLength).HasColumnName("name").IsRequired();
        builder.Property(entity => entity.Description).HasConversion(OriginEfConversions.SpeciesDescription).HasMaxLength(SpeciesDescription.MaxLength).HasColumnName("description");
        builder.Property(entity => entity.CreatedAtUtc).HasColumnName("created_at_utc");
        builder.Property(entity => entity.UpdatedAtUtc).HasColumnName("updated_at_utc");
        builder.HasMany(entity => entity.Features).WithOne().HasForeignKey(feature => feature.SourceEntityId).OnDelete(DeleteBehavior.Cascade);
        builder.HasIndex(entity => entity.Code).IsUnique().HasDatabaseName("ux_species_code");
        builder.HasIndex(entity => entity.SourceVersionId).HasDatabaseName("ix_species_source_version_id");
    }
}

internal sealed class BackgroundConfiguration : IEntityTypeConfiguration<Background>
{
    public void Configure(EntityTypeBuilder<Background> builder)
    {
        builder.ToTable("backgrounds", CompendiumDbContext.Schema);
        builder.HasKey(entity => entity.Id).HasName("pk_backgrounds");
        builder.Property(entity => entity.Id).HasConversion(OriginEfConversions.EntityId).ValueGeneratedNever().HasColumnName("id");
        builder.Property(entity => entity.RuleSourceId).HasConversion(OriginEfConversions.EntityId).HasColumnName("rule_source_id").IsRequired();
        builder.Property(entity => entity.SourceVersionId).HasConversion(OriginEfConversions.EntityId).HasColumnName("source_version_id").IsRequired();
        builder.Property(entity => entity.Code).HasConversion(OriginEfConversions.BackgroundCode).HasMaxLength(BackgroundCode.MaxLength).HasColumnName("code").IsRequired();
        builder.Property(entity => entity.Name).HasConversion(OriginEfConversions.BackgroundName).HasMaxLength(BackgroundName.MaxLength).HasColumnName("name").IsRequired();
        builder.Property(entity => entity.Description).HasConversion(OriginEfConversions.BackgroundDescription).HasMaxLength(BackgroundDescription.MaxLength).HasColumnName("description");
        builder.Property(entity => entity.CreatedAtUtc).HasColumnName("created_at_utc");
        builder.Property(entity => entity.UpdatedAtUtc).HasColumnName("updated_at_utc");
        builder.HasMany(entity => entity.AbilityOptions).WithOne().HasForeignKey(item => item.BackgroundId).OnDelete(DeleteBehavior.Cascade);
        builder.HasMany(entity => entity.AbilityBoostRules).WithOne().HasForeignKey(item => item.BackgroundId).OnDelete(DeleteBehavior.Cascade);
        builder.HasMany(entity => entity.FeatGrants).WithOne().HasForeignKey(item => item.BackgroundId).OnDelete(DeleteBehavior.Cascade);
        builder.HasMany(entity => entity.SkillProficiencies).WithOne().HasForeignKey(item => item.BackgroundId).OnDelete(DeleteBehavior.Cascade);
        builder.HasMany(entity => entity.ToolProficiencies).WithOne().HasForeignKey(item => item.BackgroundId).OnDelete(DeleteBehavior.Cascade);
        builder.HasMany(entity => entity.StartingEquipmentRules).WithOne().HasForeignKey(item => item.BackgroundId).OnDelete(DeleteBehavior.Cascade);
        builder.HasMany(entity => entity.Features).WithOne().HasForeignKey(feature => feature.SourceEntityId).OnDelete(DeleteBehavior.Cascade);
        builder.HasIndex(entity => entity.Code).IsUnique().HasDatabaseName("ux_backgrounds_code");
        builder.HasIndex(entity => entity.SourceVersionId).HasDatabaseName("ix_backgrounds_source_version_id");
    }
}

internal sealed class FeatConfiguration : IEntityTypeConfiguration<Feat>
{
    public void Configure(EntityTypeBuilder<Feat> builder)
    {
        builder.ToTable("feats", CompendiumDbContext.Schema);
        builder.HasKey(entity => entity.Id).HasName("pk_feats");
        builder.Property(entity => entity.Id).HasConversion(OriginEfConversions.EntityId).ValueGeneratedNever().HasColumnName("id");
        builder.Property(entity => entity.RuleSourceId).HasConversion(OriginEfConversions.EntityId).HasColumnName("rule_source_id").IsRequired();
        builder.Property(entity => entity.SourceVersionId).HasConversion(OriginEfConversions.EntityId).HasColumnName("source_version_id").IsRequired();
        builder.Property(entity => entity.Code).HasConversion(OriginEfConversions.FeatCode).HasMaxLength(FeatCode.MaxLength).HasColumnName("code").IsRequired();
        builder.Property(entity => entity.Name).HasConversion(OriginEfConversions.FeatName).HasMaxLength(FeatName.MaxLength).HasColumnName("name").IsRequired();
        builder.Property(entity => entity.Description).HasConversion(OriginEfConversions.FeatDescription).HasMaxLength(FeatDescription.MaxLength).HasColumnName("description");
        builder.Property(entity => entity.Category).HasConversion<string>().HasMaxLength(40).HasColumnName("category").IsRequired();
        builder.Property(entity => entity.Repeatable).HasColumnName("repeatable");
        builder.Property(entity => entity.CreatedAtUtc).HasColumnName("created_at_utc");
        builder.Property(entity => entity.UpdatedAtUtc).HasColumnName("updated_at_utc");
        builder.HasMany(entity => entity.Features).WithOne().HasForeignKey(feature => feature.SourceEntityId).OnDelete(DeleteBehavior.Cascade);
        builder.HasIndex(entity => entity.Code).IsUnique().HasDatabaseName("ux_feats_code");
        builder.HasIndex(entity => entity.SourceVersionId).HasDatabaseName("ix_feats_source_version_id");
    }
}

internal abstract class BackgroundChildConfiguration<T> : IEntityTypeConfiguration<T> where T : class
{
    private readonly string tableName;
    protected BackgroundChildConfiguration(string tableName) => this.tableName = tableName;
    public void Configure(EntityTypeBuilder<T> builder)
    {
        builder.ToTable(tableName, CompendiumDbContext.Schema);
        ConfigureEntity(builder);
    }
    protected abstract void ConfigureEntity(EntityTypeBuilder<T> builder);
    protected void ConfigureKey(EntityTypeBuilder<T> builder, System.Linq.Expressions.Expression<Func<T, object?>> id,
        System.Linq.Expressions.Expression<Func<T, Compendium.Domain.SharedKernel.CompendiumEntityId>> backgroundId)
    {
        builder.HasKey(id).HasName($"pk_{tableName}");
        builder.Property(id).HasConversion(OriginEfConversions.EntityId).ValueGeneratedNever().HasColumnName("id");
        builder.Property(backgroundId).HasConversion(OriginEfConversions.EntityId).HasColumnName("background_id").IsRequired();
    }
}

internal sealed class BackgroundAbilityOptionConfiguration : BackgroundChildConfiguration<BackgroundAbilityOption>
{
    public BackgroundAbilityOptionConfiguration() : base("background_ability_options") { }
    protected override void ConfigureEntity(EntityTypeBuilder<BackgroundAbilityOption> builder)
    {
        ConfigureKey(builder, item => item.Id, item => item.BackgroundId);
        builder.Property(item => item.AbilityId).HasConversion(OriginEfConversions.EntityId).HasColumnName("ability_id").IsRequired();
        builder.Property(item => item.SortOrder).HasColumnName("sort_order");
        builder.HasIndex(item => new { item.BackgroundId, item.AbilityId }).IsUnique().HasDatabaseName("ux_background_ability_options_background_ability");
    }
}

internal sealed class BackgroundAbilityBoostRuleConfiguration : BackgroundChildConfiguration<BackgroundAbilityBoostRule>
{
    public BackgroundAbilityBoostRuleConfiguration() : base("background_ability_boost_rules") { }
    protected override void ConfigureEntity(EntityTypeBuilder<BackgroundAbilityBoostRule> builder)
    {
        ConfigureKey(builder, item => item.Id, item => item.BackgroundId);
        builder.Property(item => item.BoostAmount).HasColumnName("boost_amount");
        builder.Property(item => item.AbilityCount).HasColumnName("ability_count");
        builder.HasIndex(item => new { item.BackgroundId, item.BoostAmount }).IsUnique().HasDatabaseName("ux_background_ability_boost_rules_background_amount");
    }
}

internal sealed class BackgroundFeatGrantConfiguration : BackgroundChildConfiguration<BackgroundFeatGrant>
{
    public BackgroundFeatGrantConfiguration() : base("background_feat_grants") { }
    protected override void ConfigureEntity(EntityTypeBuilder<BackgroundFeatGrant> builder)
    {
        ConfigureKey(builder, item => item.Id, item => item.BackgroundId);
        builder.Property(item => item.FeatId).HasConversion(OriginEfConversions.EntityId).HasColumnName("feat_id").IsRequired();
        builder.HasIndex(item => new { item.BackgroundId, item.FeatId }).IsUnique().HasDatabaseName("ux_background_feat_grants_background_feat");
    }
}

internal sealed class BackgroundSkillProficiencyConfiguration : BackgroundChildConfiguration<BackgroundSkillProficiency>
{
    public BackgroundSkillProficiencyConfiguration() : base("background_skill_proficiencies") { }
    protected override void ConfigureEntity(EntityTypeBuilder<BackgroundSkillProficiency> builder)
    {
        ConfigureKey(builder, item => item.Id, item => item.BackgroundId);
        builder.Property(item => item.ProficiencyId).HasConversion(OriginEfConversions.EntityId).HasColumnName("proficiency_id").IsRequired();
        builder.HasIndex(item => new { item.BackgroundId, item.ProficiencyId }).IsUnique().HasDatabaseName("ux_background_skill_proficiencies_background_proficiency");
    }
}

internal sealed class BackgroundToolProficiencyConfiguration : BackgroundChildConfiguration<BackgroundToolProficiency>
{
    public BackgroundToolProficiencyConfiguration() : base("background_tool_proficiencies") { }
    protected override void ConfigureEntity(EntityTypeBuilder<BackgroundToolProficiency> builder)
    {
        ConfigureKey(builder, item => item.Id, item => item.BackgroundId);
        builder.Property(item => item.ProficiencyId).HasConversion(OriginEfConversions.EntityId).HasColumnName("proficiency_id").IsRequired();
        builder.HasIndex(item => new { item.BackgroundId, item.ProficiencyId }).IsUnique().HasDatabaseName("ux_background_tool_proficiencies_background_proficiency");
    }
}

internal sealed class BackgroundStartingEquipmentRuleConfiguration : BackgroundChildConfiguration<BackgroundStartingEquipmentRule>
{
    public BackgroundStartingEquipmentRuleConfiguration() : base("background_starting_equipment_rules") { }
    protected override void ConfigureEntity(EntityTypeBuilder<BackgroundStartingEquipmentRule> builder)
    {
        ConfigureKey(builder, item => item.Id, item => item.BackgroundId);
        builder.Property(item => item.ReferenceId).HasConversion(OriginEfConversions.EntityId).HasColumnName("reference_id").IsRequired();
        builder.Property(item => item.ReferenceType).HasConversion<string>().HasMaxLength(40).HasColumnName("reference_type").IsRequired();
        builder.HasIndex(item => new { item.BackgroundId, item.ReferenceId, item.ReferenceType }).IsUnique().HasDatabaseName("ux_background_starting_equipment_rules_reference");
    }
}
