using Compendium.Domain.Features;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Compendium.Infra.Persistence.Features;

internal sealed class FeatureConfiguration : IEntityTypeConfiguration<Feature>
{
    public void Configure(EntityTypeBuilder<Feature> builder)
    {
        builder.ToTable("features", CompendiumDbContext.Schema);
        builder
.HasKey(feature => feature.Id)
.HasName("pk_features");
        builder.Property(feature => feature.Id)
.HasConversion(FeatureEfConversions.EntityId)
.ValueGeneratedNever()
.HasColumnName("id");
        builder.Property(feature => feature.RuleSourceId)
.HasConversion(FeatureEfConversions.EntityId)
.HasColumnName("rule_source_id")
.IsRequired();
        builder.Property(feature => feature.SourceVersionId)
.HasConversion(FeatureEfConversions.EntityId)
.HasColumnName("source_version_id")
.IsRequired();
        builder.Property(feature => feature.Code)
.HasConversion(FeatureEfConversions.FeatureCode)
.HasMaxLength(FeatureCode.MaxLength)
.HasColumnName("code")
.IsRequired();
        builder.Property(feature => feature.Name)
.HasConversion(FeatureEfConversions.FeatureName)
.HasMaxLength(FeatureName.MaxLength)
.HasColumnName("name")
.IsRequired();
        builder.Property(feature => feature.Description)
.HasConversion(FeatureEfConversions.NullableFeatureDescription)
.HasMaxLength(FeatureDescription.MaxLength)
.HasColumnName("description");
        builder.Property(feature => feature.LevelRequirement)
.HasColumnName("level_requirement");
        builder.Property(feature => feature.CreatedAtUtc)
.HasColumnName("created_at_utc");
        builder.Property(feature => feature.UpdatedAtUtc)
.HasColumnName("updated_at_utc");
        builder
.HasMany(feature => feature.Effects)
.WithOne()
.HasForeignKey(effect => effect.FeatureId)
.OnDelete(DeleteBehavior.Cascade);
        builder
.HasIndex(feature => feature.Code)
.IsUnique()
.HasDatabaseName("ux_features_code");
        builder
.HasIndex(feature => feature.SourceVersionId)
.HasDatabaseName("ix_features_source_version_id");
    }
}

internal sealed class FeatureLinkConfiguration<TLink> : IEntityTypeConfiguration<TLink>
    where TLink : FeatureLink
{
    private readonly string tableName;
    private readonly string sourceColumnName;

    public FeatureLinkConfiguration(string tableName, string sourceColumnName)
    {
        this.tableName = tableName;
        this.sourceColumnName = sourceColumnName;
    }

    public void Configure(EntityTypeBuilder<TLink> builder)
    {
        builder.ToTable(tableName, CompendiumDbContext.Schema);
        builder
.HasKey(link => link.Id)
.HasName($"pk_{tableName}");
        builder.Property(link => link.Id)
.HasConversion(FeatureEfConversions.EntityId)
.ValueGeneratedNever()
.HasColumnName("id");
        builder.Property(link => link.SourceEntityId)
.HasConversion(FeatureEfConversions.EntityId)
.HasColumnName(sourceColumnName)
.IsRequired();
        builder.Property(link => link.FeatureId)
.HasConversion(FeatureEfConversions.EntityId)
.HasColumnName("feature_id")
.IsRequired();
        builder.Property(link => link.SourceVersionId)
.HasConversion(FeatureEfConversions.EntityId)
.HasColumnName("source_version_id")
.IsRequired();
        builder.Property(link => link.Level)
.HasColumnName("level");
        builder
.HasIndex(link => new { link.SourceEntityId, link.FeatureId, link.Level })
.IsUnique()
.HasDatabaseName($"ux_{tableName}_source_feature_level");
    }
}

internal sealed class EffectSchemaConfiguration : IEntityTypeConfiguration<EffectSchema>
{
    public void Configure(EntityTypeBuilder<EffectSchema> builder)
    {
        builder.ToTable("effect_schemas", CompendiumDbContext.Schema);
        builder
.HasKey(schema => schema.Id)
.HasName("pk_effect_schemas");
        builder.Property(schema => schema.Id)
.HasConversion(FeatureEfConversions.EntityId)
.ValueGeneratedNever()
.HasColumnName("id");
        builder.Property(schema => schema.Code)
.HasConversion(FeatureEfConversions.FeatureCode)
.HasMaxLength(FeatureCode.MaxLength)
.HasColumnName("code")
.IsRequired();
        builder.Property(schema => schema.Name)
.HasConversion(FeatureEfConversions.FeatureName)
.HasMaxLength(FeatureName.MaxLength)
.HasColumnName("name")
.IsRequired();
        builder.Property(schema => schema.Type)
.HasConversion<string>()
.HasMaxLength(80)
.HasColumnName("type")
.IsRequired();
        builder
.HasMany(schema => schema.Fields)
.WithOne()
.HasForeignKey(field => field.EffectSchemaId)
.OnDelete(DeleteBehavior.Cascade);
        builder
.HasIndex(schema => schema.Code)
.IsUnique()
.HasDatabaseName("ux_effect_schemas_code");
    }
}

internal sealed class EffectSchemaFieldConfiguration : IEntityTypeConfiguration<EffectSchemaField>
{
    public void Configure(EntityTypeBuilder<EffectSchemaField> builder)
    {
        builder.ToTable("effect_schema_fields", CompendiumDbContext.Schema);
        builder
.HasKey(field => field.Id)
.HasName("pk_effect_schema_fields");
        builder.Property(field => field.Id)
.HasConversion(FeatureEfConversions.EntityId)
.ValueGeneratedNever()
.HasColumnName("id");
        builder.Property(field => field.EffectSchemaId)
.HasConversion(FeatureEfConversions.EntityId)
.HasColumnName("effect_schema_id")
.IsRequired();
        builder.Property(field => field.Code)
.HasConversion(FeatureEfConversions.FeatureCode)
.HasMaxLength(FeatureCode.MaxLength)
.HasColumnName("code")
.IsRequired();
        builder.Property(field => field.ValueType)
.HasConversion<string>()
.HasMaxLength(80)
.HasColumnName("value_type")
.IsRequired();
        builder.Property(field => field
.IsRequired)
.HasColumnName("is_required");
        builder.Property(field => field.SortOrder)
.HasColumnName("sort_order");
        builder
.HasIndex(field => new { field.EffectSchemaId, field.Code })
.IsUnique()
.HasDatabaseName("ux_effect_schema_fields_schema_code");
    }
}

internal sealed class FeatureEffectConfiguration : IEntityTypeConfiguration<FeatureEffect>
{
    public void Configure(EntityTypeBuilder<FeatureEffect> builder)
    {
        builder.ToTable("feature_effects", CompendiumDbContext.Schema);
        builder
.HasKey(effect => effect.Id)
.HasName("pk_feature_effects");
        builder.Property(effect => effect.Id)
.HasConversion(FeatureEfConversions.EntityId)
.ValueGeneratedNever()
.HasColumnName("id");
        builder.Property(effect => effect.FeatureId)
.HasConversion(FeatureEfConversions.EntityId)
.HasColumnName("feature_id")
.IsRequired();
        builder.Property(effect => effect.EffectSchemaId)
.HasConversion(FeatureEfConversions.EntityId)
.HasColumnName("effect_schema_id")
.IsRequired();
        builder.Property(effect => effect.Type)
.HasConversion<string>()
.HasMaxLength(80)
.HasColumnName("type")
.IsRequired();
        builder.Property(effect => effect.Target)
.HasConversion<string>()
.HasMaxLength(80)
.HasColumnName("target")
.IsRequired();
        builder.Property(effect => effect.CreatedAtUtc)
.HasColumnName("created_at_utc");
        builder
.HasMany(effect => effect.FieldValues)
.WithOne()
.HasForeignKey(value => value.FeatureEffectId)
.OnDelete(DeleteBehavior.Cascade);
        builder
.HasMany(effect => effect.Conditions)
.WithOne()
.HasForeignKey(condition => condition.FeatureEffectId)
.OnDelete(DeleteBehavior.Cascade);
        builder
.HasIndex(effect => effect.FeatureId)
.HasDatabaseName("ix_feature_effects_feature_id");
    }
}

internal sealed class FeatureEffectFieldValueConfiguration : IEntityTypeConfiguration<FeatureEffectFieldValue>
{
    public void Configure(EntityTypeBuilder<FeatureEffectFieldValue> builder)
    {
        builder.ToTable("feature_effect_field_values", CompendiumDbContext.Schema);
        builder
.HasKey(value => value.Id)
.HasName("pk_feature_effect_field_values");
        builder.Property(value => value.Id)
.HasConversion(FeatureEfConversions.EntityId)
.ValueGeneratedNever()
.HasColumnName("id");
        builder.Property(value => value.FeatureEffectId)
.HasConversion(FeatureEfConversions.EntityId)
.HasColumnName("feature_effect_id")
.IsRequired();
        builder.Property(value => value.EffectSchemaFieldId)
.HasConversion(FeatureEfConversions.EntityId)
.HasColumnName("effect_schema_field_id")
.IsRequired();
        builder.OwnsOne(value => value.Value, ConfigureTypedValue);
        builder.Navigation(value => value.Value)
.IsRequired();
        builder
.HasIndex(value => new { value.FeatureEffectId, value.EffectSchemaFieldId })
.IsUnique()
.HasDatabaseName("ux_feature_effect_field_values_effect_field");
    }

    private static void ConfigureTypedValue(OwnedNavigationBuilder<FeatureEffectFieldValue, TypedMechanicalValue> value)
    {
        TypedValueColumns.Configure(value);
    }
}

internal sealed class FeatureEffectConditionConfiguration : IEntityTypeConfiguration<FeatureEffectCondition>
{
    public void Configure(EntityTypeBuilder<FeatureEffectCondition> builder)
    {
        builder.ToTable("feature_effect_conditions", CompendiumDbContext.Schema);
        builder
.HasKey(condition => condition.Id)
.HasName("pk_feature_effect_conditions");
        builder.Property(condition => condition.Id)
.HasConversion(FeatureEfConversions.EntityId)
.ValueGeneratedNever()
.HasColumnName("id");
        builder.Property(condition => condition.FeatureEffectId)
.HasConversion(FeatureEfConversions.EntityId)
.HasColumnName("feature_effect_id")
.IsRequired();
        builder.Property(condition => condition.Type)
.HasConversion<string>()
.HasMaxLength(80)
.HasColumnName("type")
.IsRequired();
        builder.OwnsOne(condition => condition.Value, TypedValueColumns.Configure);
        builder.Navigation(condition => condition.Value)
.IsRequired();
    }
}

internal sealed class EntityPrerequisiteConfiguration : IEntityTypeConfiguration<EntityPrerequisite>
{
    public void Configure(EntityTypeBuilder<EntityPrerequisite> builder)
    {
        builder.ToTable("entity_prerequisites", CompendiumDbContext.Schema);
        builder
.HasKey(prerequisite => prerequisite.Id)
.HasName("pk_entity_prerequisites");
        builder.Property(prerequisite => prerequisite.Id)
.HasConversion(FeatureEfConversions.EntityId)
.ValueGeneratedNever()
.HasColumnName("id");
        builder.Property(prerequisite => prerequisite.EntityKind)
.HasConversion<string>()
.HasMaxLength(80)
.HasColumnName("entity_kind")
.IsRequired();
        builder.Property(prerequisite => prerequisite.EntityId)
.HasConversion(FeatureEfConversions.EntityId)
.HasColumnName("entity_id")
.IsRequired();
        builder.Property(prerequisite => prerequisite.Type)
.HasConversion<string>()
.HasMaxLength(80)
.HasColumnName("type")
.IsRequired();
        builder.Property(prerequisite => prerequisite.Operator)
.HasConversion<string>()
.HasMaxLength(80)
.HasColumnName("operator")
.IsRequired();
        builder.Property(prerequisite => prerequisite.Target)
.HasConversion<string>()
.HasMaxLength(80)
.HasColumnName("target")
.IsRequired();
        builder.OwnsOne(prerequisite => prerequisite.Value, TypedValueColumns.Configure);
        builder.Navigation(prerequisite => prerequisite.Value)
.IsRequired();
        builder
.HasIndex(prerequisite => new { prerequisite.EntityKind, prerequisite.EntityId })
.HasDatabaseName("ix_entity_prerequisites_entity");
    }
}

internal sealed class ChoiceSetConfiguration : IEntityTypeConfiguration<ChoiceSet>
{
    public void Configure(EntityTypeBuilder<ChoiceSet> builder)
    {
        builder.ToTable("choice_sets", CompendiumDbContext.Schema);
        builder
.HasKey(choiceSet => choiceSet.Id)
.HasName("pk_choice_sets");
        builder.Property(choiceSet => choiceSet.Id)
.HasConversion(FeatureEfConversions.EntityId)
.ValueGeneratedNever()
.HasColumnName("id");
        builder.Property(choiceSet => choiceSet.SourceEntityKind)
.HasConversion<string>()
.HasMaxLength(80)
.HasColumnName("source_entity_kind")
.IsRequired();
        builder.Property(choiceSet => choiceSet.SourceEntityId)
.HasConversion(FeatureEfConversions.EntityId)
.HasColumnName("source_entity_id")
.IsRequired();
        builder.Property(choiceSet => choiceSet.Code)
.HasConversion(FeatureEfConversions.ChoiceSetCode)
.HasMaxLength(ChoiceSetCode.MaxLength)
.HasColumnName("code")
.IsRequired();
        builder.Property(choiceSet => choiceSet.MinimumChoices)
.HasColumnName("minimum_choices");
        builder.Property(choiceSet => choiceSet.MaximumChoices)
.HasColumnName("maximum_choices");
        builder
.HasMany(choiceSet => choiceSet.Filters)
.WithOne()
.HasForeignKey(filter => filter.ChoiceSetId)
.OnDelete(DeleteBehavior.Cascade);
        builder
.HasMany(choiceSet => choiceSet.Options)
.WithOne()
.HasForeignKey(option => option.ChoiceSetId)
.OnDelete(DeleteBehavior.Cascade);
        builder
.HasIndex(choiceSet => choiceSet.Code)
.IsUnique()
.HasDatabaseName("ux_choice_sets_code");
        builder
.HasIndex(choiceSet => new { choiceSet.SourceEntityKind, choiceSet.SourceEntityId })
.HasDatabaseName("ix_choice_sets_source_entity");
    }
}

internal sealed class ChoiceSetFilterConfiguration : IEntityTypeConfiguration<ChoiceSetFilter>
{
    public void Configure(EntityTypeBuilder<ChoiceSetFilter> builder)
    {
        builder.ToTable("choice_set_filters", CompendiumDbContext.Schema);
        builder
.HasKey(filter => filter.Id)
.HasName("pk_choice_set_filters");
        builder.Property(filter => filter.Id)
.HasConversion(FeatureEfConversions.EntityId)
.ValueGeneratedNever()
.HasColumnName("id");
        builder.Property(filter => filter.ChoiceSetId)
.HasConversion(FeatureEfConversions.EntityId)
.HasColumnName("choice_set_id")
.IsRequired();
        builder.Property(filter => filter.Type)
.HasConversion<string>()
.HasMaxLength(80)
.HasColumnName("type")
.IsRequired();
        builder.OwnsOne(filter => filter.Value, TypedValueColumns.Configure);
        builder.Navigation(filter => filter.Value)
.IsRequired();
    }
}

internal sealed class ChoiceOptionConfiguration : IEntityTypeConfiguration<ChoiceOption>
{
    public void Configure(EntityTypeBuilder<ChoiceOption> builder)
    {
        builder.ToTable("choice_options", CompendiumDbContext.Schema);
        builder
.HasKey(option => option.Id)
.HasName("pk_choice_options");
        builder.Property(option => option.Id)
.HasConversion(FeatureEfConversions.EntityId)
.ValueGeneratedNever()
.HasColumnName("id");
        builder.Property(option => option.ChoiceSetId)
.HasConversion(FeatureEfConversions.EntityId)
.HasColumnName("choice_set_id")
.IsRequired();
        builder.Property(option => option.Type)
.HasConversion<string>()
.HasMaxLength(80)
.HasColumnName("type")
.IsRequired();
        builder.Property(option => option.ReferenceId)
.HasConversion(FeatureEfConversions.NullableEntityId)
.HasColumnName("ref_id");
        builder.Property(option => option.DisplayText)
.HasMaxLength(500)
.HasColumnName("display_text");
        builder.Property(option => option.SortOrder)
.HasColumnName("sort_order");
    }
}

file static class TypedValueColumns
{
    public static void Configure<TOwner>(OwnedNavigationBuilder<TOwner, TypedMechanicalValue> value)
        where TOwner : class
    {
        value.Property(v => v.ValueType)
.HasConversion<string>()
.HasMaxLength(80)
.HasColumnName("value_type");
        value.Property(v => v.TextValue)
.HasMaxLength(1000)
.HasColumnName("text_value");
        value.Property(v => v.NumericValue)
.HasColumnName("numeric_value");
        value.Property(v => v.BooleanValue)
.HasColumnName("boolean_value");
        value.Property(v => v.ReferenceId)
.HasConversion(FeatureEfConversions.NullableEntityId)
.HasColumnName("ref_id");
        value.Property(v => v.EnumValue)
.HasMaxLength(120)
.HasColumnName("enum_value");
    }
}
