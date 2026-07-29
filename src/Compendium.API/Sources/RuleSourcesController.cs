using Compendium.API.Controllers;
using Compendium.API.Security;
using Compendium.Application.Sources;
using Compendium.Domain.Sources;
using Microsoft.AspNetCore.Mvc;

namespace Compendium.API.Sources;

[Route("/api/compendium")]
public sealed class RuleSourcesController(
    CreateRuleSourceUseCase createRuleSource,
    ActivateRuleSourceUseCase activateRuleSource,
    DeactivateRuleSourceUseCase deactivateRuleSource,
    ListRuleSourcesByRulesetQuery listRuleSourcesByRuleset) : CompendiumControllerBase
{
    [HttpPost("rule-sources", Name = "CreateRuleSource")]
    [AdministrativeWrite]
    public async Task<IActionResult> Create(
        CreateRuleSourceRequest request,
        CancellationToken cancellationToken)
    {
        var result = await createRuleSource.ExecuteAsync(
            new CreateRuleSourceCommand(
                request.RulesetId,
                request.Code,
                request.Name,
                request.Type,
                request.Status),
            cancellationToken);

        return CreatedOrProblem(
            result,
            ruleSource => $"/api/compendium/rule-sources/{ruleSource.Id}");
    }

    [HttpPost("rule-sources/{id:guid}/activate", Name = "ActivateRuleSource")]
    [AdministrativeWrite]
    public async Task<IActionResult> Activate(
        Guid id,
        CancellationToken cancellationToken)
    {
        var result = await activateRuleSource.ExecuteAsync(id, cancellationToken);
        return OkOrProblem(result);
    }

    [HttpPost("rule-sources/{id:guid}/deactivate", Name = "DeactivateRuleSource")]
    [AdministrativeWrite]
    public async Task<IActionResult> Deactivate(
        Guid id,
        CancellationToken cancellationToken)
    {
        var result = await deactivateRuleSource.ExecuteAsync(id, cancellationToken);
        return OkOrProblem(result);
    }

    [HttpGet(
        "rulesets/{rulesetId:guid}/rule-sources",
        Name = "ListRuleSourcesByRuleset")]
    public async Task<IActionResult> ListByRuleset(
        Guid rulesetId,
        bool includeInactive,
        CancellationToken cancellationToken)
    {
        var result = await listRuleSourcesByRuleset.ExecuteAsync(
            rulesetId,
            includeInactive,
            cancellationToken);

        return OkOrProblem(result);
    }
}

public sealed record CreateRuleSourceRequest(
    Guid RulesetId,
    string Code,
    string Name,
    SourceType Type,
    SourceStatus Status);
