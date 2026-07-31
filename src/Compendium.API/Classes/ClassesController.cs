using Compendium.API.Controllers;
using Compendium.API.Security;
using Compendium.Application.Classes;
using Microsoft.AspNetCore.Mvc;

namespace Compendium.API.Classes;

[Route("/api/compendium/classes")]
public sealed class ClassesController(
    CreateClassUseCase createClass,
    UpdateClassUseCase updateClass,
    ListClassesQuery listClasses,
    GetClassDetailsQuery getClassDetails,
    ConfigureClassProgressionUseCase configureClassProgression,
    GetClassProgressionQuery getClassProgression) : CompendiumControllerBase
{
    [HttpPost(Name = "CreateClass")]
    [AdministrativeWrite]
    public async Task<IActionResult> Create(
        CreateClassRequest request,
        CancellationToken cancellationToken)
    {
        var result = await createClass.ExecuteAsync(
            new CreateClassCommand(
                request.RuleSourceId,
                request.SourceVersionId,
                request.Code,
                request.Name,
                request.Description,
                request.CoreTraits,
                request.PrimaryAbilityIds,
                request.Levels),
            cancellationToken);

        return CreatedOrProblem(
            result,
            characterClass => $"/api/compendium/classes/{characterClass.Code}");
    }

    [HttpPut("{code}", Name = "UpdateClass")]
    [AdministrativeWrite]
    public async Task<IActionResult> Update(
        string code,
        UpdateClassRequest request,
        CancellationToken cancellationToken)
    {
        var result = await updateClass.ExecuteAsync(
            new UpdateClassCommand(
                code,
                request.RuleSourceId,
                request.SourceVersionId,
                request.Name,
                request.Description,
                request.CoreTraits,
                request.PrimaryAbilityIds),
            cancellationToken);

        return OkOrProblem(result);
    }

    [HttpGet(Name = "ListClasses")]
    public async Task<IActionResult> List(CancellationToken cancellationToken)
    {
        var result = await listClasses.ExecuteAsync(cancellationToken);
        return OkOrProblem(result);
    }

    [HttpGet("{code}", Name = "GetClassDetails")]
    public async Task<IActionResult> GetDetails(
        string code,
        CancellationToken cancellationToken)
    {
        var result = await getClassDetails.ExecuteAsync(code, cancellationToken);
        return OkOrProblem(result);
    }

    [HttpPut("{code}/progression", Name = "ConfigureClassProgression")]
    [AdministrativeWrite]
    public async Task<IActionResult> ConfigureProgression(
        string code,
        ConfigureClassProgressionRequest request,
        CancellationToken cancellationToken)
    {
        var result = await configureClassProgression.ExecuteAsync(
            new ConfigureClassProgressionCommand(
                code,
                request.RuleSourceId,
                request.SourceVersionId,
                request.Levels,
                request.SpellcastingProgression),
            cancellationToken);

        return OkOrProblem(result);
    }

    [HttpGet("{code}/progression", Name = "GetClassProgression")]
    public async Task<IActionResult> GetProgression(
        string code,
        CancellationToken cancellationToken)
    {
        var result = await getClassProgression.ExecuteAsync(
            code,
            cancellationToken);

        return OkOrProblem(result);
    }
}

public sealed record CreateClassRequest(
    Guid RuleSourceId,
    Guid SourceVersionId,
    string Code,
    string Name,
    string? Description,
    ClassCoreTraitsCommand CoreTraits,
    IReadOnlyCollection<Guid> PrimaryAbilityIds,
    IReadOnlyCollection<ClassLevelCommand> Levels);

public sealed record UpdateClassRequest(
    Guid RuleSourceId,
    Guid SourceVersionId,
    string Name,
    string? Description,
    ClassCoreTraitsCommand CoreTraits,
    IReadOnlyCollection<Guid> PrimaryAbilityIds);

public sealed record ConfigureClassProgressionRequest(
    Guid RuleSourceId,
    Guid SourceVersionId,
    IReadOnlyCollection<ClassLevelCommand> Levels,
    ClassSpellcastingProgressionCommand? SpellcastingProgression);
