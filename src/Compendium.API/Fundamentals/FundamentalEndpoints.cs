using Compendium.API.Errors;
using Compendium.Application.Fundamentals;
using Compendium.Domain.Fundamentals;

namespace Compendium.API.Fundamentals;

public static class FundamentalEndpoints
{
    public static IEndpointRouteBuilder MapFundamentalEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/compendium");

        group.MapPost("/abilities", CreateAbility).WithName("CreateAbility");
        group.MapPut("/abilities/{code}", UpdateAbility).WithName("UpdateAbility");
        group.MapGet("/abilities", ListAbilities).WithName("ListAbilities");

        group.MapPost("/skills", CreateSkill).WithName("CreateSkill");
        group.MapPut("/skills/{code}", UpdateSkill).WithName("UpdateSkill");
        group.MapGet("/skills", ListSkills).WithName("ListSkills");

        group.MapPost("/languages", CreateLanguage).WithName("CreateLanguage");
        group.MapPut("/languages/{code}", UpdateLanguage).WithName("UpdateLanguage");
        group.MapGet("/languages", ListLanguages).WithName("ListLanguages");

        group.MapPost("/proficiencies", CreateProficiency).WithName("CreateProficiency");
        group.MapPut("/proficiencies/{code}", UpdateProficiency).WithName("UpdateProficiency");
        group.MapGet("/proficiencies", ListProficiencies).WithName("ListProficiencies");

        group.MapPost("/armor-training-categories", CreateArmorTrainingCategory).WithName("CreateArmorTrainingCategory");
        group.MapGet("/armor-training-categories", ListArmorTrainingCategories).WithName("ListArmorTrainingCategories");

        group.MapPost("/hit-dice", CreateHitDie).WithName("CreateHitDie");
        group.MapGet("/hit-dice", ListHitDice).WithName("ListHitDice");

        group.MapPost("/ability-score-methods", CreateAbilityScoreMethod).WithName("CreateAbilityScoreMethod");
        group.MapGet("/ability-score-methods", ListAbilityScoreMethods).WithName("ListAbilityScoreMethods");

        return endpoints;
    }

    private static async Task<IResult> CreateAbility(
        CreateAbilityRequest request,
        CreateAbilityUseCase useCase,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(
            new CreateAbilityCommand(request.RuleSourceId, request.SourceVersionId, request.Code, request.Name),
            cancellationToken);

        return result.IsSuccess
            ? Results.Created($"/api/compendium/abilities/{result.Value.Code}", result.Value)
            : HttpErrorMapper.ToProblem(result.Error);
    }

    private static async Task<IResult> UpdateAbility(
        string code,
        UpdateAbilityRequest request,
        UpdateAbilityUseCase useCase,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(
            new UpdateAbilityCommand(code, request.RuleSourceId, request.SourceVersionId, request.Name),
            cancellationToken);

        return result.IsSuccess ? Results.Ok(result.Value) : HttpErrorMapper.ToProblem(result.Error);
    }

    private static async Task<IResult> ListAbilities(ListAbilitiesQuery query, CancellationToken cancellationToken)
    {
        var result = await query.ExecuteAsync(cancellationToken);
        return result.IsSuccess ? Results.Ok(result.Value) : HttpErrorMapper.ToProblem(result.Error);
    }

    private static async Task<IResult> CreateSkill(
        CreateSkillRequest request,
        CreateSkillUseCase useCase,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(
            new CreateSkillCommand(
                request.RuleSourceId,
                request.SourceVersionId,
                request.Code,
                request.Name,
                request.DefaultAbilityId),
            cancellationToken);

        return result.IsSuccess
            ? Results.Created($"/api/compendium/skills/{result.Value.Code}", result.Value)
            : HttpErrorMapper.ToProblem(result.Error);
    }

    private static async Task<IResult> UpdateSkill(
        string code,
        UpdateSkillRequest request,
        UpdateSkillUseCase useCase,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(
            new UpdateSkillCommand(code, request.RuleSourceId, request.SourceVersionId, request.Name, request.DefaultAbilityId),
            cancellationToken);

        return result.IsSuccess ? Results.Ok(result.Value) : HttpErrorMapper.ToProblem(result.Error);
    }

    private static async Task<IResult> ListSkills(ListSkillsQuery query, CancellationToken cancellationToken)
    {
        var result = await query.ExecuteAsync(cancellationToken);
        return result.IsSuccess ? Results.Ok(result.Value) : HttpErrorMapper.ToProblem(result.Error);
    }

    private static async Task<IResult> CreateLanguage(
        CreateLanguageRequest request,
        CreateLanguageUseCase useCase,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(
            new CreateLanguageCommand(request.RuleSourceId, request.SourceVersionId, request.Code, request.Name),
            cancellationToken);

        return result.IsSuccess
            ? Results.Created($"/api/compendium/languages/{result.Value.Code}", result.Value)
            : HttpErrorMapper.ToProblem(result.Error);
    }

    private static async Task<IResult> UpdateLanguage(
        string code,
        UpdateLanguageRequest request,
        UpdateLanguageUseCase useCase,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(
            new UpdateLanguageCommand(code, request.RuleSourceId, request.SourceVersionId, request.Name),
            cancellationToken);

        return result.IsSuccess ? Results.Ok(result.Value) : HttpErrorMapper.ToProblem(result.Error);
    }

    private static async Task<IResult> ListLanguages(ListLanguagesQuery query, CancellationToken cancellationToken)
    {
        var result = await query.ExecuteAsync(cancellationToken);
        return result.IsSuccess ? Results.Ok(result.Value) : HttpErrorMapper.ToProblem(result.Error);
    }

    private static async Task<IResult> CreateProficiency(
        CreateProficiencyRequest request,
        CreateProficiencyUseCase useCase,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(
            new CreateProficiencyCommand(
                request.RuleSourceId,
                request.SourceVersionId,
                request.Code,
                request.Name,
                request.Type,
                request.RelatedEntityId),
            cancellationToken);

        return result.IsSuccess
            ? Results.Created($"/api/compendium/proficiencies/{result.Value.Code}", result.Value)
            : HttpErrorMapper.ToProblem(result.Error);
    }

    private static async Task<IResult> UpdateProficiency(
        string code,
        UpdateProficiencyRequest request,
        UpdateProficiencyUseCase useCase,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(
            new UpdateProficiencyCommand(
                code,
                request.RuleSourceId,
                request.SourceVersionId,
                request.Name,
                request.Type,
                request.RelatedEntityId),
            cancellationToken);

        return result.IsSuccess ? Results.Ok(result.Value) : HttpErrorMapper.ToProblem(result.Error);
    }

    private static async Task<IResult> ListProficiencies(
        ProficiencyType? type,
        ListProficienciesQuery query,
        CancellationToken cancellationToken)
    {
        var result = await query.ExecuteAsync(type, cancellationToken);
        return result.IsSuccess ? Results.Ok(result.Value) : HttpErrorMapper.ToProblem(result.Error);
    }

    private static async Task<IResult> CreateArmorTrainingCategory(
        CreateArmorTrainingCategoryRequest request,
        CreateArmorTrainingCategoryUseCase useCase,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(
            new CreateArmorTrainingCategoryCommand(
                request.RuleSourceId,
                request.SourceVersionId,
                request.Code,
                request.Name,
                request.SortOrder),
            cancellationToken);

        return result.IsSuccess
            ? Results.Created($"/api/compendium/armor-training-categories/{result.Value.Code}", result.Value)
            : HttpErrorMapper.ToProblem(result.Error);
    }

    private static async Task<IResult> ListArmorTrainingCategories(
        ListArmorTrainingCategoriesQuery query,
        CancellationToken cancellationToken)
    {
        var result = await query.ExecuteAsync(cancellationToken);
        return result.IsSuccess ? Results.Ok(result.Value) : HttpErrorMapper.ToProblem(result.Error);
    }

    private static async Task<IResult> CreateHitDie(
        CreateHitDieRequest request,
        CreateHitDieUseCase useCase,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(new CreateHitDieCommand(request.RuleSourceId, request.SourceVersionId, request.Die), cancellationToken);

        return result.IsSuccess
            ? Results.Created($"/api/compendium/hit-dice/{result.Value.Code}", result.Value)
            : HttpErrorMapper.ToProblem(result.Error);
    }

    private static async Task<IResult> ListHitDice(ListHitDiceQuery query, CancellationToken cancellationToken)
    {
        var result = await query.ExecuteAsync(cancellationToken);
        return result.IsSuccess ? Results.Ok(result.Value) : HttpErrorMapper.ToProblem(result.Error);
    }

    private static async Task<IResult> CreateAbilityScoreMethod(
        CreateAbilityScoreMethodRequest request,
        CreateAbilityScoreMethodUseCase useCase,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(
            new CreateAbilityScoreMethodCommand(
                request.RuleSourceId,
                request.SourceVersionId,
                request.Code,
                request.Name,
                request.Type,
                request.Rules,
                request.StandardValues,
                request.PointBuyCosts,
                request.RollRule),
            cancellationToken);

        return result.IsSuccess
            ? Results.Created($"/api/compendium/ability-score-methods/{result.Value.Code}", result.Value)
            : HttpErrorMapper.ToProblem(result.Error);
    }

    private static async Task<IResult> ListAbilityScoreMethods(
        ListAbilityScoreMethodsQuery query,
        CancellationToken cancellationToken)
    {
        var result = await query.ExecuteAsync(cancellationToken);
        return result.IsSuccess ? Results.Ok(result.Value) : HttpErrorMapper.ToProblem(result.Error);
    }
}

public sealed record CreateAbilityRequest(Guid RuleSourceId, Guid SourceVersionId, string Code, string Name);

public sealed record UpdateAbilityRequest(Guid RuleSourceId, Guid SourceVersionId, string Name);

public sealed record CreateSkillRequest(Guid RuleSourceId, Guid SourceVersionId, string Code, string Name, Guid? DefaultAbilityId);

public sealed record UpdateSkillRequest(Guid RuleSourceId, Guid SourceVersionId, string Name, Guid? DefaultAbilityId);

public sealed record CreateLanguageRequest(Guid RuleSourceId, Guid SourceVersionId, string Code, string Name);

public sealed record UpdateLanguageRequest(Guid RuleSourceId, Guid SourceVersionId, string Name);

public sealed record CreateProficiencyRequest(
    Guid RuleSourceId,
    Guid SourceVersionId,
    string Code,
    string Name,
    ProficiencyType Type,
    Guid? RelatedEntityId);

public sealed record UpdateProficiencyRequest(
    Guid RuleSourceId,
    Guid SourceVersionId,
    string Name,
    ProficiencyType Type,
    Guid? RelatedEntityId);

public sealed record CreateArmorTrainingCategoryRequest(
    Guid RuleSourceId,
    Guid SourceVersionId,
    string Code,
    string Name,
    int SortOrder);

public sealed record CreateHitDieRequest(Guid RuleSourceId, Guid SourceVersionId, int Die);

public sealed record CreateAbilityScoreMethodRequest(
    Guid RuleSourceId,
    Guid SourceVersionId,
    string Code,
    string Name,
    AbilityScoreMethodType Type,
    IReadOnlyCollection<CreateAbilityScoreMethodRuleCommand> Rules,
    IReadOnlyCollection<int> StandardValues,
    IReadOnlyCollection<CreateAbilityScorePointBuyCostCommand> PointBuyCosts,
    CreateAbilityScoreRollRuleCommand? RollRule);
