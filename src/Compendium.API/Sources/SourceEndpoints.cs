using Compendium.API.Errors;
using Compendium.Application.Sources;
using Compendium.Domain.Sources;

namespace Compendium.API.Sources;

public static class SourceEndpoints
{
    public static IEndpointRouteBuilder MapSourceEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/compendium");

        group.MapPost("/rulesets", CreateRuleset).WithName("CreateRuleset");
        group.MapPut("/rulesets/{code}", UpdateRuleset).WithName("UpdateRuleset");
        group.MapGet("/rulesets/{code}", GetRulesetByCode).WithName("GetRulesetByCode");

        group.MapPost("/rule-sources", CreateRuleSource).WithName("CreateRuleSource");
        group.MapPost("/rule-sources/{id:guid}/activate", ActivateRuleSource).WithName("ActivateRuleSource");
        group.MapPost("/rule-sources/{id:guid}/deactivate", DeactivateRuleSource).WithName("DeactivateRuleSource");
        group.MapGet("/rulesets/{rulesetId:guid}/rule-sources", ListRuleSourcesByRuleset).WithName("ListRuleSourcesByRuleset");

        group.MapPost("/source-versions", CreateSourceVersion).WithName("CreateSourceVersion");
        group.MapPost("/rule-sources/{ruleSourceId:guid}/source-versions/{versionId:guid}/current", MarkSourceVersionAsCurrent)
            .WithName("MarkSourceVersionAsCurrent");
        group.MapGet("/rule-sources/{ruleSourceId:guid}/source-versions/current", GetCurrentSourceVersion)
            .WithName("GetCurrentSourceVersion");
        group.MapGet("/rule-sources/{ruleSourceId:guid}/source-versions", ListSourceVersions).WithName("ListSourceVersions");

        return endpoints;
    }

    private static async Task<IResult> CreateRuleset(
        CreateRulesetRequest request,
        CreateRulesetUseCase useCase,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(
            new CreateRulesetCommand(request.Code, request.Name, request.Version, request.Status),
            cancellationToken);

        return result.IsSuccess
            ? Results.Created($"/api/compendium/rulesets/{result.Value.Code}", result.Value)
            : HttpErrorMapper.ToProblem(result.Error);
    }

    private static async Task<IResult> UpdateRuleset(
        string code,
        UpdateRulesetRequest request,
        UpdateRulesetUseCase useCase,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(
            new UpdateRulesetCommand(code, request.Name, request.Version, request.Status),
            cancellationToken);

        return result.IsSuccess ? Results.Ok(result.Value) : HttpErrorMapper.ToProblem(result.Error);
    }

    private static async Task<IResult> GetRulesetByCode(
        string code,
        GetRulesetByCodeQuery query,
        CancellationToken cancellationToken)
    {
        var result = await query.ExecuteAsync(code, cancellationToken);
        return result.IsSuccess ? Results.Ok(result.Value) : HttpErrorMapper.ToProblem(result.Error);
    }

    private static async Task<IResult> CreateRuleSource(
        CreateRuleSourceRequest request,
        CreateRuleSourceUseCase useCase,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(
            new CreateRuleSourceCommand(request.RulesetId, request.Code, request.Name, request.Type, request.Status),
            cancellationToken);

        return result.IsSuccess
            ? Results.Created($"/api/compendium/rule-sources/{result.Value.Id}", result.Value)
            : HttpErrorMapper.ToProblem(result.Error);
    }

    private static async Task<IResult> ActivateRuleSource(
        Guid id,
        ActivateRuleSourceUseCase useCase,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(id, cancellationToken);
        return result.IsSuccess ? Results.Ok(result.Value) : HttpErrorMapper.ToProblem(result.Error);
    }

    private static async Task<IResult> DeactivateRuleSource(
        Guid id,
        DeactivateRuleSourceUseCase useCase,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(id, cancellationToken);
        return result.IsSuccess ? Results.Ok(result.Value) : HttpErrorMapper.ToProblem(result.Error);
    }

    private static async Task<IResult> ListRuleSourcesByRuleset(
        Guid rulesetId,
        bool includeInactive,
        ListRuleSourcesByRulesetQuery query,
        CancellationToken cancellationToken)
    {
        var result = await query.ExecuteAsync(rulesetId, includeInactive, cancellationToken);
        return result.IsSuccess ? Results.Ok(result.Value) : HttpErrorMapper.ToProblem(result.Error);
    }

    private static async Task<IResult> CreateSourceVersion(
        CreateSourceVersionRequest request,
        CreateSourceVersionUseCase useCase,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(
            new CreateSourceVersionCommand(
                request.RuleSourceId,
                request.VersionNumber,
                request.PublicationDate,
                request.ImportStatus,
                request.IsCurrent),
            cancellationToken);

        return result.IsSuccess
            ? Results.Created($"/api/compendium/source-versions/{result.Value.Id}", result.Value)
            : HttpErrorMapper.ToProblem(result.Error);
    }

    private static async Task<IResult> MarkSourceVersionAsCurrent(
        Guid ruleSourceId,
        Guid versionId,
        MarkSourceVersionAsCurrentUseCase useCase,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(ruleSourceId, versionId, cancellationToken);
        return result.IsSuccess ? Results.Ok(result.Value) : HttpErrorMapper.ToProblem(result.Error);
    }

    private static async Task<IResult> GetCurrentSourceVersion(
        Guid ruleSourceId,
        GetCurrentSourceVersionQuery query,
        CancellationToken cancellationToken)
    {
        var result = await query.ExecuteAsync(ruleSourceId, cancellationToken);
        return result.IsSuccess ? Results.Ok(result.Value) : HttpErrorMapper.ToProblem(result.Error);
    }

    private static async Task<IResult> ListSourceVersions(
        Guid ruleSourceId,
        ListSourceVersionsQuery query,
        CancellationToken cancellationToken)
    {
        var result = await query.ExecuteAsync(ruleSourceId, cancellationToken);
        return result.IsSuccess ? Results.Ok(result.Value) : HttpErrorMapper.ToProblem(result.Error);
    }
}

public sealed record CreateRulesetRequest(string Code, string Name, string Version, RulesetStatus Status);

public sealed record UpdateRulesetRequest(string Name, string Version, RulesetStatus Status);

public sealed record CreateRuleSourceRequest(Guid RulesetId, string Code, string Name, SourceType Type, SourceStatus Status);

public sealed record CreateSourceVersionRequest(
    Guid RuleSourceId,
    string VersionNumber,
    DateOnly PublicationDate,
    ImportStatus ImportStatus,
    bool IsCurrent);
