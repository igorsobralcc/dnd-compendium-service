using Compendium.Application.InternalQueries;
using Microsoft.AspNetCore.Mvc;

namespace Compendium.API.InternalQueries;

public static class InternalCompendiumEndpoints
{
    public static IEndpointRouteBuilder MapInternalCompendiumEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/internal/compendium").WithTags("Internal Compendium");

        group.MapGet("/character-creation-options", GetCharacterCreationOptions)
            .WithName("GetCharacterCreationOptionsV1")
            .Produces<CharacterCreationOptionsV1>()
            .ProducesValidationProblem();

        group.MapGet("/entities/{entityType}/{entityId:guid}/mechanics", GetMechanicalEntityDetails)
            .WithName("GetMechanicalEntityDetailsV1")
            .Produces<MechanicalEntityDetailsV1>()
            .Produces(StatusCodes.Status404NotFound);

        group.MapGet("/changes", ListChanges)
            .WithName("ListCompendiumChangesV1")
            .Produces<CompendiumChangesV1>()
            .ProducesValidationProblem();

        return endpoints;
    }

    private static async Task<IResult> GetCharacterCreationOptions(
        [FromQuery(Name = "ruleset_id")] Guid rulesetId,
        [FromQuery(Name = "source_version_id")] Guid sourceVersionId,
        string? locale,
        int? level,
        GetCharacterCreationOptionsQuery query,
        CancellationToken cancellationToken)
    {
        if (rulesetId == Guid.Empty || sourceVersionId == Guid.Empty)
            return Results.ValidationProblem(new Dictionary<string, string[]>
            {
                ["filters"] = ["rulesetId and sourceVersionId are required."]
            });
        if (level is < 1 or > 20)
            return Results.ValidationProblem(new Dictionary<string, string[]>
            {
                ["level"] = ["Level must be between 1 and 20."]
            });

        var result = await query.ExecuteAsync(
            new(rulesetId, sourceVersionId, locale ?? "en-US", level),
            cancellationToken);
        return Results.Ok(result);
    }

    private static async Task<IResult> GetMechanicalEntityDetails(
        string entityType,
        Guid entityId,
        string? locale,
        GetMechanicalEntityDetailsQuery query,
        CancellationToken cancellationToken)
    {
        var result = await query.ExecuteAsync(entityType, entityId, locale ?? "en-US", cancellationToken);
        return result is null ? Results.NotFound() : Results.Ok(result);
    }

    private static async Task<IResult> ListChanges(
        [FromQuery(Name = "source_version_id")] Guid? sourceVersionId,
        [FromQuery(Name = "entity_type")] string? entityType,
        [FromQuery(Name = "changed_since")] DateTimeOffset? changedSince,
        long? revision,
        int? page,
        [FromQuery(Name = "page_size")] int? pageSize,
        ListCompendiumChangesSinceQuery query,
        CancellationToken cancellationToken)
    {
        if (revision < 0 || page < 1 || pageSize is < 1 or > 200)
            return Results.ValidationProblem(new Dictionary<string, string[]>
            {
                ["pagination"] = ["revision must be non-negative, page >= 1 and pageSize between 1 and 200."]
            });

        var result = await query.ExecuteAsync(
            new(sourceVersionId, entityType, changedSince, revision, page ?? 1, pageSize ?? 50),
            cancellationToken);
        return Results.Ok(result);
    }
}
