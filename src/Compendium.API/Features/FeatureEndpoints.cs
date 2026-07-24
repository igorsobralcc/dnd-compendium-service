using Compendium.API.Errors;
using Compendium.Application.Features;
using Compendium.Domain.Features;

namespace Compendium.API.Features;

public static class FeatureEndpoints
{
    public static IEndpointRouteBuilder MapFeatureEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/compendium");

        group.MapPost("/features", CreateFeature).WithName("CreateFeature");
        group.MapPut("/features/{code}", UpdateFeature).WithName("UpdateFeature");
        group.MapGet("/features", ListFeatures).WithName("ListFeatures");
        group.MapGet("/features/{code}", GetFeatureDetails).WithName("GetFeatureDetails");
        group.MapPost("/effect-schemas", CreateEffectSchema).WithName("CreateEffectSchema");
        group.MapPost("/features/{featureCode}/effects", AttachEffectToFeature).WithName("AttachEffectToFeature");
        group.MapGet("/features/{featureCode}/effects", GetFeatureEffects).WithName("GetFeatureEffects");
        group.MapPost("/entity-prerequisites", AddPrerequisiteToEntity).WithName("AddPrerequisiteToEntity");
        group.MapDelete("/entity-prerequisites/{prerequisiteId:guid}", RemovePrerequisiteFromEntity).WithName("RemovePrerequisiteFromEntity");
        group.MapGet("/entity-prerequisites/{entityKind}/{entityId:guid}", GetEntityPrerequisites).WithName("GetEntityPrerequisites");
        group.MapPost("/choice-sets", CreateChoiceSet).WithName("CreateChoiceSet");
        group.MapGet("/choice-sets/{code}", GetChoiceSetDetails).WithName("GetChoiceSetDetails");
        group.MapGet("/choice-sets/by-source/{entityKind}/{entityId:guid}", ListChoiceSetsBySourceEntity).WithName("ListChoiceSetsBySourceEntity");
        group.MapPost("/choice-sets/{code}/options", AddChoiceOption).WithName("AddChoiceOption");
        group.MapPost("/choice-sets/{code}/filters", AddChoiceFilter).WithName("AddChoiceFilter");

        return endpoints;
    }

    private static async Task<IResult> CreateFeature(CreateFeatureRequest request, CreateFeatureUseCase useCase, CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(new CreateFeatureCommand(request.RuleSourceId, request.SourceVersionId, request.Code, request.Name, request.Description, request.LevelRequirement), cancellationToken);
        return result.IsSuccess ? Results.Created($"/api/compendium/features/{result.Value.Code}", result.Value) : HttpErrorMapper.ToProblem(result.Error);
    }

    private static async Task<IResult> UpdateFeature(string code, UpdateFeatureRequest request, UpdateFeatureUseCase useCase, CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(new UpdateFeatureCommand(code, request.RuleSourceId, request.SourceVersionId, request.Name, request.Description, request.LevelRequirement), cancellationToken);
        return result.IsSuccess ? Results.Ok(result.Value) : HttpErrorMapper.ToProblem(result.Error);
    }

    private static async Task<IResult> ListFeatures(ListFeaturesQuery query, CancellationToken cancellationToken)
    {
        var result = await query.ExecuteAsync(cancellationToken);
        return result.IsSuccess ? Results.Ok(result.Value) : HttpErrorMapper.ToProblem(result.Error);
    }

    private static async Task<IResult> GetFeatureDetails(string code, GetFeatureDetailsQuery query, CancellationToken cancellationToken)
    {
        var result = await query.ExecuteAsync(code, cancellationToken);
        return result.IsSuccess ? Results.Ok(result.Value) : HttpErrorMapper.ToProblem(result.Error);
    }

    private static async Task<IResult> CreateEffectSchema(CreateEffectSchemaRequest request, CreateEffectSchemaUseCase useCase, CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(new CreateEffectSchemaCommand(request.Code, request.Name, request.Type, request.Fields), cancellationToken);
        return result.IsSuccess ? Results.Created($"/api/compendium/effect-schemas/{result.Value.Code}", result.Value) : HttpErrorMapper.ToProblem(result.Error);
    }

    private static async Task<IResult> AttachEffectToFeature(string featureCode, AttachEffectToFeatureRequest request, AttachEffectToFeatureUseCase useCase, CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(new AttachEffectToFeatureCommand(featureCode, request.EffectSchemaCode, request.Type, request.Target, request.Fields, request.Conditions), cancellationToken);
        return result.IsSuccess ? Results.Ok(result.Value) : HttpErrorMapper.ToProblem(result.Error);
    }

    private static async Task<IResult> GetFeatureEffects(string featureCode, GetFeatureEffectsQuery query, CancellationToken cancellationToken)
    {
        var result = await query.ExecuteAsync(featureCode, cancellationToken);
        return result.IsSuccess ? Results.Ok(result.Value) : HttpErrorMapper.ToProblem(result.Error);
    }

    private static async Task<IResult> AddPrerequisiteToEntity(AddPrerequisiteToEntityRequest request, AddPrerequisiteToEntityUseCase useCase, CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(new AddPrerequisiteToEntityCommand(request.EntityKind, request.EntityId, request.Type, request.Operator, request.Target, request.ValueType, request.TextValue, request.NumericValue, request.BooleanValue, request.ReferenceId, request.EnumValue), cancellationToken);
        return result.IsSuccess ? Results.Created($"/api/compendium/entity-prerequisites/{result.Value.Id}", result.Value) : HttpErrorMapper.ToProblem(result.Error);
    }

    private static async Task<IResult> RemovePrerequisiteFromEntity(Guid prerequisiteId, RemovePrerequisiteFromEntityUseCase useCase, CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(new RemovePrerequisiteFromEntityCommand(prerequisiteId), cancellationToken);
        return result.IsSuccess ? Results.NoContent() : HttpErrorMapper.ToProblem(result.Error);
    }

    private static async Task<IResult> GetEntityPrerequisites(CompendiumEntityKind entityKind, Guid entityId, GetEntityPrerequisitesQuery query, CancellationToken cancellationToken)
    {
        var result = await query.ExecuteAsync(entityKind, entityId, cancellationToken);
        return result.IsSuccess ? Results.Ok(result.Value) : HttpErrorMapper.ToProblem(result.Error);
    }

    private static async Task<IResult> CreateChoiceSet(CreateChoiceSetRequest request, CreateChoiceSetUseCase useCase, CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(new CreateChoiceSetCommand(request.SourceEntityKind, request.SourceEntityId, request.Code, request.MinimumChoices, request.MaximumChoices), cancellationToken);
        return result.IsSuccess ? Results.Created($"/api/compendium/choice-sets/{result.Value.Code}", result.Value) : HttpErrorMapper.ToProblem(result.Error);
    }

    private static async Task<IResult> GetChoiceSetDetails(string code, GetChoiceSetDetailsQuery query, CancellationToken cancellationToken)
    {
        var result = await query.ExecuteAsync(code, cancellationToken);
        return result.IsSuccess ? Results.Ok(result.Value) : HttpErrorMapper.ToProblem(result.Error);
    }

    private static async Task<IResult> ListChoiceSetsBySourceEntity(CompendiumEntityKind entityKind, Guid entityId, ListChoiceSetsBySourceEntityQuery query, CancellationToken cancellationToken)
    {
        var result = await query.ExecuteAsync(entityKind, entityId, cancellationToken);
        return result.IsSuccess ? Results.Ok(result.Value) : HttpErrorMapper.ToProblem(result.Error);
    }

    private static async Task<IResult> AddChoiceOption(string code, AddChoiceOptionRequest request, AddChoiceOptionUseCase useCase, CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(new AddChoiceOptionCommand(code, request.Type, request.ReferenceId, request.DisplayText, request.SortOrder), cancellationToken);
        return result.IsSuccess ? Results.Ok(result.Value) : HttpErrorMapper.ToProblem(result.Error);
    }

    private static async Task<IResult> AddChoiceFilter(string code, AddChoiceFilterRequest request, AddChoiceFilterUseCase useCase, CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(new AddChoiceFilterCommand(code, request.Type, request.ValueType, request.TextValue, request.NumericValue, request.BooleanValue, request.ReferenceId, request.EnumValue), cancellationToken);
        return result.IsSuccess ? Results.Ok(result.Value) : HttpErrorMapper.ToProblem(result.Error);
    }
}

public sealed record CreateFeatureRequest(Guid RuleSourceId, Guid SourceVersionId, string Code, string Name, string? Description, int? LevelRequirement);
public sealed record UpdateFeatureRequest(Guid RuleSourceId, Guid SourceVersionId, string Name, string? Description, int? LevelRequirement);
public sealed record CreateEffectSchemaRequest(string Code, string Name, EffectType Type, IReadOnlyCollection<EffectSchemaFieldCommand> Fields);
public sealed record AttachEffectToFeatureRequest(string EffectSchemaCode, EffectType Type, EffectTarget Target, IReadOnlyCollection<TypedValueFieldCommand> Fields, IReadOnlyCollection<FeatureEffectConditionCommand> Conditions);
public sealed record AddPrerequisiteToEntityRequest(CompendiumEntityKind EntityKind, Guid EntityId, PrerequisiteType Type, ComparisonOperator Operator, EffectTarget Target, EffectValueType ValueType, string? TextValue, decimal? NumericValue, bool? BooleanValue, Guid? ReferenceId, string? EnumValue);
public sealed record CreateChoiceSetRequest(CompendiumEntityKind SourceEntityKind, Guid SourceEntityId, string Code, int MinimumChoices, int MaximumChoices);
public sealed record AddChoiceOptionRequest(ChoiceOptionType Type, Guid? ReferenceId, string? DisplayText, int SortOrder);
public sealed record AddChoiceFilterRequest(ChoiceFilterType Type, EffectValueType ValueType, string? TextValue, decimal? NumericValue, bool? BooleanValue, Guid? ReferenceId, string? EnumValue);
