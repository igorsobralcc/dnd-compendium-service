using Compendium.API.Controllers;
using Compendium.API.Security;
using Compendium.Application.Equipment;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Compendium.API.Equipment;

[Tags("Equipment")]
[Route("/api/compendium/equipment/weapons/masteries")]
public sealed class WeaponMasteriesController(
    ConfigureWeaponMasteryUseCase configureWeaponMastery)
    : CompendiumControllerBase
{
    [HttpPost]
    [AdministrativeWrite]
    public async Task<IActionResult> Create(
        ConfigureWeaponMasteryCommand request,
        CancellationToken cancellationToken)
    {
        var result = await configureWeaponMastery.ExecuteAsync(
            request,
            cancellationToken);

        return ToActionResult(
            result,
            id => Created(
                $"/api/compendium/equipment/weapons/masteries/{id}",
                new { id }));
    }
}
