using Compendium.API.Controllers;
using Compendium.API.Security;
using Compendium.Application.Classes;
using Microsoft.AspNetCore.Mvc;

namespace Compendium.API.Classes;

[Route("/api/compendium/classes/{classCode}/subclasses")]
public sealed class SubclassesController(
    CreateSubclassUseCase createSubclass,
    LinkSubclassFeatureUseCase linkSubclassFeature,
    ListSubclassesByClassQuery listSubclassesByClass,
    GetSubclassDetailsQuery getSubclassDetails) : CompendiumControllerBase
{
    [HttpPost(Name = "CreateSubclass")]
    [AdministrativeWrite]
    public async Task<IActionResult> Create(
        string classCode,
        CreateSubclassRequest request,
        CancellationToken cancellationToken)
    {
        var result = await createSubclass.ExecuteAsync(
            new CreateSubclassCommand(
                classCode,
                request.RuleSourceId,
                request.SourceVersionId,
                request.Code,
                request.Name,
                request.Description),
            cancellationToken);

        return CreatedOrProblem(
            result,
            subclass =>
                $"/api/compendium/classes/{classCode}/subclasses/{subclass.Code}");
    }

    [HttpGet(Name = "ListSubclassesByClass")]
    public async Task<IActionResult> ListByClass(
        string classCode,
        CancellationToken cancellationToken)
    {
        var result = await listSubclassesByClass.ExecuteAsync(
            classCode,
            cancellationToken);

        return OkOrProblem(result);
    }

    [HttpGet("{subclassCode}", Name = "GetSubclassDetails")]
    public async Task<IActionResult> GetDetails(
        string classCode,
        string subclassCode,
        CancellationToken cancellationToken)
    {
        var result = await getSubclassDetails.ExecuteAsync(
            classCode,
            subclassCode,
            cancellationToken);

        return OkOrProblem(result);
    }

    [HttpPost("{subclassCode}/features", Name = "LinkSubclassFeature")]
    [AdministrativeWrite]
    public async Task<IActionResult> LinkFeature(
        string classCode,
        string subclassCode,
        LinkSubclassFeatureRequest request,
        CancellationToken cancellationToken)
    {
        var result = await linkSubclassFeature.ExecuteAsync(
            new LinkSubclassFeatureCommand(
                classCode,
                subclassCode,
                request.RuleSourceId,
                request.SourceVersionId,
                request.FeatureId,
                request.Level),
            cancellationToken);

        return OkOrProblem(result);
    }
}

public sealed record CreateSubclassRequest(
    Guid RuleSourceId,
    Guid SourceVersionId,
    string Code,
    string Name,
    string? Description);

public sealed record LinkSubclassFeatureRequest(
    Guid RuleSourceId,
    Guid SourceVersionId,
    Guid FeatureId,
    int Level);
