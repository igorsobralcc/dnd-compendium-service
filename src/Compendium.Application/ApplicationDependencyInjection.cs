using Compendium.Domain.SharedKernel;
using Microsoft.Extensions.DependencyInjection;

namespace Compendium.Application;

public static class ApplicationDependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddSingleton<IClock, SystemClock>();

        return services;
    }
}
