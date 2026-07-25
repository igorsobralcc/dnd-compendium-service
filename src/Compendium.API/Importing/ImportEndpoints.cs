using Compendium.API.Errors;
using Compendium.Application.Importing;

namespace Compendium.API.Importing;

public static class ImportEndpoints
{
    public static IEndpointRouteBuilder MapImportEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/compendium/source-versions");
        group.MapPost("/{sourceVersionId:guid}/imports", Import).WithName("ImportSourceVersion");
        group.MapPost("/{sourceVersionId:guid}/validation", Validate).WithName("ValidateSourceVersion");
        group.MapGet("/{sourceVersionId:guid}/validation/issues", ListIssues).WithName("ListSourceVersionValidationIssues");
        return endpoints;
    }

    private static async Task<IResult> Import(
        Guid sourceVersionId,
        ImportSourceVersionRequest request,
        HttpContext httpContext,
        ImportSourceVersionUseCase useCase,
        CancellationToken cancellationToken)
    {
        var correlationId = httpContext.Request.Headers["X-Correlation-ID"].FirstOrDefault() ?? httpContext.TraceIdentifier;
        var result = await useCase.ExecuteAsync(new(
            sourceVersionId,
            correlationId,
            request.Abilities ?? [],
            request.Skills ?? [],
            request.Languages ?? [],
            request.Proficiencies ?? [],
            request.HitDice ?? [],
            request.Equipment ?? []), cancellationToken);

        return result.IsSuccess
            ? (result.Value.AlreadyImported ? Results.Ok(result.Value) : Results.Created($"/api/compendium/source-versions/{sourceVersionId}/imports/{result.Value.ImportId}", result.Value))
            : HttpErrorMapper.ToProblem(result.Error);
    }

    private static async Task<IResult> Validate(Guid sourceVersionId, ValidateSourceVersionUseCase useCase, CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(sourceVersionId, cancellationToken);
        return result.IsSuccess ? Results.Ok(result.Value) : HttpErrorMapper.ToProblem(result.Error);
    }

    private static async Task<IResult> ListIssues(Guid sourceVersionId, ListSourceVersionValidationIssuesQuery query, CancellationToken cancellationToken)
    {
        var result = await query.ExecuteAsync(sourceVersionId, cancellationToken);
        return result.IsSuccess ? Results.Ok(result.Value) : HttpErrorMapper.ToProblem(result.Error);
    }
}

public sealed record ImportSourceVersionRequest(
    IReadOnlyCollection<SeedNamedEntry>? Abilities,
    IReadOnlyCollection<SeedSkillEntry>? Skills,
    IReadOnlyCollection<SeedNamedEntry>? Languages,
    IReadOnlyCollection<SeedProficiencyEntry>? Proficiencies,
    IReadOnlyCollection<int>? HitDice,
    IReadOnlyCollection<SeedEquipmentEntry>? Equipment);
