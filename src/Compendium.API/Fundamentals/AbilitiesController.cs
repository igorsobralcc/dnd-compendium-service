using Compendium.API.Controllers;
using Compendium.API.Security;
using Compendium.Application.Fundamentals;
using Microsoft.AspNetCore.Mvc;

namespace Compendium.API.Fundamentals;

[Route("/api/compendium/abilities")]
public sealed class AbilitiesController(
    CreateAbilityUseCase createAbility,
    UpdateAbilityUseCase updateAbility,
    ListAbilitiesQuery listAbilities) : CompendiumControllerBase
{
    [HttpPost(Name = "CreateAbility")]
    [AdministrativeWrite]
    public async Task<IActionResult> Create(
        CreateAbilityRequest request,
        CancellationToken cancellationToken)
    {
        var result = await createAbility.ExecuteAsync(
            new CreateAbilityCommand(
                request.RuleSourceId,
                request.SourceVersionId,
                request.Code,
                request.Name),
            cancellationToken);

        return CreatedOrProblem(
            result,
            ability => $"/api/compendium/abilities/{ability.Code}");
    }

    [HttpPut("{code}", Name = "UpdateAbility")]
    [AdministrativeWrite]
    public async Task<IActionResult> Update(
        string code,
        UpdateAbilityRequest request,
        CancellationToken cancellationToken)
    {
        var result = await updateAbility.ExecuteAsync(
            new UpdateAbilityCommand(
                code,
                request.RuleSourceId,
                request.SourceVersionId,
                request.Name),
            cancellationToken);

        return OkOrProblem(result);
    }

    [HttpGet(Name = "ListAbilities")]
    public async Task<IActionResult> List(CancellationToken cancellationToken)
    {
        var result = await listAbilities.ExecuteAsync(cancellationToken);
        return OkOrProblem(result);
    }
}

public sealed record CreateAbilityRequest(
    Guid RuleSourceId,
    Guid SourceVersionId,
    string Code,
    string Name);

public sealed record UpdateAbilityRequest(
    Guid RuleSourceId,
    Guid SourceVersionId,
    string Name);
