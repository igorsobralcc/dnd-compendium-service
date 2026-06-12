using Compendium.Application.Fundamentals;
using Compendium.Application.Sources;
using Compendium.Infra.Persistence;
using Compendium.Infra.Persistence.Fundamentals;
using Compendium.Infra.Persistence.Sources;
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

        services.AddScoped<IRulesetRepository, RulesetRepository>();
        services.AddScoped<IRuleSourceRepository, RuleSourceRepository>();
        services.AddScoped<ISourceVersionRepository, SourceVersionRepository>();
        services.AddScoped<IAbilityRepository, AbilityRepository>();
        services.AddScoped<ISkillRepository, SkillRepository>();
        services.AddScoped<ILanguageRepository, LanguageRepository>();
        services.AddScoped<IProficiencyRepository, ProficiencyRepository>();
        services.AddScoped<IArmorTrainingCategoryRepository, ArmorTrainingCategoryRepository>();
        services.AddScoped<IHitDieRepository, HitDieRepository>();

        return services;
    }
}
