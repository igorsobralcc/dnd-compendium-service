using Compendium.API.Controllers;
using Compendium.API.Security;
using Compendium.Application.Sources;
using Compendium.Domain.Sources;
using Microsoft.AspNetCore.Mvc;

namespace Compendium.API.Sources;

[Route("/api/compendium/rulesets")]
public sealed class RulesetsController(
    CreateRulesetUseCase createRuleset,
    UpdateRulesetUseCase updateRuleset,
    GetRulesetByCodeQuery getRulesetByCode) : CompendiumControllerBase
{
    [HttpPost(Name = "CreateRuleset")]
    [AdministrativeWrite]
    public async Task<IActionResult> Create(
        CreateRulesetRequest request,
        CancellationToken cancellationToken)
    {
        var result = await createRuleset.ExecuteAsync(
            new CreateRulesetCommand(
                request.Code,
                request.Name,
                request.Version,
                request.Status),
            cancellationToken);

        return CreatedOrProblem(
            result,
            ruleset => $"/api/compendium/rulesets/{ruleset.Code}");
    }

    [HttpPut("{code}", Name = "UpdateRuleset")]
    [AdministrativeWrite]
    public async Task<IActionResult> Update(
        string code,
        UpdateRulesetRequest request,
        CancellationToken cancellationToken)
    {
        var result = await updateRuleset.ExecuteAsync(
            new UpdateRulesetCommand(
                code,
                request.Name,
                request.Version,
                request.Status),
            cancellationToken);

        return OkOrProblem(result);
    }

    [HttpGet("{code}", Name = "GetRulesetByCode")]
    public async Task<IActionResult> GetByCode(
        string code,
        CancellationToken cancellationToken)
    {
        var result = await getRulesetByCode.ExecuteAsync(code, cancellationToken);
        return OkOrProblem(result);
    }
}

public sealed record CreateRulesetRequest(
    string Code,
    string Name,
    string Version,
    RulesetStatus Status);

public sealed record UpdateRulesetRequest(
    string Name,
    string Version,
    RulesetStatus Status);
