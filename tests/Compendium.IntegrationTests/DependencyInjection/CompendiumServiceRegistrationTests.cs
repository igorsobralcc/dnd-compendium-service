using Compendium.Application.Sources;
using Compendium.CrossCutting;
using Compendium.Domain.SharedKernel;
using Compendium.Infra.Integration;
using Compendium.Infra.Observability;
using Compendium.Infra.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace Compendium.IntegrationTests.DependencyInjection;

public sealed class CompendiumServiceRegistrationTests
{
    [Fact]
    public void Every_application_handler_is_registered_and_resolvable()
    {
        var services = CreateServices();
        var handlerTypes = typeof(CreateRulesetUseCase).Assembly
            .GetTypes()
            .Where(type =>
                type is { IsClass: true, IsAbstract: false, IsPublic: true }
                && (type.Name.EndsWith("UseCase", StringComparison.Ordinal)
                    || type.Name.EndsWith("Query", StringComparison.Ordinal)))
            .ToArray();

        Assert.Equal(81, handlerTypes.Length);

        using var provider = services.BuildServiceProvider();
        using var scope = provider.CreateScope();

        foreach (var handlerType in handlerTypes)
        {
            Assert.NotNull(scope.ServiceProvider.GetRequiredService(handlerType));
        }
    }

    [Fact]
    public void Critical_services_keep_their_required_lifetimes()
    {
        var services = CreateServices();

        Assert.Equal(ServiceLifetime.Scoped, LifetimeOf<CompendiumDbContext>(services));
        Assert.Equal(ServiceLifetime.Singleton, LifetimeOf<IClock>(services));
        Assert.Equal(ServiceLifetime.Singleton, LifetimeOf<DatabaseTelemetryInterceptor>(services));
        Assert.Contains(
            services,
            descriptor => descriptor.ServiceType == typeof(IHostedService)
                && descriptor.ImplementationType == typeof(OutboxDispatcher));
        Assert.Contains(
            services,
            descriptor => descriptor.ServiceType == typeof(IHostedService)
                && descriptor.ImplementationType == typeof(OutboxBacklogCollector));
        Assert.Contains(
            services,
            descriptor => descriptor.ServiceType == typeof(IHostedService)
                && descriptor.ImplementationType == typeof(OutboxCleanupService));
    }

    [Fact]
    public void Integration_messaging_defaults_are_validated_at_startup()
    {
        var services = CreateServices();

        using var provider = services.BuildServiceProvider();
        var options = provider.GetRequiredService<IOptions<IntegrationMessagingOptions>>().Value;

        Assert.Equal(TimeSpan.FromSeconds(2), options.PollingInterval);
        Assert.Equal(TimeSpan.FromMinutes(1), options.BacklogMetricsInterval);
        Assert.False(options.CleanupEnabled);
        Assert.Equal(TimeSpan.FromDays(30), options.PublishedRetention);
        Assert.Equal(TimeSpan.FromHours(1), options.CleanupInterval);
        Assert.Equal(1_000, options.CleanupBatchSize);
        Assert.Equal(10, options.CleanupMaxBatchesPerRun);
        Assert.Equal(TimeSpan.FromMilliseconds(100), options.CleanupInterBatchDelay);
        Assert.Equal(TimeSpan.FromMinutes(2), options.ProcessingLeaseDuration);
        Assert.Equal(TimeSpan.FromSeconds(30), options.PublishAttemptTimeout);
    }

    [Theory]
    [InlineData("IntegrationMessaging:PublishedRetention", "23:59:59")]
    [InlineData("IntegrationMessaging:CleanupInterval", "00:00:00")]
    [InlineData("IntegrationMessaging:CleanupBatchSize", "0")]
    [InlineData("IntegrationMessaging:CleanupMaxBatchesPerRun", "0")]
    [InlineData("IntegrationMessaging:CleanupInterBatchDelay", "-00:00:01")]
    [InlineData("IntegrationMessaging:CleanupBatchSize", "100001")]
    [InlineData("IntegrationMessaging:ProcessingLeaseDuration", "00:00:29")]
    [InlineData("IntegrationMessaging:PublishAttemptTimeout", "00:00:00")]
    [InlineData("IntegrationMessaging:PublishAttemptTimeout", "00:01:00")]
    public void Integration_messaging_rejects_unsafe_retention_and_claim_settings(
        string key,
        string value)
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?> { [key] = value })
            .Build();
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddCompendiumServices(configuration);

        using var provider = services.BuildServiceProvider();

        Assert.Throws<OptionsValidationException>(() =>
            provider.GetRequiredService<IOptions<IntegrationMessagingOptions>>().Value);
    }

    [Fact]
    public void Integration_messaging_rejects_an_unsafe_backlog_interval()
    {
        var values = new Dictionary<string, string?>
        {
            ["IntegrationMessaging:BacklogMetricsInterval"] = "00:00:01"
        };
        var configuration = new ConfigurationBuilder().AddInMemoryCollection(values).Build();
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddCompendiumServices(configuration);

        using var provider = services.BuildServiceProvider();

        Assert.Throws<OptionsValidationException>(() =>
            provider.GetRequiredService<IOptions<IntegrationMessagingOptions>>().Value);
    }

    private static ServiceCollection CreateServices()
    {
        var configuration = new ConfigurationBuilder().Build();
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddCompendiumServices(configuration);
        return services;
    }

    private static ServiceLifetime LifetimeOf<TService>(IServiceCollection services) =>
        Assert.Single(
            services,
            descriptor => descriptor.ServiceType == typeof(TService)).Lifetime;
}
