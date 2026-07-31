using Compendium.API.Controllers;
using Compendium.API.Security;
using Compendium.Application.Importing;
using Microsoft.AspNetCore.Mvc;

namespace Compendium.API.Importing;

[Route("/api/compendium/source-versions/{sourceVersionId:guid}")]
public sealed class SourceVersionImportsController(
    ImportSourceVersionUseCase importSourceVersion,
    ValidateSourceVersionUseCase validateSourceVersion,
    ListSourceVersionValidationIssuesQuery listValidationIssues)
    : CompendiumControllerBase
{
    [HttpPost("imports", Name = "ImportSourceVersion")]
    [AdministrativeWrite]
    public async Task<IActionResult> Import(
        Guid sourceVersionId,
        ImportSourceVersionRequest request,
        CancellationToken cancellationToken)
    {
        var correlationId = Request.Headers["X-Correlation-ID"].FirstOrDefault()
            ?? HttpContext.TraceIdentifier;
        var result = await importSourceVersion.ExecuteAsync(
            new ImportSourceVersionCommand(
                sourceVersionId,
                correlationId,
                request.Abilities ?? [],
                request.Skills ?? [],
                request.Languages ?? [],
                request.Proficiencies ?? [],
                request.HitDice ?? [],
                request.Equipment ?? []),
            cancellationToken);

        return ToActionResult(
            result,
            value => value.AlreadyImported
                ? Ok(value)
                : Created(
                    $"/api/compendium/source-versions/{sourceVersionId}/imports/{value.ImportId}",
                    value));
    }

    [HttpPost("validation", Name = "ValidateSourceVersion")]
    [AdministrativeWrite]
    public async Task<IActionResult> Validate(
        Guid sourceVersionId,
        CancellationToken cancellationToken)
    {
        var result = await validateSourceVersion.ExecuteAsync(
            sourceVersionId,
            cancellationToken);

        return OkOrProblem(result);
    }

    [HttpGet("validation/issues", Name = "ListSourceVersionValidationIssues")]
    public async Task<IActionResult> ListIssues(
        Guid sourceVersionId,
        CancellationToken cancellationToken)
    {
        var result = await listValidationIssues.ExecuteAsync(
            sourceVersionId,
            cancellationToken);

        return OkOrProblem(result);
    }
}

public sealed record ImportSourceVersionRequest(
    IReadOnlyCollection<SeedNamedEntry>? Abilities,
    IReadOnlyCollection<SeedSkillEntry>? Skills,
    IReadOnlyCollection<SeedNamedEntry>? Languages,
    IReadOnlyCollection<SeedProficiencyEntry>? Proficiencies,
    IReadOnlyCollection<int>? HitDice,
    IReadOnlyCollection<SeedEquipmentEntry>? Equipment);
