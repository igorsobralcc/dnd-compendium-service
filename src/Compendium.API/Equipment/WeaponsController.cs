using Compendium.API.Controllers;
using Compendium.API.Security;
using Compendium.Application.Equipment;
using Compendium.Domain.Equipment;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Compendium.API.Equipment;

[Tags("Equipment")]
[Route("/api/compendium/equipment/weapons")]
public sealed class WeaponsController(
    CreateWeaponUseCase createWeapon,
    AttachWeaponPropertyUseCase attachWeaponProperty,
    ListWeaponsQuery listWeapons,
    GetWeaponDetailsQuery getWeaponDetails) : CompendiumControllerBase
{
    [HttpPost]
    [AdministrativeWrite]
    public async Task<IActionResult> Create(
        CreateWeaponRequest request,
        CancellationToken cancellationToken)
    {
        var result = await createWeapon.ExecuteAsync(
            new CreateWeaponCommand(
                request.EquipmentItemId,
                request.Category,
                request.DamageDice,
                request.DamageType),
            cancellationToken);

        return OkOrProblem(result);
    }

    [HttpPost("{equipmentItemId:guid}/properties")]
    [AdministrativeWrite]
    public async Task<IActionResult> AttachProperty(
        Guid equipmentItemId,
        AttachWeaponPropertyRequest request,
        CancellationToken cancellationToken)
    {
        var result = await attachWeaponProperty.ExecuteAsync(
            new AttachWeaponPropertyCommand(
                equipmentItemId,
                request.WeaponPropertyId,
                request.Values),
            cancellationToken);

        return NoContentOrProblem(result);
    }

    [HttpGet]
    public async Task<IActionResult> List(CancellationToken cancellationToken)
    {
        var result = await listWeapons.ExecuteAsync(cancellationToken);
        return OkOrProblem(result);
    }

    [HttpGet("{equipmentItemId:guid}")]
    public async Task<IActionResult> GetDetails(
        Guid equipmentItemId,
        CancellationToken cancellationToken)
    {
        var result = await getWeaponDetails.ExecuteAsync(
            equipmentItemId,
            cancellationToken);

        return OkOrProblem(result);
    }
}

public sealed record CreateWeaponRequest(
    Guid EquipmentItemId,
    WeaponCategory Category,
    string DamageDice,
    DamageType DamageType);

public sealed record AttachWeaponPropertyRequest(
    Guid WeaponPropertyId,
    IReadOnlyCollection<string> Values);
