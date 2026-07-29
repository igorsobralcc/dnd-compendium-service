using Compendium.API.Controllers;
using Compendium.API.Security;
using Compendium.Application.Fundamentals;
using Microsoft.AspNetCore.Mvc;

namespace Compendium.API.Fundamentals;

[Route("/api/compendium/hit-dice")]
public sealed class HitDiceController(
    CreateHitDieUseCase createHitDie,
    ListHitDiceQuery listHitDice) : CompendiumControllerBase
{
    [HttpPost(Name = "CreateHitDie")]
    [AdministrativeWrite]
    public async Task<IActionResult> Create(
        CreateHitDieRequest request,
        CancellationToken cancellationToken)
    {
        var result = await createHitDie.ExecuteAsync(
            new CreateHitDieCommand(
                request.RuleSourceId,
                request.SourceVersionId,
                request.Die),
            cancellationToken);

        return CreatedOrProblem(
            result,
            hitDie => $"/api/compendium/hit-dice/{hitDie.Code}");
    }

    [HttpGet(Name = "ListHitDice")]
    public async Task<IActionResult> List(CancellationToken cancellationToken)
    {
        var result = await listHitDice.ExecuteAsync(cancellationToken);
        return OkOrProblem(result);
    }
}

public sealed record CreateHitDieRequest(
    Guid RuleSourceId,
    Guid SourceVersionId,
    int Die);
