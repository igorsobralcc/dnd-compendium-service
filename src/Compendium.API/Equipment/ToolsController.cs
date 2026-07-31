using Compendium.API.Controllers;
using Compendium.API.Security;
using Compendium.Application.Equipment;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Compendium.API.Equipment;

[Tags("Equipment")]
[Route("/api/compendium/equipment/tools")]
public sealed class ToolsController(CreateToolUseCase createTool)
    : CompendiumControllerBase
{
    [HttpPost]
    [AdministrativeWrite]
    public async Task<IActionResult> Create(
        CreateToolCommand request,
        CancellationToken cancellationToken)
    {
        var result = await createTool.ExecuteAsync(request, cancellationToken);

        return ToActionResult(
            result,
            id => Created(
                $"/api/compendium/equipment/tools/{id}",
                new { id }));
    }
}
