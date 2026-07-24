using Compendium.API.Errors;
using Compendium.Application.Origins;
using Compendium.Domain.Features;
using Compendium.Domain.Origins;

namespace Compendium.API.Origins;

public static class OriginEndpoints
{
    public static IEndpointRouteBuilder MapOriginEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/compendium");
        group.MapPost("/species", CreateSpecies).WithName("CreateSpecies");
        group.MapGet("/species", ListSpecies).WithName("ListSpecies");
        group.MapGet("/species/{code}", GetSpeciesDetails).WithName("GetSpeciesDetails");
        group.MapPost("/species/{code}/features", LinkSpeciesFeature).WithName("LinkSpeciesFeature");

        group.MapPost("/backgrounds", CreateBackground).WithName("CreateBackground");
        group.MapGet("/backgrounds", ListBackgrounds).WithName("ListBackgrounds");
        group.MapGet("/backgrounds/{code}", GetBackgroundDetails).WithName("GetBackgroundDetails");
        group.MapPut("/backgrounds/{code}/mechanics", ConfigureBackgroundMechanics).WithName("ConfigureBackgroundMechanics");
        group.MapPost("/backgrounds/{code}/features", LinkBackgroundFeature).WithName("LinkBackgroundFeature");

        group.MapPost("/feats", CreateFeat).WithName("CreateFeat");
        group.MapGet("/feats", ListFeats).WithName("ListFeats");
        group.MapGet("/feats/{code}", GetFeatDetails).WithName("GetFeatDetails");
        group.MapPost("/feats/{code}/features", LinkFeatFeature).WithName("LinkFeatFeature");
        group.MapPost("/feats/{code}/prerequisites", AddFeatPrerequisite).WithName("AddFeatPrerequisite");
        return endpoints;
    }

    private static async Task<IResult> CreateSpecies(CreateSpeciesRequest request, CreateSpeciesUseCase useCase, CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(new(request.RuleSourceId, request.SourceVersionId, request.Code, request.Name, request.Description), cancellationToken);
        return result.IsSuccess ? Results.Created($"/api/compendium/species/{result.Value.Code}", result.Value) : HttpErrorMapper.ToProblem(result.Error);
    }
    private static async Task<IResult> ListSpecies(ListSpeciesQuery query, CancellationToken cancellationToken)
    {
        var result = await query.ExecuteAsync(cancellationToken);
        return result.IsSuccess ? Results.Ok(result.Value) : HttpErrorMapper.ToProblem(result.Error);
    }
    private static async Task<IResult> GetSpeciesDetails(string code, GetSpeciesDetailsQuery query, CancellationToken cancellationToken)
    {
        var result = await query.ExecuteAsync(code, cancellationToken);
        return result.IsSuccess ? Results.Ok(result.Value) : HttpErrorMapper.ToProblem(result.Error);
    }
    private static async Task<IResult> LinkSpeciesFeature(string code, LinkFeatureRequest request, LinkSpeciesFeatureUseCase useCase, CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(new(code, request.RuleSourceId, request.SourceVersionId, request.FeatureId), cancellationToken);
        return result.IsSuccess ? Results.Ok(result.Value) : HttpErrorMapper.ToProblem(result.Error);
    }

    private static async Task<IResult> CreateBackground(CreateBackgroundRequest request, CreateBackgroundUseCase useCase, CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(new(request.RuleSourceId, request.SourceVersionId, request.Code, request.Name, request.Description), cancellationToken);
        return result.IsSuccess ? Results.Created($"/api/compendium/backgrounds/{result.Value.Code}", result.Value) : HttpErrorMapper.ToProblem(result.Error);
    }
    private static async Task<IResult> ListBackgrounds(ListBackgroundsQuery query, CancellationToken cancellationToken)
    {
        var result = await query.ExecuteAsync(cancellationToken);
        return result.IsSuccess ? Results.Ok(result.Value) : HttpErrorMapper.ToProblem(result.Error);
    }
    private static async Task<IResult> GetBackgroundDetails(string code, GetBackgroundDetailsQuery query, CancellationToken cancellationToken)
    {
        var result = await query.ExecuteAsync(code, cancellationToken);
        return result.IsSuccess ? Results.Ok(result.Value) : HttpErrorMapper.ToProblem(result.Error);
    }
    private static async Task<IResult> ConfigureBackgroundMechanics(string code, ConfigureBackgroundMechanicsRequest request, ConfigureBackgroundMechanicsUseCase useCase, CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(new(code, request.RuleSourceId, request.SourceVersionId, request.AbilityOptionIds,
            request.AbilityBoostRules, request.FeatIds, request.SkillProficiencyIds, request.ToolProficiencyIds, request.StartingEquipmentRules), cancellationToken);
        return result.IsSuccess ? Results.Ok(result.Value) : HttpErrorMapper.ToProblem(result.Error);
    }
    private static async Task<IResult> LinkBackgroundFeature(string code, LinkFeatureRequest request, LinkBackgroundFeatureUseCase useCase, CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(new(code, request.RuleSourceId, request.SourceVersionId, request.FeatureId), cancellationToken);
        return result.IsSuccess ? Results.Ok(result.Value) : HttpErrorMapper.ToProblem(result.Error);
    }

    private static async Task<IResult> CreateFeat(CreateFeatRequest request, CreateFeatUseCase useCase, CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(new(request.RuleSourceId, request.SourceVersionId, request.Code, request.Name, request.Description, request.Category, request.Repeatable), cancellationToken);
        return result.IsSuccess ? Results.Created($"/api/compendium/feats/{result.Value.Code}", result.Value) : HttpErrorMapper.ToProblem(result.Error);
    }
    private static async Task<IResult> ListFeats(ListFeatsQuery query, CancellationToken cancellationToken)
    {
        var result = await query.ExecuteAsync(cancellationToken);
        return result.IsSuccess ? Results.Ok(result.Value) : HttpErrorMapper.ToProblem(result.Error);
    }
    private static async Task<IResult> GetFeatDetails(string code, GetFeatDetailsQuery query, CancellationToken cancellationToken)
    {
        var result = await query.ExecuteAsync(code, cancellationToken);
        return result.IsSuccess ? Results.Ok(result.Value) : HttpErrorMapper.ToProblem(result.Error);
    }
    private static async Task<IResult> LinkFeatFeature(string code, LinkFeatureRequest request, LinkFeatFeatureUseCase useCase, CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(new(code, request.RuleSourceId, request.SourceVersionId, request.FeatureId), cancellationToken);
        return result.IsSuccess ? Results.Ok(result.Value) : HttpErrorMapper.ToProblem(result.Error);
    }
    private static async Task<IResult> AddFeatPrerequisite(string code, AddFeatPrerequisiteRequest request, AddFeatPrerequisiteUseCase useCase, CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(new(code, request.Type, request.Operator, request.Target, request.ValueType,
            request.TextValue, request.NumericValue, request.BooleanValue, request.ReferenceId, request.EnumValue), cancellationToken);
        return result.IsSuccess ? Results.Created($"/api/compendium/feats/{code}/prerequisites/{result.Value.Id}", result.Value) : HttpErrorMapper.ToProblem(result.Error);
    }
}

public sealed record CreateSpeciesRequest(Guid RuleSourceId, Guid SourceVersionId, string Code, string Name, string? Description);
public sealed record CreateBackgroundRequest(Guid RuleSourceId, Guid SourceVersionId, string Code, string Name, string? Description);
public sealed record LinkFeatureRequest(Guid RuleSourceId, Guid SourceVersionId, Guid FeatureId);
public sealed record ConfigureBackgroundMechanicsRequest(
    Guid RuleSourceId,
    Guid SourceVersionId,
    IReadOnlyCollection<Guid> AbilityOptionIds,
    IReadOnlyCollection<BackgroundAbilityBoostRuleCommand> AbilityBoostRules,
    IReadOnlyCollection<Guid> FeatIds,
    IReadOnlyCollection<Guid> SkillProficiencyIds,
    IReadOnlyCollection<Guid> ToolProficiencyIds,
    IReadOnlyCollection<BackgroundStartingEquipmentRuleCommand> StartingEquipmentRules);
public sealed record CreateFeatRequest(Guid RuleSourceId, Guid SourceVersionId, string Code, string Name, string? Description, FeatCategory Category, bool Repeatable);
public sealed record AddFeatPrerequisiteRequest(
    PrerequisiteType Type, ComparisonOperator Operator, EffectTarget Target, EffectValueType ValueType,
    string? TextValue, decimal? NumericValue, bool? BooleanValue, Guid? ReferenceId, string? EnumValue);
