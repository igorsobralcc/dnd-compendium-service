using Compendium.API.Controllers;
using Compendium.API.Security;
using Compendium.Application.Fundamentals;
using Compendium.Domain.Fundamentals;
using Microsoft.AspNetCore.Mvc;

namespace Compendium.API.Fundamentals;

[Route("/api/compendium/ability-score-methods")]
public sealed class AbilityScoreMethodsController(
    CreateAbilityScoreMethodUseCase createAbilityScoreMethod,
    ListAbilityScoreMethodsQuery listAbilityScoreMethods)
    : CompendiumControllerBase
{
    [HttpPost(Name = "CreateAbilityScoreMethod")]
    [AdministrativeWrite]
    public async Task<IActionResult> Create(
        CreateAbilityScoreMethodRequest request,
        CancellationToken cancellationToken)
    {
        var result = await createAbilityScoreMethod.ExecuteAsync(
            new CreateAbilityScoreMethodCommand(
                request.RuleSourceId,
                request.SourceVersionId,
                request.Code,
                request.Name,
                request.Type,
                request.Rules,
                request.StandardValues,
                request.PointBuyCosts,
                request.RollRule),
            cancellationToken);

        return CreatedOrProblem(
            result,
            method =>
                $"/api/compendium/ability-score-methods/{method.Code}");
    }

    [HttpGet(Name = "ListAbilityScoreMethods")]
    public async Task<IActionResult> List(CancellationToken cancellationToken)
    {
        var result = await listAbilityScoreMethods.ExecuteAsync(
            cancellationToken);

        return OkOrProblem(result);
    }
}

public sealed record CreateAbilityScoreMethodRequest(
    Guid RuleSourceId,
    Guid SourceVersionId,
    string Code,
    string Name,
    AbilityScoreMethodType Type,
    IReadOnlyCollection<CreateAbilityScoreMethodRuleCommand> Rules,
    IReadOnlyCollection<int> StandardValues,
    IReadOnlyCollection<CreateAbilityScorePointBuyCostCommand> PointBuyCosts,
    CreateAbilityScoreRollRuleCommand? RollRule);
