using Compendium.API.Controllers;
using Compendium.API.Security;
using Compendium.Application.Equipment;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Compendium.API.Equipment;

[Tags("Equipment")]
[Route("/api/compendium/equipment/packs")]
public sealed class EquipmentPacksController(
    CreateEquipmentPackUseCase createEquipmentPack) : CompendiumControllerBase
{
    [HttpPost]
    [AdministrativeWrite]
    public async Task<IActionResult> Create(
        CreateEquipmentPackCommand request,
        CancellationToken cancellationToken)
    {
        var result = await createEquipmentPack.ExecuteAsync(
            request,
            cancellationToken);

        return ToActionResult(
            result,
            id => Created(
                $"/api/compendium/equipment/packs/{id}",
                new { id }));
    }
}
