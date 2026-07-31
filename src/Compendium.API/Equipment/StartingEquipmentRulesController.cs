using Compendium.API.Controllers;
using Compendium.API.Security;
using Compendium.Application.Equipment;
using Compendium.Domain.Equipment;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Compendium.API.Equipment;

[Tags("Equipment")]
[Route("/api/compendium/equipment/starting-rules")]
public sealed class StartingEquipmentRulesController(
    CreateStartingEquipmentRuleUseCase createStartingEquipmentRule,
    GetStartingEquipmentRuleQuery getStartingEquipmentRule)
    : CompendiumControllerBase
{
    [HttpPost]
    [AdministrativeWrite]
    public async Task<IActionResult> Create(
        CreateStartingEquipmentRuleCommand request,
        CancellationToken cancellationToken)
    {
        var result = await createStartingEquipmentRule.ExecuteAsync(
            request,
            cancellationToken);

        return OkOrProblem(result);
    }

    [HttpGet("{ownerType}/{ownerId:guid}")]
    public async Task<IActionResult> Get(
        StartingEquipmentOwnerType ownerType,
        Guid ownerId,
        CancellationToken cancellationToken)
    {
        var result = await getStartingEquipmentRule.ExecuteAsync(
            ownerType,
            ownerId,
            cancellationToken);

        return OkOrProblem(result);
    }
}
