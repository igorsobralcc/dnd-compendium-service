using Compendium.API.Controllers;
using Compendium.API.Security;
using Compendium.Application.Fundamentals;
using Compendium.Domain.Fundamentals;
using Microsoft.AspNetCore.Mvc;

namespace Compendium.API.Fundamentals;

[Route("/api/compendium/proficiencies")]
public sealed class ProficienciesController(
    CreateProficiencyUseCase createProficiency,
    UpdateProficiencyUseCase updateProficiency,
    ListProficienciesQuery listProficiencies) : CompendiumControllerBase
{
    [HttpPost(Name = "CreateProficiency")]
    [AdministrativeWrite]
    public async Task<IActionResult> Create(
        CreateProficiencyRequest request,
        CancellationToken cancellationToken)
    {
        var result = await createProficiency.ExecuteAsync(
            new CreateProficiencyCommand(
                request.RuleSourceId,
                request.SourceVersionId,
                request.Code,
                request.Name,
                request.Type,
                request.RelatedEntityId),
            cancellationToken);

        return CreatedOrProblem(
            result,
            proficiency => $"/api/compendium/proficiencies/{proficiency.Code}");
    }

    [HttpPut("{code}", Name = "UpdateProficiency")]
    [AdministrativeWrite]
    public async Task<IActionResult> Update(
        string code,
        UpdateProficiencyRequest request,
        CancellationToken cancellationToken)
    {
        var result = await updateProficiency.ExecuteAsync(
            new UpdateProficiencyCommand(
                code,
                request.RuleSourceId,
                request.SourceVersionId,
                request.Name,
                request.Type,
                request.RelatedEntityId),
            cancellationToken);

        return OkOrProblem(result);
    }

    [HttpGet(Name = "ListProficiencies")]
    public async Task<IActionResult> List(
        ProficiencyType? type,
        CancellationToken cancellationToken)
    {
        var result = await listProficiencies.ExecuteAsync(type, cancellationToken);
        return OkOrProblem(result);
    }
}

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
