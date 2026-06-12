using Compendium.Application.Errors;
using Compendium.Domain.SharedKernel;
using Microsoft.AspNetCore.Mvc;

namespace Compendium.API.Errors;

public static class HttpErrorMapper
{
    public static IResult ToProblem(DomainError error)
    {
        var statusCode = ToStatusCode(error.Kind);

        return Results.Problem(new ProblemDetails
        {
            Status = statusCode,
            Title = error.Kind.ToString(),
            Detail = error.Message,
            Type = $"https://errors.dnd-compendium-service.local/{error.Code}"
        });
    }

    public static IResult ToProblem(ApplicationError error)
    {
        var statusCode = ToStatusCode(error.Kind);

        return Results.Problem(new ProblemDetails
        {
            Status = statusCode,
            Title = error.Kind.ToString(),
            Detail = error.Message,
            Type = $"https://errors.dnd-compendium-service.local/{error.Code}"
        });
    }

    private static int ToStatusCode(DomainErrorKind kind) =>
        kind switch
        {
            DomainErrorKind.Validation => StatusCodes.Status400BadRequest,
            DomainErrorKind.Conflict => StatusCodes.Status409Conflict,
            DomainErrorKind.NotFound => StatusCodes.Status404NotFound,
            DomainErrorKind.Unauthorized => StatusCodes.Status401Unauthorized,
            DomainErrorKind.Forbidden => StatusCodes.Status403Forbidden,
            DomainErrorKind.Unexpected => StatusCodes.Status500InternalServerError,
            _ => StatusCodes.Status500InternalServerError
        };

    private static int ToStatusCode(ApplicationErrorKind kind) =>
        kind switch
        {
            ApplicationErrorKind.Validation => StatusCodes.Status400BadRequest,
            ApplicationErrorKind.Conflict => StatusCodes.Status409Conflict,
            ApplicationErrorKind.NotFound => StatusCodes.Status404NotFound,
            ApplicationErrorKind.Unauthorized => StatusCodes.Status401Unauthorized,
            ApplicationErrorKind.Forbidden => StatusCodes.Status403Forbidden,
            ApplicationErrorKind.Unexpected => StatusCodes.Status500InternalServerError,
            _ => StatusCodes.Status500InternalServerError
        };
}
