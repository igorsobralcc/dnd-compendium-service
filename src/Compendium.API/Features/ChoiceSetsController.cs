using Compendium.API.Controllers;
using Compendium.API.Security;
using Compendium.Application.Features;
using Compendium.Domain.Features;
using Microsoft.AspNetCore.Mvc;

namespace Compendium.API.Features;

[Route("/api/compendium/choice-sets")]
public sealed class ChoiceSetsController(
    CreateChoiceSetUseCase createChoiceSet,
    AddChoiceOptionUseCase addChoiceOption,
    AddChoiceFilterUseCase addChoiceFilter,
    GetChoiceSetDetailsQuery getChoiceSetDetails,
    ListChoiceSetsBySourceEntityQuery listChoiceSetsBySourceEntity)
    : CompendiumControllerBase
{
    [HttpPost(Name = "CreateChoiceSet")]
    [AdministrativeWrite]
    public async Task<IActionResult> Create(
        CreateChoiceSetRequest request,
        CancellationToken cancellationToken)
    {
        var result = await createChoiceSet.ExecuteAsync(
            new CreateChoiceSetCommand(
                request.SourceEntityKind,
                request.SourceEntityId,
                request.Code,
                request.MinimumChoices,
                request.MaximumChoices),
            cancellationToken);

        return CreatedOrProblem(
            result,
            choiceSet => $"/api/compendium/choice-sets/{choiceSet.Code}");
    }

    [HttpGet("{code}", Name = "GetChoiceSetDetails")]
    public async Task<IActionResult> GetDetails(
        string code,
        CancellationToken cancellationToken)
    {
        var result = await getChoiceSetDetails.ExecuteAsync(code, cancellationToken);
        return OkOrProblem(result);
    }

    [HttpGet(
        "by-source/{entityKind}/{entityId:guid}",
        Name = "ListChoiceSetsBySourceEntity")]
    public async Task<IActionResult> ListBySourceEntity(
        CompendiumEntityKind entityKind,
        Guid entityId,
        CancellationToken cancellationToken)
    {
        var result = await listChoiceSetsBySourceEntity.ExecuteAsync(
            entityKind,
            entityId,
            cancellationToken);

        return OkOrProblem(result);
    }

    [HttpPost("{code}/options", Name = "AddChoiceOption")]
    [AdministrativeWrite]
    public async Task<IActionResult> AddOption(
        string code,
        AddChoiceOptionRequest request,
        CancellationToken cancellationToken)
    {
        var result = await addChoiceOption.ExecuteAsync(
            new AddChoiceOptionCommand(
                code,
                request.Type,
                request.ReferenceId,
                request.DisplayText,
                request.SortOrder),
            cancellationToken);

        return OkOrProblem(result);
    }

    [HttpPost("{code}/filters", Name = "AddChoiceFilter")]
    [AdministrativeWrite]
    public async Task<IActionResult> AddFilter(
        string code,
        AddChoiceFilterRequest request,
        CancellationToken cancellationToken)
    {
        var result = await addChoiceFilter.ExecuteAsync(
            new AddChoiceFilterCommand(
                code,
                request.Type,
                request.ValueType,
                request.TextValue,
                request.NumericValue,
                request.BooleanValue,
                request.ReferenceId,
                request.EnumValue),
            cancellationToken);

        return OkOrProblem(result);
    }
}

public sealed record CreateChoiceSetRequest(
    CompendiumEntityKind SourceEntityKind,
    Guid SourceEntityId,
    string Code,
    int MinimumChoices,
    int MaximumChoices);

public sealed record AddChoiceOptionRequest(
    ChoiceOptionType Type,
    Guid? ReferenceId,
    string? DisplayText,
    int SortOrder);

public sealed record AddChoiceFilterRequest(
    ChoiceFilterType Type,
    EffectValueType ValueType,
    string? TextValue,
    decimal? NumericValue,
    bool? BooleanValue,
    Guid? ReferenceId,
    string? EnumValue);
