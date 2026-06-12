using Compendium.Infra.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Compendium.Infra;

public static class InfrastructureDependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString(CompendiumDatabaseOptions.ConnectionStringName);

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            connectionString = CompendiumDatabaseOptions.DefaultLocalConnectionString;
        }

        services.Configure<CompendiumDatabaseOptions>(
            configuration.GetSection(CompendiumDatabaseOptions.SectionName));

        services.AddDbContext<CompendiumDbContext>(options =>
            options.UseNpgsql(
                connectionString,
                npgsql => npgsql.MigrationsHistoryTable(
                    CompendiumDbContext.MigrationsHistoryTable,
                    CompendiumDbContext.Schema)));

        return services;
    }
}
