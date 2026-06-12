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
    }

    [Fact]
    public void Initial_migration_is_registered_for_code_first_pipeline()
    {
        using var dbContext = CreateContext();

        var migrations = dbContext.GetService<IMigrationsAssembly>().Migrations;

        Assert.Contains("20260611210000_InitialCompendiumSchema", migrations.Keys);
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
