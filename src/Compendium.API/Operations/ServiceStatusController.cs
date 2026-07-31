using Compendium.API.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace Compendium.API.Operations;

[Route("/")]
public sealed class ServiceStatusController : CompendiumControllerBase
{
    [HttpGet(Name = "GetServiceStatus")]
    public IActionResult Get() =>
        Ok(new ServiceStatusResponse(
            "dnd-compendium-service",
            "running"));
}

public sealed record ServiceStatusResponse(string Service, string Status);
