using Compendium.API.Controllers;
using Compendium.API.Security;
using Compendium.Application.Features;
using Compendium.Domain.Features;
using Microsoft.AspNetCore.Mvc;

namespace Compendium.API.Features;

[Route("/api/compendium/effect-schemas")]
public sealed class EffectSchemasController(
    CreateEffectSchemaUseCase createEffectSchema) : CompendiumControllerBase
{
    [HttpPost(Name = "CreateEffectSchema")]
    [AdministrativeWrite]
    public async Task<IActionResult> Create(
        CreateEffectSchemaRequest request,
        CancellationToken cancellationToken)
    {
        var result = await createEffectSchema.ExecuteAsync(
            new CreateEffectSchemaCommand(
                request.Code,
                request.Name,
                request.Type,
                request.Fields),
            cancellationToken);

        return CreatedOrProblem(
            result,
            schema => $"/api/compendium/effect-schemas/{schema.Code}");
    }
}

public sealed record CreateEffectSchemaRequest(
    string Code,
    string Name,
    EffectType Type,
    IReadOnlyCollection<EffectSchemaFieldCommand> Fields);
