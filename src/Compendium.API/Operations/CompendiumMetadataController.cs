using Compendium.API.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace Compendium.API.Operations;

[Route("/internal/compendium/metadata")]
public sealed class CompendiumMetadataController : CompendiumControllerBase
{
    [HttpGet(Name = "GetCompendiumMetadata")]
    public IActionResult Get() =>
        Ok(new CompendiumMetadataResponse(
            "dnd-compendium-service",
            "compendium",
            "v1"));
}

public sealed record CompendiumMetadataResponse(
    string Service,
    string DatabaseSchema,
    string ApiVersion);
