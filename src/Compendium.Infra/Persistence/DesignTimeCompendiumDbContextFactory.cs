using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Compendium.Infra.Persistence;

public sealed class DesignTimeCompendiumDbContextFactory : IDesignTimeDbContextFactory<CompendiumDbContext>
{
    public CompendiumDbContext CreateDbContext(string[] args)
    {
        var connectionString =
            Environment.GetEnvironmentVariable("ConnectionStrings__CompendiumDb") ??
            Environment.GetEnvironmentVariable("COMPENDIUM_DB_CONNECTION") ??
            CompendiumDatabaseOptions.DefaultLocalConnectionString;

        var options = new DbContextOptionsBuilder<CompendiumDbContext>()
            .UseNpgsql(
                connectionString,
                npgsql => npgsql.MigrationsHistoryTable(
                    CompendiumDbContext.MigrationsHistoryTable,
                    CompendiumDbContext.Schema))
            .Options;

        return new CompendiumDbContext(options);
    }
}
