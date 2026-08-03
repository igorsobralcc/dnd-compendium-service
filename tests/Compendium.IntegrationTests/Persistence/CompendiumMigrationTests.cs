using Compendium.Infra.Persistence;
using Compendium.Domain.Classes;
using Compendium.Domain.Translations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Compendium.Infra.Persistence.InternalQueries;

namespace Compendium.IntegrationTests.Persistence;

public sealed class CompendiumMigrationTests
{
    [Fact]
    public void Initial_migration_registers_compendium_schema_and_technical_tables()
    {
        using var dbContext = CreateContext();

        var tables = dbContext.Model.GetEntityTypes()
            .Select(entityType => (Schema: entityType.GetSchema(), Table: entityType.GetTableName()))
            .ToHashSet();

        Assert.Contains((CompendiumDbContext.Schema, "integration_outbox"), tables);
        Assert.Contains((CompendiumDbContext.Schema, "integration_outbox_fields"), tables);
        Assert.Contains((CompendiumDbContext.Schema, "integration_inbox"), tables);
        Assert.Contains((CompendiumDbContext.Schema, "rulesets"), tables);
        Assert.Contains((CompendiumDbContext.Schema, "rule_sources"), tables);
        Assert.Contains((CompendiumDbContext.Schema, "source_versions"), tables);
        Assert.Contains((CompendiumDbContext.Schema, "abilities"), tables);
        Assert.Contains((CompendiumDbContext.Schema, "skills"), tables);
        Assert.Contains((CompendiumDbContext.Schema, "languages"), tables);
        Assert.Contains((CompendiumDbContext.Schema, "proficiencies"), tables);
        Assert.Contains((CompendiumDbContext.Schema, "armor_training_categories"), tables);
        Assert.Contains((CompendiumDbContext.Schema, "hit_dice"), tables);
        Assert.Contains((CompendiumDbContext.Schema, "ability_score_methods"), tables);
        Assert.Contains((CompendiumDbContext.Schema, "ability_score_method_rules"), tables);
        Assert.Contains((CompendiumDbContext.Schema, "ability_score_standard_values"), tables);
        Assert.Contains((CompendiumDbContext.Schema, "ability_score_point_buy_costs"), tables);
        Assert.Contains((CompendiumDbContext.Schema, "ability_score_roll_rules"), tables);
        Assert.Contains((CompendiumDbContext.Schema, "classes"), tables);
        Assert.Contains((CompendiumDbContext.Schema, "class_core_traits"), tables);
        Assert.Contains((CompendiumDbContext.Schema, "class_primary_abilities"), tables);
        Assert.Contains((CompendiumDbContext.Schema, "class_levels"), tables);
        Assert.Contains((CompendiumDbContext.Schema, "class_level_spell_slots"), tables);
        Assert.Contains((CompendiumDbContext.Schema, "class_proficiency_grants"), tables);
        Assert.Contains((CompendiumDbContext.Schema, "class_weapon_mastery_count_by_level"), tables);
        Assert.Contains((CompendiumDbContext.Schema, "class_spellcasting_progressions"), tables);
        Assert.Contains((CompendiumDbContext.Schema, "class_spellcasting_level_rules"), tables);
        Assert.Contains((CompendiumDbContext.Schema, "subclasses"), tables);
        Assert.Contains((CompendiumDbContext.Schema, "subclass_features"), tables);
        Assert.Contains((CompendiumDbContext.Schema, "translations"), tables);
        Assert.Contains((CompendiumDbContext.Schema, "compendium_changes"), tables);
    }

    [Fact]
    public void Initial_migration_is_registered_for_code_first_pipeline()
    {
        using var dbContext = CreateContext();

        var migrations = dbContext.GetService<IMigrationsAssembly>().Migrations;

        Assert.Contains("20260611210000_InitialCompendiumSchema", migrations.Keys);
        Assert.Contains("20260612005038_AddSourcesRulesetsAndVersions", migrations.Keys);
        Assert.Contains("20260612020556_AddFundamentalRuleData", migrations.Keys);
        Assert.Contains("20260612022105_AddAbilityScoreMethods", migrations.Keys);
        Assert.Contains("20260612024509_AddClassesAndSubclasses", migrations.Keys);
        Assert.Contains("20260725220037_AddTranslationsEpic", migrations.Keys);
        Assert.Contains(migrations.Keys, key => key.EndsWith("_AddInternalQueryApisEpic", StringComparison.Ordinal));
        Assert.Contains("20260803090000_AddOutboxPerformanceIndexes", migrations.Keys);
        Assert.Contains(migrations.Keys, key => key.EndsWith("_AddOutboxConcurrentClaims", StringComparison.Ordinal));
    }

    [Fact]
    public void Source_model_enforces_ruleset_source_and_current_version_uniqueness()
    {
        using var dbContext = CreateContext();

        var rulesetIndexes = dbContext.Model.FindEntityType(typeof(Compendium.Domain.Sources.Ruleset))!.GetIndexes();
        var sourceIndexes = dbContext.Model.FindEntityType(typeof(Compendium.Domain.Sources.RuleSource))!.GetIndexes();
        var versionIndexes = dbContext.Model.FindEntityType(typeof(Compendium.Domain.Sources.SourceVersion))!.GetIndexes();

        Assert.Contains(rulesetIndexes, index => index.IsUnique && index.GetDatabaseName() == "ux_rulesets_code");
        Assert.Contains(sourceIndexes, index => index.IsUnique && index.GetDatabaseName() == "ux_rule_sources_ruleset_code");
        Assert.Contains(versionIndexes, index => index.IsUnique && index.GetDatabaseName() == "ux_source_versions_current_per_source");
    }

    [Fact]
    public void Fundamental_model_enforces_reusable_code_uniqueness()
    {
        using var dbContext = CreateContext();

        var abilityIndexes = dbContext.Model.FindEntityType(typeof(Compendium.Domain.Fundamentals.Ability))!.GetIndexes();
        var skillIndexes = dbContext.Model.FindEntityType(typeof(Compendium.Domain.Fundamentals.Skill))!.GetIndexes();
        var languageIndexes = dbContext.Model.FindEntityType(typeof(Compendium.Domain.Fundamentals.Language))!.GetIndexes();
        var proficiencyIndexes = dbContext.Model.FindEntityType(typeof(Compendium.Domain.Fundamentals.Proficiency))!.GetIndexes();
        var hitDieIndexes = dbContext.Model.FindEntityType(typeof(Compendium.Domain.Fundamentals.HitDie))!.GetIndexes();
        var abilityScoreMethodIndexes = dbContext.Model.FindEntityType(typeof(Compendium.Domain.Fundamentals.AbilityScoreMethod))!.GetIndexes();
        var pointBuyCostIndexes = dbContext.Model.FindEntityType(typeof(Compendium.Domain.Fundamentals.AbilityScorePointBuyCost))!.GetIndexes();
        var rollRuleIndexes = dbContext.Model.FindEntityType(typeof(Compendium.Domain.Fundamentals.AbilityScoreRollRule))!.GetIndexes();

        Assert.Contains(abilityIndexes, index => index.IsUnique && index.GetDatabaseName() == "ux_abilities_code");
        Assert.Contains(skillIndexes, index => index.IsUnique && index.GetDatabaseName() == "ux_skills_code");
        Assert.Contains(languageIndexes, index => index.IsUnique && index.GetDatabaseName() == "ux_languages_code");
        Assert.Contains(proficiencyIndexes, index => index.IsUnique && index.GetDatabaseName() == "ux_proficiencies_code");
        Assert.Contains(hitDieIndexes, index => index.IsUnique && index.GetDatabaseName() == "ux_hit_dice_die");
        Assert.Contains(abilityScoreMethodIndexes, index => index.IsUnique && index.GetDatabaseName() == "ux_ability_score_methods_code");
        Assert.Contains(pointBuyCostIndexes, index => index.IsUnique && index.GetDatabaseName() == "ux_ability_score_point_buy_costs_method_score");
        Assert.Contains(rollRuleIndexes, index => index.IsUnique && index.GetDatabaseName() == "ux_ability_score_roll_rules_method");
    }

    [Fact]
    public void Class_model_enforces_progression_and_subclass_uniqueness()
    {
        using var dbContext = CreateContext();

        var classIndexes = dbContext.Model.FindEntityType(typeof(CharacterClass))!.GetIndexes();
        var levelIndexes = dbContext.Model.FindEntityType(typeof(ClassLevel))!.GetIndexes();
        var primaryAbilityIndexes = dbContext.Model.FindEntityType(typeof(ClassPrimaryAbility))!.GetIndexes();
        var spellSlotIndexes = dbContext.Model.FindEntityType(typeof(ClassLevelSpellSlot))!.GetIndexes();
        var subclassIndexes = dbContext.Model.FindEntityType(typeof(CharacterSubclass))!.GetIndexes();
        var subclassFeatureIndexes = dbContext.Model.FindEntityType(typeof(SubclassFeature))!.GetIndexes();

        Assert.Contains(classIndexes, index => index.IsUnique && index.GetDatabaseName() == "ux_classes_code");
        Assert.Contains(levelIndexes, index => index.IsUnique && index.GetDatabaseName() == "ux_class_levels_class_level");
        Assert.Contains(primaryAbilityIndexes, index => index.IsUnique && index.GetDatabaseName() == "ux_class_primary_abilities_class_ability");
        Assert.Contains(spellSlotIndexes, index => index.IsUnique && index.GetDatabaseName() == "ux_class_level_spell_slots_level_spell_level");
        Assert.Contains(subclassIndexes, index => index.IsUnique && index.GetDatabaseName() == "ux_subclasses_class_code");
        Assert.Contains(subclassFeatureIndexes, index => index.IsUnique && index.GetDatabaseName() == "ux_subclass_features_subclass_feature_level");
    }

    [Fact]
    public void Technical_model_does_not_use_json_columns()
    {
        using var dbContext = CreateContext();

        var jsonColumns = dbContext.Model.GetEntityTypes()
            .SelectMany(entityType => entityType.GetProperties())
            .Where(property =>
                property.GetColumnType()?.Contains("json", StringComparison.OrdinalIgnoreCase) == true)
            .Select(property => property.Name)
            .ToArray();

        Assert.Empty(jsonColumns);
    }

    [Fact]
    public void Translation_model_enforces_one_value_per_entity_locale_and_field()
    {
        using var dbContext = CreateContext();

        var indexes = dbContext.Model.FindEntityType(typeof(Translation))!.GetIndexes();

        Assert.Contains(indexes, index =>
            index.IsUnique &&
            index.GetDatabaseName() == "ux_translations_entity_locale_field");
    }

    [Fact]
    public void Change_feed_is_relational_and_indexed_for_revision_queries()
    {
        using var dbContext = CreateContext();
        var entity = dbContext.Model.FindEntityType(typeof(CompendiumChange))!;

        Assert.Equal("revision", entity.FindPrimaryKey()!.Properties.Single().GetColumnName());
        Assert.Contains(entity.GetIndexes(), index =>
            index.GetDatabaseName() == "ix_compendium_changes_source_revision");
        Assert.Contains(entity.GetIndexes(), index =>
            index.GetDatabaseName() == "ix_compendium_changes_type_revision");
    }

    [Fact]
    public void Outbox_model_has_active_and_retention_indexes()
    {
        using var dbContext = CreateContext();
        var entity = dbContext.Model.FindEntityType(typeof(Compendium.Infra.Persistence.Integration.IntegrationOutbox))!;

        Assert.Contains(entity.GetIndexes(), index =>
            index.GetDatabaseName() == "ix_integration_outbox_active_available_created"
            && index.GetFilter() == "status IN ('PENDING', 'FAILED')");
        Assert.Contains(entity.GetIndexes(), index =>
            index.GetDatabaseName() == "ix_integration_outbox_published_at"
            && index.GetFilter() == "status = 'PUBLISHED'");
        Assert.Contains(entity.GetIndexes(), index =>
            index.GetDatabaseName() == "ix_integration_outbox_processing_lease"
            && index.GetFilter() == "status = 'PROCESSING'");
        Assert.True(entity.FindProperty("ClaimToken")!.IsConcurrencyToken);
    }

    private static CompendiumDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<CompendiumDbContext>()
            .UseNpgsql(CompendiumDatabaseOptions.DefaultLocalConnectionString)
            .Options;

        return new CompendiumDbContext(options);
    }
}
