using System.Reflection;
using System.Runtime.CompilerServices;
using Compendium.Application;
using Compendium.Application.Classes;
using Compendium.Application.Equipment;
using Compendium.Application.Features;
using Compendium.Application.Fundamentals;
using Compendium.Application.Importing;
using Compendium.Application.Integration;
using Compendium.Application.InternalQueries;
using Compendium.Application.Sources;
using Compendium.Application.Translations;
using Compendium.Domain.Importing;
using Compendium.Domain.SharedKernel;
using Compendium.Infra.Integration;
using Compendium.Infra.Observability;
using Compendium.Infra.Persistence;
using Compendium.Infra.Persistence.Classes;
using Compendium.Infra.Persistence.Equipment;
using Compendium.Infra.Persistence.Features;
using Compendium.Infra.Persistence.Fundamentals;
using Compendium.Infra.Persistence.Importing;
using Compendium.Infra.Persistence.InternalQueries;
using Compendium.Infra.Persistence.Sources;
using Compendium.Infra.Persistence.Translations;
using Compendium.Application.Observability;
using Compendium.CrossCutting.Http;
using Compendium.CrossCutting.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace Compendium.CrossCutting;

public static class CompendiumServiceCollectionExtensions
{
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static IServiceCollection AddCompendiumServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var applicationAssembly = Assembly.GetCallingAssembly();
        AddApplicationServices(services);
        AddInfrastructureServices(services, configuration);
        AddHttpServices(services, configuration, applicationAssembly);

        return services;
    }

    private static void AddHttpServices(
        IServiceCollection services,
        IConfiguration configuration,
        Assembly applicationAssembly)
    {
        var mvc = services.AddControllers();
        if (applicationAssembly.GetName().Name?.EndsWith(
                ".API",
                StringComparison.Ordinal) == true)
        {
            AddMvcApplicationPart(mvc, applicationAssembly);
        }
        services.AddProblemDetails();
        services.AddExceptionHandler<CompendiumExceptionHandler>();
        services.AddCompendiumSecurity(configuration);
        services.AddOpenTelemetry()
            .ConfigureResource(resource =>
                resource.AddService(CompendiumTelemetry.ServiceName))
            .WithTracing(tracing => tracing
                .AddSource(CompendiumTelemetry.ActivitySourceName)
                .AddAspNetCoreInstrumentation())
            .WithMetrics(metrics => metrics
                .AddMeter(CompendiumTelemetry.MeterName)
                .AddAspNetCoreInstrumentation()
                .AddPrometheusExporter());
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static void AddMvcApplicationPart(
        object mvcBuilder,
        Assembly applicationAssembly) =>
        ((IMvcBuilder)mvcBuilder).AddApplicationPart(applicationAssembly);

    private static void AddApplicationServices(IServiceCollection services)
    {
        var handlerTypes = typeof(CreateRulesetUseCase).Assembly
            .GetTypes()
            .Where(type =>
                type is { IsClass: true, IsAbstract: false, IsPublic: true }
                && (type.Name.EndsWith("UseCase", StringComparison.Ordinal)
                    || type.Name.EndsWith("Query", StringComparison.Ordinal)));

        foreach (var handlerType in handlerTypes)
        {
            services.AddScoped(handlerType);
        }

        services.AddSingleton<IClock, SystemClock>();
        services.AddSingleton<CompendiumConsistencyChecker>();
    }

    private static void AddInfrastructureServices(
        IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString(
            CompendiumDatabaseOptions.ConnectionStringName);

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            connectionString = CompendiumDatabaseOptions.DefaultLocalConnectionString;
        }

        services.Configure<CompendiumDatabaseOptions>(
            configuration.GetSection(CompendiumDatabaseOptions.SectionName));
        services.AddOptions<IntegrationMessagingOptions>()
            .Bind(configuration.GetSection(IntegrationMessagingOptions.SectionName))
            .Validate(
                options => options.PollingInterval > TimeSpan.Zero,
                "IntegrationMessaging:PollingInterval must be positive.")
            .Validate(
                options => options.BacklogMetricsInterval >= TimeSpan.FromSeconds(5)
                    && options.BacklogMetricsInterval <= TimeSpan.FromHours(1),
                "IntegrationMessaging:BacklogMetricsInterval must be between five seconds and one hour.")
            .Validate(
                options => options.BatchSize > 0 && options.MaxRetries > 0 && options.RetryDelay >= TimeSpan.Zero,
                "IntegrationMessaging batch and retry settings are invalid.")
            .Validate(
                options => options.ProcessingLeaseDuration >= TimeSpan.FromSeconds(30)
                    && options.PublishAttemptTimeout > TimeSpan.Zero
                    && options.PublishAttemptTimeout < options.ProcessingLeaseDuration / 2,
                "IntegrationMessaging publish timeout must be positive and less than half of a lease of at least 30 seconds.")
            .Validate(
                options => options.PublishedRetention >= TimeSpan.FromDays(1)
                    && options.CleanupInterval > TimeSpan.Zero
                    && options.CleanupBatchSize > 0
                    && options.CleanupMaxBatchesPerRun > 0
                    && (long)options.CleanupBatchSize * options.CleanupMaxBatchesPerRun <= 100_000
                    && options.CleanupInterBatchDelay >= TimeSpan.Zero,
                "IntegrationMessaging cleanup settings are outside their safe bounds.")
            .ValidateOnStart();

        services.AddSingleton<DatabaseTelemetryInterceptor>();
        services.AddDbContext<CompendiumDbContext>((provider, options) =>
            options.UseNpgsql(
                    connectionString,
                    npgsql => npgsql.MigrationsHistoryTable(
                        CompendiumDbContext.MigrationsHistoryTable,
                        CompendiumDbContext.Schema))
                .AddInterceptors(provider.GetRequiredService<DatabaseTelemetryInterceptor>()));

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
        services.AddScoped<ISourceVersionImportGateway>(
            provider => provider.GetRequiredService<SourceVersionImportGateway>());
        services.AddScoped<ISourceVersionValidationGateway>(
            provider => provider.GetRequiredService<SourceVersionImportGateway>());
        services.AddScoped<IInternalCompendiumQueryGateway, InternalCompendiumQueryGateway>();
        services.AddScoped<IEventPublisher, OutboxEventPublisher>();
        services.AddScoped<IMessageConsumer, IdempotentMessageConsumer>();
        services.AddScoped<IEventTransport, LoggingEventTransport>();
        services.AddHostedService<OutboxDispatcher>();
        services.AddHostedService<OutboxBacklogCollector>();
    }
}
