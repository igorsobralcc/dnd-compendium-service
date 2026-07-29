using Compendium.API.Controllers;
using Compendium.API.Security;
using Compendium.Application.Sources;
using Compendium.Domain.Sources;
using Microsoft.AspNetCore.Mvc;

namespace Compendium.API.Sources;

[Route("/api/compendium")]
public sealed class SourceVersionsController(
    CreateSourceVersionUseCase createSourceVersion,
    MarkSourceVersionAsCurrentUseCase markSourceVersionAsCurrent,
    GetCurrentSourceVersionQuery getCurrentSourceVersion,
    ListSourceVersionsQuery listSourceVersions) : CompendiumControllerBase
{
    [HttpPost("source-versions", Name = "CreateSourceVersion")]
    [AdministrativeWrite]
    public async Task<IActionResult> Create(
        CreateSourceVersionRequest request,
        CancellationToken cancellationToken)
    {
        var result = await createSourceVersion.ExecuteAsync(
            new CreateSourceVersionCommand(
                request.RuleSourceId,
                request.VersionNumber,
                request.PublicationDate,
                request.ImportStatus,
                request.IsCurrent),
            cancellationToken);

        return CreatedOrProblem(
            result,
            sourceVersion => $"/api/compendium/source-versions/{sourceVersion.Id}");
    }

    [HttpPost(
        "rule-sources/{ruleSourceId:guid}/source-versions/{versionId:guid}/current",
        Name = "MarkSourceVersionAsCurrent")]
    [AdministrativeWrite]
    public async Task<IActionResult> MarkAsCurrent(
        Guid ruleSourceId,
        Guid versionId,
        CancellationToken cancellationToken)
    {
        var result = await markSourceVersionAsCurrent.ExecuteAsync(
            ruleSourceId,
            versionId,
            cancellationToken);

        return OkOrProblem(result);
    }

    [HttpGet(
        "rule-sources/{ruleSourceId:guid}/source-versions/current",
        Name = "GetCurrentSourceVersion")]
    public async Task<IActionResult> GetCurrent(
        Guid ruleSourceId,
        CancellationToken cancellationToken)
    {
        var result = await getCurrentSourceVersion.ExecuteAsync(
            ruleSourceId,
            cancellationToken);

        return OkOrProblem(result);
    }

    [HttpGet(
        "rule-sources/{ruleSourceId:guid}/source-versions",
        Name = "ListSourceVersions")]
    public async Task<IActionResult> List(
        Guid ruleSourceId,
        CancellationToken cancellationToken)
    {
        var result = await listSourceVersions.ExecuteAsync(
            ruleSourceId,
            cancellationToken);

        return OkOrProblem(result);
    }
}

public sealed record CreateSourceVersionRequest(
    Guid RuleSourceId,
    string VersionNumber,
    DateOnly PublicationDate,
    ImportStatus ImportStatus,
    bool IsCurrent);
