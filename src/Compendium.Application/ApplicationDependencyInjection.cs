using Compendium.Domain.SharedKernel;
using Compendium.Application.Sources;
using Microsoft.Extensions.DependencyInjection;

namespace Compendium.Application;

public static class ApplicationDependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddSingleton<IClock, SystemClock>();
        services.AddScoped<CreateRulesetUseCase>();
        services.AddScoped<UpdateRulesetUseCase>();
        services.AddScoped<GetRulesetByCodeQuery>();
        services.AddScoped<CreateRuleSourceUseCase>();
        services.AddScoped<ActivateRuleSourceUseCase>();
        services.AddScoped<DeactivateRuleSourceUseCase>();
        services.AddScoped<ListRuleSourcesByRulesetQuery>();
        services.AddScoped<CreateSourceVersionUseCase>();
        services.AddScoped<MarkSourceVersionAsCurrentUseCase>();
        services.AddScoped<GetCurrentSourceVersionQuery>();
        services.AddScoped<ListSourceVersionsQuery>();

        return services;
    }
}
