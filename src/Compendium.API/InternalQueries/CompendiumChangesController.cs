using Compendium.API.Controllers;
using Compendium.API.Security;
using Compendium.Application.InternalQueries;
using Microsoft.AspNetCore.Mvc;

namespace Compendium.API.InternalQueries;

[Tags("Internal Compendium")]
[InternalRead]
[Route("/internal/compendium/changes")]
public sealed class CompendiumChangesController(
    ListCompendiumChangesSinceQuery listCompendiumChanges)
    : CompendiumControllerBase
{
    [HttpGet(Name = "ListCompendiumChangesV1")]
    [ProducesResponseType<CompendiumChangesV1>(StatusCodes.Status200OK)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> List(
        [FromQuery(Name = "source_version_id")] Guid? sourceVersionId,
        [FromQuery(Name = "entity_type")] string? entityType,
        [FromQuery(Name = "changed_since")] DateTimeOffset? changedSince,
        long? revision,
        int? page,
        [FromQuery(Name = "page_size")] int? pageSize,
        CancellationToken cancellationToken)
    {
        if (revision < 0 || page < 1 || pageSize is < 1 or > 200)
        {
            return ValidationProblem(new ValidationProblemDetails(
                new Dictionary<string, string[]>
                {
                    ["pagination"] =
                    [
                        "revision must be non-negative, page >= 1 and pageSize between 1 and 200."
                    ]
                }));
        }

        var result = await listCompendiumChanges.ExecuteAsync(
            new CompendiumChangesRequest(
                sourceVersionId,
                entityType,
                changedSince,
                revision,
                page ?? 1,
                pageSize ?? 50),
            cancellationToken);

        return Ok(result);
    }
}
