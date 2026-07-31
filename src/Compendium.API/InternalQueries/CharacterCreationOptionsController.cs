using Compendium.API.Controllers;
using Compendium.API.Security;
using Compendium.Application.InternalQueries;
using Microsoft.AspNetCore.Mvc;

namespace Compendium.API.InternalQueries;

[Tags("Internal Compendium")]
[InternalRead]
[Route("/internal/compendium/character-creation-options")]
public sealed class CharacterCreationOptionsController(
    GetCharacterCreationOptionsQuery getCharacterCreationOptions)
    : CompendiumControllerBase
{
    [HttpGet(Name = "GetCharacterCreationOptionsV1")]
    [ProducesResponseType<CharacterCreationOptionsV1>(StatusCodes.Status200OK)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Get(
        [FromQuery(Name = "ruleset_id")] Guid rulesetId,
        [FromQuery(Name = "source_version_id")] Guid sourceVersionId,
        string? locale,
        int? level,
        CancellationToken cancellationToken)
    {
        if (rulesetId == Guid.Empty || sourceVersionId == Guid.Empty)
        {
            return ValidationProblem(new ValidationProblemDetails(
                new Dictionary<string, string[]>
                {
                    ["filters"] = ["rulesetId and sourceVersionId are required."]
                }));
        }

        if (level is < 1 or > 20)
        {
            return ValidationProblem(new ValidationProblemDetails(
                new Dictionary<string, string[]>
                {
                    ["level"] = ["Level must be between 1 and 20."]
                }));
        }

        var result = await getCharacterCreationOptions.ExecuteAsync(
            new CharacterCreationOptionsRequest(
                rulesetId,
                sourceVersionId,
                locale ?? "en-US",
                level),
            cancellationToken);

        return Ok(result);
    }
}
