using Compendium.API.Controllers;
using Compendium.API.Security;
using Compendium.Application.Equipment;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Compendium.API.Equipment;

[Tags("Equipment")]
[Route("/api/compendium/equipment/armors")]
public sealed class ArmorsController(
    CreateArmorUseCase createArmor,
    ConfigureArmorAcRuleUseCase configureArmorAcRule,
    ListArmorsQuery listArmors,
    GetArmorDetailsQuery getArmorDetails) : CompendiumControllerBase
{
    [HttpPost]
    [AdministrativeWrite]
    public async Task<IActionResult> Create(
        CreateArmorCommand request,
        CancellationToken cancellationToken)
    {
        var result = await createArmor.ExecuteAsync(request, cancellationToken);
        return OkOrProblem(result);
    }

    [HttpPut("{equipmentItemId:guid}/ac-rule")]
    [AdministrativeWrite]
    public async Task<IActionResult> ConfigureAcRule(
        Guid equipmentItemId,
        ConfigureArmorRequest request,
        CancellationToken cancellationToken)
    {
        var result = await configureArmorAcRule.ExecuteAsync(
            new ConfigureArmorAcRuleCommand(
                equipmentItemId,
                request.BaseAc,
                request.AddsDexterity,
                request.MaximumDexterityBonus,
                request.Bonus,
                request.Drawbacks),
            cancellationToken);

        return NoContentOrProblem(result);
    }

    [HttpGet]
    public async Task<IActionResult> List(CancellationToken cancellationToken)
    {
        var result = await listArmors.ExecuteAsync(cancellationToken);
        return OkOrProblem(result);
    }

    [HttpGet("{equipmentItemId:guid}")]
    public async Task<IActionResult> GetDetails(
        Guid equipmentItemId,
        CancellationToken cancellationToken)
    {
        var result = await getArmorDetails.ExecuteAsync(
            equipmentItemId,
            cancellationToken);

        return OkOrProblem(result);
    }
}

public sealed record ConfigureArmorRequest(
    int BaseAc,
    bool AddsDexterity,
    int? MaximumDexterityBonus,
    int Bonus,
    IReadOnlyCollection<ArmorDrawbackCommand> Drawbacks);
