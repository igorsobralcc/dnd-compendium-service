using Compendium.API.Errors;
using Compendium.Application.Classes;
using Compendium.Domain.Classes;

namespace Compendium.API.Classes;

public static class ClassEndpoints
{
    public static IEndpointRouteBuilder MapClassEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/compendium");

        group.MapPost("/classes", CreateClass).WithName("CreateClass");
        group.MapPut("/classes/{code}", UpdateClass).WithName("UpdateClass");
        group.MapGet("/classes", ListClasses).WithName("ListClasses");
        group.MapGet("/classes/{code}", GetClassDetails).WithName("GetClassDetails");
        group.MapPut("/classes/{code}/progression", ConfigureClassProgression).WithName("ConfigureClassProgression");
        group.MapGet("/classes/{code}/progression", GetClassProgression).WithName("GetClassProgression");
        group.MapPost("/classes/{classCode}/subclasses", CreateSubclass).WithName("CreateSubclass");
        group.MapGet("/classes/{classCode}/subclasses", ListSubclassesByClass).WithName("ListSubclassesByClass");
        group.MapGet("/classes/{classCode}/subclasses/{subclassCode}", GetSubclassDetails).WithName("GetSubclassDetails");
        group.MapPost("/classes/{classCode}/subclasses/{subclassCode}/features", LinkSubclassFeature).WithName("LinkSubclassFeature");

        return endpoints;
    }

    private static async Task<IResult> CreateClass(
        CreateClassRequest request,
        CreateClassUseCase useCase,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(
            new CreateClassCommand(
                request.RuleSourceId,
                request.SourceVersionId,
                request.Code,
                request.Name,
                request.Description,
                request.CoreTraits,
                request.PrimaryAbilityIds,
                request.Levels),
            cancellationToken);

        return result.IsSuccess
            ? Results.Created($"/api/compendium/classes/{result.Value.Code}", result.Value)
            : HttpErrorMapper.ToProblem(result.Error);
    }

    private static async Task<IResult> UpdateClass(
        string code,
        UpdateClassRequest request,
        UpdateClassUseCase useCase,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(
            new UpdateClassCommand(
                code,
                request.RuleSourceId,
                request.SourceVersionId,
                request.Name,
                request.Description,
                request.CoreTraits,
                request.PrimaryAbilityIds),
            cancellationToken);

        return result.IsSuccess ? Results.Ok(result.Value) : HttpErrorMapper.ToProblem(result.Error);
    }

    private static async Task<IResult> ListClasses(ListClassesQuery query, CancellationToken cancellationToken)
    {
        var result = await query.ExecuteAsync(cancellationToken);
        return result.IsSuccess ? Results.Ok(result.Value) : HttpErrorMapper.ToProblem(result.Error);
    }

    private static async Task<IResult> GetClassDetails(
        string code,
        GetClassDetailsQuery query,
        CancellationToken cancellationToken)
    {
        var result = await query.ExecuteAsync(code, cancellationToken);
        return result.IsSuccess ? Results.Ok(result.Value) : HttpErrorMapper.ToProblem(result.Error);
    }

    private static async Task<IResult> ConfigureClassProgression(
        string code,
        ConfigureClassProgressionRequest request,
        ConfigureClassProgressionUseCase useCase,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(
            new ConfigureClassProgressionCommand(
                code,
                request.RuleSourceId,
                request.SourceVersionId,
                request.Levels,
                request.SpellcastingProgression),
            cancellationToken);

        return result.IsSuccess ? Results.Ok(result.Value) : HttpErrorMapper.ToProblem(result.Error);
    }

    private static async Task<IResult> GetClassProgression(
        string code,
        GetClassProgressionQuery query,
        CancellationToken cancellationToken)
    {
        var result = await query.ExecuteAsync(code, cancellationToken);
        return result.IsSuccess ? Results.Ok(result.Value) : HttpErrorMapper.ToProblem(result.Error);
    }

    private static async Task<IResult> CreateSubclass(
        string classCode,
        CreateSubclassRequest request,
        CreateSubclassUseCase useCase,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(
            new CreateSubclassCommand(
                classCode,
                request.RuleSourceId,
                request.SourceVersionId,
                request.Code,
                request.Name,
                request.Description),
            cancellationToken);

        return result.IsSuccess
            ? Results.Created($"/api/compendium/classes/{classCode}/subclasses/{result.Value.Code}", result.Value)
            : HttpErrorMapper.ToProblem(result.Error);
    }

    private static async Task<IResult> ListSubclassesByClass(
        string classCode,
        ListSubclassesByClassQuery query,
        CancellationToken cancellationToken)
    {
        var result = await query.ExecuteAsync(classCode, cancellationToken);
        return result.IsSuccess ? Results.Ok(result.Value) : HttpErrorMapper.ToProblem(result.Error);
    }

    private static async Task<IResult> GetSubclassDetails(
        string classCode,
        string subclassCode,
        GetSubclassDetailsQuery query,
        CancellationToken cancellationToken)
    {
        var result = await query.ExecuteAsync(classCode, subclassCode, cancellationToken);
        return result.IsSuccess ? Results.Ok(result.Value) : HttpErrorMapper.ToProblem(result.Error);
    }

    private static async Task<IResult> LinkSubclassFeature(
        string classCode,
        string subclassCode,
        LinkSubclassFeatureRequest request,
        LinkSubclassFeatureUseCase useCase,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(
            new LinkSubclassFeatureCommand(
                classCode,
                subclassCode,
                request.RuleSourceId,
                request.SourceVersionId,
                request.FeatureId,
                request.Level),
            cancellationToken);

        return result.IsSuccess ? Results.Ok(result.Value) : HttpErrorMapper.ToProblem(result.Error);
    }
}

public sealed record CreateClassRequest(
    Guid RuleSourceId,
    Guid SourceVersionId,
    string Code,
    string Name,
    string? Description,
    ClassCoreTraitsCommand CoreTraits,
    IReadOnlyCollection<Guid> PrimaryAbilityIds,
    IReadOnlyCollection<ClassLevelCommand> Levels);

public sealed record UpdateClassRequest(
    Guid RuleSourceId,
    Guid SourceVersionId,
    string Name,
    string? Description,
    ClassCoreTraitsCommand CoreTraits,
    IReadOnlyCollection<Guid> PrimaryAbilityIds);

public sealed record ConfigureClassProgressionRequest(
    Guid RuleSourceId,
    Guid SourceVersionId,
    IReadOnlyCollection<ClassLevelCommand> Levels,
    ClassSpellcastingProgressionCommand? SpellcastingProgression);

public sealed record CreateSubclassRequest(
    Guid RuleSourceId,
    Guid SourceVersionId,
    string Code,
    string Name,
    string? Description);

public sealed record LinkSubclassFeatureRequest(Guid RuleSourceId, Guid SourceVersionId, Guid FeatureId, int Level);
