using Compendium.Application.Errors;
using Microsoft.AspNetCore.Mvc;

namespace Compendium.API.Controllers;

[ApiController]
public abstract class CompendiumControllerBase : ControllerBase
{
    protected IActionResult OkOrProblem<T>(ApplicationResult<T> result) =>
        ToActionResult(result, value => Ok(value));

    protected IActionResult CreatedOrProblem<T>(
        ApplicationResult<T> result,
        Func<T, string> location) =>
        ToActionResult(result, value => Created(location(value), value));

    protected IActionResult NoContentOrProblem(ApplicationResult result) =>
        ToActionResult(result, NoContent);

    protected IActionResult ToActionResult<T>(
        ApplicationResult<T> result,
        Func<T, IActionResult> onSuccess) =>
        result.IsSuccess
            ? onSuccess(result.Value)
            : ToProblem(result.Error);

    protected IActionResult ToActionResult(
        ApplicationResult result,
        Func<IActionResult> onSuccess) =>
        result.IsSuccess
            ? onSuccess()
            : ToProblem(result.Error);

    private static ObjectResult ToProblem(ApplicationError error)
    {
        var statusCode = ToStatusCode(error.Kind);

        return new ObjectResult(new ProblemDetails
        {
            Status = statusCode,
            Title = error.Kind.ToString(),
            Detail = error.Message,
            Type = $"https://errors.dnd-compendium-service.local/{error.Code}",
            Extensions = { ["code"] = error.Code }
        })
        {
            StatusCode = statusCode
        };
    }

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
