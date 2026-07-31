using Compendium.API.Controllers;
using Compendium.API.Security;
using Compendium.Application.InternalQueries;
using Microsoft.AspNetCore.Mvc;

namespace Compendium.API.InternalQueries;

[Tags("Internal Compendium")]
[InternalRead]
[Route("/internal/compendium/entities")]
public sealed class MechanicalEntitiesController(
    GetMechanicalEntityDetailsQuery getMechanicalEntityDetails)
    : CompendiumControllerBase
{
    [HttpGet("{entityType}/{entityId:guid}/mechanics", Name = "GetMechanicalEntityDetailsV1")]
    [ProducesResponseType<MechanicalEntityDetailsV1>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetDetails(
        string entityType,
        Guid entityId,
        string? locale,
        CancellationToken cancellationToken)
    {
        var result = await getMechanicalEntityDetails.ExecuteAsync(
            entityType,
            entityId,
            locale ?? "en-US",
            cancellationToken);

        return result is null
            ? NotFound()
            : Ok(result);
    }
}
