using Compendium.Infra.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

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
    }

    [Fact]
    public void Initial_migration_is_registered_for_code_first_pipeline()
    {
        using var dbContext = CreateContext();

        var migrations = dbContext.GetService<IMigrationsAssembly>().Migrations;

        Assert.Contains("20260611210000_InitialCompendiumSchema", migrations.Keys);
        Assert.Contains("20260612005038_AddSourcesRulesetsAndVersions", migrations.Keys);
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

    private static CompendiumDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<CompendiumDbContext>()
            .UseNpgsql(CompendiumDatabaseOptions.DefaultLocalConnectionString)
            .Options;

        return new CompendiumDbContext(options);
    }
}
