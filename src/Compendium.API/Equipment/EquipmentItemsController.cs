using Compendium.API.Controllers;
using Compendium.API.Security;
using Compendium.Application.Equipment;
using Compendium.Domain.Equipment;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Compendium.API.Equipment;

[Tags("Equipment")]
[Route("/api/compendium/equipment/")]
public sealed class EquipmentItemsController(
    CreateEquipmentItemUseCase createEquipmentItem,
    UpdateEquipmentItemUseCase updateEquipmentItem,
    ListEquipmentItemsQuery listEquipmentItems,
    GetEquipmentItemDetailsQuery getEquipmentItemDetails)
    : CompendiumControllerBase
{
    [HttpPost]
    [AdministrativeWrite]
    public async Task<IActionResult> Create(
        CreateEquipmentItemRequest request,
        CancellationToken cancellationToken)
    {
        var result = await createEquipmentItem.ExecuteAsync(
            new CreateEquipmentItemCommand(
                request.RuleSourceId,
                request.SourceVersionId,
                request.Code,
                request.Name,
                request.Category,
                request.Weight,
                request.CostAmount,
                request.CostCurrency,
                request.Description),
            cancellationToken);

        return CreatedOrProblem(
            result,
            item => $"/api/compendium/equipment/{item.Code}");
    }

    [HttpPut("{code}")]
    [AdministrativeWrite]
    public async Task<IActionResult> Update(
        string code,
        UpdateEquipmentItemRequest request,
        CancellationToken cancellationToken)
    {
        var result = await updateEquipmentItem.ExecuteAsync(
            new UpdateEquipmentItemCommand(
                code,
                request.RuleSourceId,
                request.SourceVersionId,
                request.Name,
                request.Category,
                request.Weight,
                request.CostAmount,
                request.CostCurrency,
                request.Description),
            cancellationToken);

        return OkOrProblem(result);
    }

    [HttpGet]
    public async Task<IActionResult> List(
        EquipmentCategory? category,
        CancellationToken cancellationToken)
    {
        var result = await listEquipmentItems.ExecuteAsync(
            category,
            cancellationToken);

        return OkOrProblem(result);
    }

    [HttpGet("{code}")]
    public async Task<IActionResult> GetDetails(
        string code,
        CancellationToken cancellationToken)
    {
        var result = await getEquipmentItemDetails.ExecuteAsync(
            code,
            cancellationToken);

        return OkOrProblem(result);
    }
}

public sealed record CreateEquipmentItemRequest(
    Guid RuleSourceId,
    Guid SourceVersionId,
    string Code,
    string Name,
    EquipmentCategory Category,
    decimal Weight,
    decimal CostAmount,
    Currency CostCurrency,
    string? Description);

public sealed record UpdateEquipmentItemRequest(
    Guid RuleSourceId,
    Guid SourceVersionId,
    string Name,
    EquipmentCategory Category,
    decimal Weight,
    decimal CostAmount,
    Currency CostCurrency,
    string? Description);
