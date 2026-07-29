using Compendium.API.Controllers;
using Compendium.API.Security;
using Compendium.Application.Fundamentals;
using Microsoft.AspNetCore.Mvc;

namespace Compendium.API.Fundamentals;

[Route("/api/compendium/skills")]
public sealed class SkillsController(
    CreateSkillUseCase createSkill,
    UpdateSkillUseCase updateSkill,
    ListSkillsQuery listSkills) : CompendiumControllerBase
{
    [HttpPost(Name = "CreateSkill")]
    [AdministrativeWrite]
    public async Task<IActionResult> Create(
        CreateSkillRequest request,
        CancellationToken cancellationToken)
    {
        var result = await createSkill.ExecuteAsync(
            new CreateSkillCommand(
                request.RuleSourceId,
                request.SourceVersionId,
                request.Code,
                request.Name,
                request.DefaultAbilityId),
            cancellationToken);

        return CreatedOrProblem(
            result,
            skill => $"/api/compendium/skills/{skill.Code}");
    }

    [HttpPut("{code}", Name = "UpdateSkill")]
    [AdministrativeWrite]
    public async Task<IActionResult> Update(
        string code,
        UpdateSkillRequest request,
        CancellationToken cancellationToken)
    {
        var result = await updateSkill.ExecuteAsync(
            new UpdateSkillCommand(
                code,
                request.RuleSourceId,
                request.SourceVersionId,
                request.Name,
                request.DefaultAbilityId),
            cancellationToken);

        return OkOrProblem(result);
    }

    [HttpGet(Name = "ListSkills")]
    public async Task<IActionResult> List(CancellationToken cancellationToken)
    {
        var result = await listSkills.ExecuteAsync(cancellationToken);
        return OkOrProblem(result);
    }
}

public sealed record CreateSkillRequest(
    Guid RuleSourceId,
    Guid SourceVersionId,
    string Code,
    string Name,
    Guid? DefaultAbilityId);

public sealed record UpdateSkillRequest(
    Guid RuleSourceId,
    Guid SourceVersionId,
    string Name,
    Guid? DefaultAbilityId);
