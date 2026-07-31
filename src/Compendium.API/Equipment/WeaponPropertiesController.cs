using Compendium.API.Controllers;
using Compendium.API.Security;
using Compendium.Application.Equipment;
using Compendium.Domain.Equipment;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Compendium.API.Equipment;

[Tags("Equipment")]
[Route("/api/compendium/equipment/weapons/properties")]
public sealed class WeaponPropertiesController(
    CreateWeaponPropertyUseCase createWeaponProperty) : CompendiumControllerBase
{
    [HttpPost]
    [AdministrativeWrite]
    public async Task<IActionResult> Create(
        CreateWeaponPropertyRequest request,
        CancellationToken cancellationToken)
    {
        var result = await createWeaponProperty.ExecuteAsync(
            new CreateWeaponPropertyCommand(
                request.Code,
                request.Name,
                request.ValueType,
                request.Rules),
            cancellationToken);

        return ToActionResult(
            result,
            id => Created(
                $"/api/compendium/equipment/weapons/properties/{id}",
                new { id }));
    }
}

public sealed record CreateWeaponPropertyRequest(
    string Code,
    string Name,
    WeaponPropertyValueType ValueType,
    IReadOnlyCollection<WeaponPropertyRuleCommand> Rules);
