using System.Diagnostics;
using Compendium.Application.Observability;

namespace Compendium.API.Observability;

public sealed class RequestObservabilityMiddleware(
    RequestDelegate next,
    ILogger<RequestObservabilityMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        var correlationId = context.Request.Headers["X-Correlation-ID"].FirstOrDefault();
        if (string.IsNullOrWhiteSpace(correlationId))
            correlationId = context.TraceIdentifier;

        context.Response.Headers["X-Correlation-ID"] = correlationId;
        Activity.Current?.SetTag("correlation.id", correlationId);

        var started = Stopwatch.GetTimestamp();
        using var scope = logger.BeginScope(new Dictionary<string, object>
        {
            ["CorrelationId"] = correlationId,
            ["TraceId"] = Activity.Current?.TraceId.ToString() ?? context.TraceIdentifier
        });

        try
        {
            await next(context);
        }
        finally
        {
            var durationMs = Stopwatch.GetElapsedTime(started).TotalMilliseconds;
            var tags = new TagList
            {
                { "http.request.method", context.Request.Method },
                { "http.route", context.GetEndpoint()?.DisplayName ?? context.Request.Path.Value ?? "unknown" },
                { "http.response.status_code", context.Response.StatusCode }
            };
            CompendiumTelemetry.HttpRequestDuration.Record(durationMs, tags);

            if (context.Request.Path.StartsWithSegments("/api/compendium/source-versions")
                && context.Request.Path.Value?.EndsWith("/imports", StringComparison.OrdinalIgnoreCase) == true
                && context.Response.StatusCode >= StatusCodes.Status400BadRequest)
            {
                CompendiumTelemetry.ImportFailures.Add(1, tags);
            }

            logger.LogInformation(
                "HTTP {Method} {Path} completed with {StatusCode} in {ElapsedMilliseconds:F2} ms.",
                context.Request.Method,
                context.Request.Path,
                context.Response.StatusCode,
                durationMs);
        }
    }
}
