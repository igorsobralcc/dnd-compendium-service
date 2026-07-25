using Compendium.Application.Classes;
using Compendium.Application.Features;
using Compendium.Application.Equipment;
using Compendium.Application.Fundamentals;
using Compendium.Application.Sources;
using Compendium.Application.Translations;
using Compendium.Application.Importing;
using Compendium.Infra.Persistence;
using Compendium.Infra.Persistence.Classes;
using Compendium.Infra.Persistence.Features;
using Compendium.Infra.Persistence.Equipment;
using Compendium.Infra.Persistence.Fundamentals;
using Compendium.Infra.Persistence.Sources;
using Compendium.Infra.Persistence.Translations;
using Compendium.Infra.Persistence.Importing;
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
        services.AddScoped<IAbilityScoreMethodRepository, AbilityScoreMethodRepository>();
        services.AddScoped<ICharacterClassRepository, CharacterClassRepository>();
        services.AddScoped<ICharacterSubclassRepository, CharacterSubclassRepository>();
        services.AddScoped<IFeatureRepository, FeatureRepository>();
        services.AddScoped<IEffectSchemaRepository, EffectSchemaRepository>();
        services.AddScoped<IEntityPrerequisiteRepository, EntityPrerequisiteRepository>();
        services.AddScoped<IChoiceSetRepository, ChoiceSetRepository>();
        services.AddScoped<IEquipmentRepository, EquipmentRepository>();
        services.AddScoped<IWeaponRepository, WeaponRepository>();
        services.AddScoped<IWeaponPropertyRepository, WeaponPropertyRepository>();
        services.AddScoped<IWeaponMasteryRepository, WeaponMasteryRepository>();
        services.AddScoped<IArmorRepository, ArmorRepository>();
        services.AddScoped<IToolRepository, ToolRepository>();
        services.AddScoped<IEquipmentPackRepository, EquipmentPackRepository>();
        services.AddScoped<IStartingEquipmentRuleRepository, StartingEquipmentRuleRepository>();
        services.AddScoped<ITranslationRepository, TranslationRepository>();
        services.AddSingleton(TimeProvider.System);
        services.AddScoped<SourceVersionImportGateway>();
        services.AddScoped<ISourceVersionImportGateway>(provider => provider.GetRequiredService<SourceVersionImportGateway>());
        services.AddScoped<ISourceVersionValidationGateway>(provider => provider.GetRequiredService<SourceVersionImportGateway>());

        return services;
    }
}
