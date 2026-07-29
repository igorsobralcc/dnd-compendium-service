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
