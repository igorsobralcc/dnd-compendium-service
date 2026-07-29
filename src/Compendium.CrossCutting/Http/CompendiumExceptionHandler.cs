using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Compendium.CrossCutting.Http;

internal sealed class CompendiumExceptionHandler(
    IProblemDetailsService problemDetailsService,
    ILogger<CompendiumExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        try
        {
            logger.LogError(
                exception,
                "An unhandled exception occurred while processing {Method} {Path}.",
                httpContext.Request.Method,
                httpContext.Request.Path);
        }
        catch (Exception loggingException) when (loggingException != exception)
        {
            // A failing logging sink must not replace the sanitized HTTP response.
        }

        httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
        var correlationId = httpContext.Request.Headers["X-Correlation-ID"].FirstOrDefault();
        if (string.IsNullOrWhiteSpace(correlationId))
        {
            correlationId = httpContext.TraceIdentifier;
        }

        httpContext.Response.Headers["X-Correlation-ID"] = correlationId;

        return await problemDetailsService.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            Exception = exception,
            ProblemDetails = new ProblemDetails
            {
                Status = StatusCodes.Status500InternalServerError,
                Title = "Unexpected error",
                Detail = "An unexpected error occurred while processing the request.",
                Type = "https://dnd-compendium/errors/compendium.unexpected",
                Instance = httpContext.Request.Path,
                Extensions =
                {
                    ["code"] = "compendium.unexpected",
                    ["traceId"] = httpContext.TraceIdentifier
                }
            }
        });
    }
}
