using Compendium.API.Controllers;
using Compendium.API.Security;
using Compendium.Application.Features;
using Compendium.Domain.Features;
using Microsoft.AspNetCore.Mvc;

namespace Compendium.API.Features;

[Route("/api/compendium/features")]
public sealed class FeaturesController(
    CreateFeatureUseCase createFeature,
    UpdateFeatureUseCase updateFeature,
    ListFeaturesQuery listFeatures,
    GetFeatureDetailsQuery getFeatureDetails,
    AttachEffectToFeatureUseCase attachEffectToFeature,
    GetFeatureEffectsQuery getFeatureEffects) : CompendiumControllerBase
{
    [HttpPost(Name = "CreateFeature")]
    [AdministrativeWrite]
    public async Task<IActionResult> Create(
        CreateFeatureRequest request,
        CancellationToken cancellationToken)
    {
        var result = await createFeature.ExecuteAsync(
            new CreateFeatureCommand(
                request.RuleSourceId,
                request.SourceVersionId,
                request.Code,
                request.Name,
                request.Description,
                request.LevelRequirement),
            cancellationToken);

        return CreatedOrProblem(
            result,
            feature => $"/api/compendium/features/{feature.Code}");
    }

    [HttpPut("{code}", Name = "UpdateFeature")]
    [AdministrativeWrite]
    public async Task<IActionResult> Update(
        string code,
        UpdateFeatureRequest request,
        CancellationToken cancellationToken)
    {
        var result = await updateFeature.ExecuteAsync(
            new UpdateFeatureCommand(
                code,
                request.RuleSourceId,
                request.SourceVersionId,
                request.Name,
                request.Description,
                request.LevelRequirement),
            cancellationToken);

        return OkOrProblem(result);
    }

    [HttpGet(Name = "ListFeatures")]
    public async Task<IActionResult> List(CancellationToken cancellationToken)
    {
        var result = await listFeatures.ExecuteAsync(cancellationToken);
        return OkOrProblem(result);
    }

    [HttpGet("{code}", Name = "GetFeatureDetails")]
    public async Task<IActionResult> GetDetails(
        string code,
        CancellationToken cancellationToken)
    {
        var result = await getFeatureDetails.ExecuteAsync(code, cancellationToken);
        return OkOrProblem(result);
    }

    [HttpPost("{featureCode}/effects", Name = "AttachEffectToFeature")]
    [AdministrativeWrite]
    public async Task<IActionResult> AttachEffect(
        string featureCode,
        AttachEffectToFeatureRequest request,
        CancellationToken cancellationToken)
    {
        var result = await attachEffectToFeature.ExecuteAsync(
            new AttachEffectToFeatureCommand(
                featureCode,
                request.EffectSchemaCode,
                request.Type,
                request.Target,
                request.Fields,
                request.Conditions),
            cancellationToken);

        return OkOrProblem(result);
    }

    [HttpGet("{featureCode}/effects", Name = "GetFeatureEffects")]
    public async Task<IActionResult> GetEffects(
        string featureCode,
        CancellationToken cancellationToken)
    {
        var result = await getFeatureEffects.ExecuteAsync(
            featureCode,
            cancellationToken);

        return OkOrProblem(result);
    }
}

public sealed record CreateFeatureRequest(
    Guid RuleSourceId,
    Guid SourceVersionId,
    string Code,
    string Name,
    string? Description,
    int? LevelRequirement);

public sealed record UpdateFeatureRequest(
    Guid RuleSourceId,
    Guid SourceVersionId,
    string Name,
    string? Description,
    int? LevelRequirement);

public sealed record AttachEffectToFeatureRequest(
    string EffectSchemaCode,
    EffectType Type,
    EffectTarget Target,
    IReadOnlyCollection<TypedValueFieldCommand> Fields,
    IReadOnlyCollection<FeatureEffectConditionCommand> Conditions);
