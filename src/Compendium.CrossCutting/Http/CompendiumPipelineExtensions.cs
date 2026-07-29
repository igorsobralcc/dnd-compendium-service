using Compendium.CrossCutting.Observability;
using Compendium.CrossCutting.Security;
using Microsoft.AspNetCore.Builder;

namespace Compendium.CrossCutting.Http;

public static class CompendiumPipelineExtensions
{
    public static WebApplication UseCompendiumPipeline(this WebApplication app)
    {
        app.UseExceptionHandler();
        app.UseMiddleware<RequestObservabilityMiddleware>();
        app.UseAuthentication();
        app.UseMiddleware<CompendiumRouteAuthorizationMiddleware>();
        app.UseAuthorization();

        return app;
    }
}
