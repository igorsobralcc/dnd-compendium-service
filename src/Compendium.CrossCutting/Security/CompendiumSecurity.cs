using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Compendium.CrossCutting.Security;

public static class CompendiumSecurity
{
    public const string Scheme = "CompendiumApiKey";
    public const string AdministrativeWritePolicy = "CompendiumAdministrativeWrite";
    public const string InternalReadPolicy = "CompendiumInternalRead";

    internal static IServiceCollection AddCompendiumSecurity(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<CompendiumSecurityOptions>(
            configuration.GetSection("Compendium:Security"));
        services.AddAuthentication(Scheme)
            .AddScheme<AuthenticationSchemeOptions, ApiKeyAuthenticationHandler>(
                Scheme,
                _ => { });
        services.AddAuthorization(options =>
        {
            options.AddPolicy(AdministrativeWritePolicy, policy =>
                policy.RequireAuthenticatedUser()
                    .RequireClaim("compendium.permission", "write"));
            options.AddPolicy(InternalReadPolicy, policy =>
                policy.RequireAuthenticatedUser()
                    .RequireClaim("compendium.permission", "read"));
        });

        return services;
    }
}

public sealed class CompendiumSecurityOptions
{
    public string HeaderName { get; init; } = "X-API-Key";

    public string? AdministrativeApiKey { get; init; }

    public string? InternalServiceApiKey { get; init; }
}

internal sealed class ApiKeyAuthenticationHandler(
    IOptionsMonitor<AuthenticationSchemeOptions> schemes,
    ILoggerFactory logger,
    UrlEncoder encoder,
    IOptionsMonitor<CompendiumSecurityOptions> securityOptions)
    : AuthenticationHandler<AuthenticationSchemeOptions>(schemes, logger, encoder)
{
    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var security = securityOptions.CurrentValue;
        if (!Request.Headers.TryGetValue(security.HeaderName, out var suppliedValues))
        {
            return Task.FromResult(AuthenticateResult.NoResult());
        }

        var supplied = suppliedValues.FirstOrDefault();
        if (string.IsNullOrWhiteSpace(supplied))
        {
            return Task.FromResult(AuthenticateResult.Fail("The API key is empty."));
        }

        var permissions = new List<Claim>();
        if (Matches(supplied, security.AdministrativeApiKey))
        {
            permissions.Add(new("compendium.permission", "write"));
            permissions.Add(new("compendium.permission", "read"));
        }
        else if (Matches(supplied, security.InternalServiceApiKey))
        {
            permissions.Add(new("compendium.permission", "read"));
        }
        else
        {
            return Task.FromResult(AuthenticateResult.Fail("The API key is invalid."));
        }

        var claims = new List<Claim> { new(ClaimTypes.NameIdentifier, "api-key") };
        claims.AddRange(permissions);
        var identity = new ClaimsIdentity(claims, CompendiumSecurity.Scheme);

        return Task.FromResult(AuthenticateResult.Success(
            new AuthenticationTicket(
                new ClaimsPrincipal(identity),
                CompendiumSecurity.Scheme)));
    }

    private static bool Matches(string supplied, string? configured)
    {
        if (string.IsNullOrEmpty(configured))
        {
            return false;
        }

        var suppliedBytes = Encoding.UTF8.GetBytes(supplied);
        var configuredBytes = Encoding.UTF8.GetBytes(configured);

        return suppliedBytes.Length == configuredBytes.Length
            && System.Security.Cryptography.CryptographicOperations.FixedTimeEquals(
                suppliedBytes,
                configuredBytes);
    }

    protected override Task HandleChallengeAsync(AuthenticationProperties properties) =>
        WriteProblemAsync(
            StatusCodes.Status401Unauthorized,
            "Authentication required",
            "A valid API key is required to access this resource.",
            "compendium.security.unauthenticated");

    protected override Task HandleForbiddenAsync(AuthenticationProperties properties) =>
        WriteProblemAsync(
            StatusCodes.Status403Forbidden,
            "Insufficient permission",
            "The authenticated principal is not allowed to access this resource.",
            "compendium.security.forbidden");

    private Task WriteProblemAsync(int status, string title, string detail, string code)
    {
        Response.StatusCode = status;
        Response.ContentType = "application/problem+json";
        var problem = new ProblemDetails
        {
            Status = status,
            Title = title,
            Detail = detail,
            Type = $"https://dnd-compendium/errors/{code}",
            Instance = Request.Path,
            Extensions =
            {
                ["code"] = code,
                ["traceId"] = Context.TraceIdentifier
            }
        };

        return JsonSerializer.SerializeAsync(
            Response.Body,
            problem,
            new JsonSerializerOptions(JsonSerializerDefaults.Web),
            Context.RequestAborted);
    }
}

internal sealed class CompendiumRouteAuthorizationMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(
        HttpContext context,
        IAuthorizationService authorization)
    {
        var policy = RequiredPolicy(context.Request);
        if (policy is null)
        {
            await next(context);
            return;
        }

        var result = await authorization.AuthorizeAsync(context.User, null, policy);
        if (result.Succeeded)
        {
            await next(context);
            return;
        }

        if (context.User.Identity?.IsAuthenticated == true)
        {
            await context.ForbidAsync(CompendiumSecurity.Scheme);
        }
        else
        {
            await context.ChallengeAsync(CompendiumSecurity.Scheme);
        }
    }

    private static string? RequiredPolicy(HttpRequest request)
    {
        if (request.Path.StartsWithSegments("/internal/compendium")
            && !request.Path.Equals("/internal/compendium/metadata"))
        {
            return CompendiumSecurity.InternalReadPolicy;
        }

        if (request.Path.StartsWithSegments("/api/compendium")
            && request.Method is not ("GET" or "HEAD" or "OPTIONS"))
        {
            return CompendiumSecurity.AdministrativeWritePolicy;
        }

        return null;
    }
}
