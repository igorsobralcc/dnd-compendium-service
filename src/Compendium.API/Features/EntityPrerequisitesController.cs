using Compendium.API.Controllers;
using Compendium.API.Security;
using Compendium.Application.Features;
using Compendium.Domain.Features;
using Microsoft.AspNetCore.Mvc;

namespace Compendium.API.Features;

[Route("/api/compendium/entity-prerequisites")]
public sealed class EntityPrerequisitesController(
    AddPrerequisiteToEntityUseCase addPrerequisiteToEntity,
    RemovePrerequisiteFromEntityUseCase removePrerequisiteFromEntity,
    GetEntityPrerequisitesQuery getEntityPrerequisites) : CompendiumControllerBase
{
    [HttpPost(Name = "AddPrerequisiteToEntity")]
    [AdministrativeWrite]
    public async Task<IActionResult> Add(
        AddPrerequisiteToEntityRequest request,
        CancellationToken cancellationToken)
    {
        var result = await addPrerequisiteToEntity.ExecuteAsync(
            new AddPrerequisiteToEntityCommand(
                request.EntityKind,
                request.EntityId,
                request.Type,
                request.Operator,
                request.Target,
                request.ValueType,
                request.TextValue,
                request.NumericValue,
                request.BooleanValue,
                request.ReferenceId,
                request.EnumValue),
            cancellationToken);

        return CreatedOrProblem(
            result,
            prerequisite =>
                $"/api/compendium/entity-prerequisites/{prerequisite.Id}");
    }

    [HttpDelete("{prerequisiteId:guid}", Name = "RemovePrerequisiteFromEntity")]
    [AdministrativeWrite]
    public async Task<IActionResult> Remove(
        Guid prerequisiteId,
        CancellationToken cancellationToken)
    {
        var result = await removePrerequisiteFromEntity.ExecuteAsync(
            new RemovePrerequisiteFromEntityCommand(prerequisiteId),
            cancellationToken);

        return NoContentOrProblem(result);
    }

    [HttpGet("{entityKind}/{entityId:guid}", Name = "GetEntityPrerequisites")]
    public async Task<IActionResult> GetForEntity(
        CompendiumEntityKind entityKind,
        Guid entityId,
        CancellationToken cancellationToken)
    {
        var result = await getEntityPrerequisites.ExecuteAsync(
            entityKind,
            entityId,
            cancellationToken);

        return OkOrProblem(result);
    }
}

public sealed record AddPrerequisiteToEntityRequest(
    CompendiumEntityKind EntityKind,
    Guid EntityId,
    PrerequisiteType Type,
    ComparisonOperator Operator,
    EffectTarget Target,
    EffectValueType ValueType,
    string? TextValue,
    decimal? NumericValue,
    bool? BooleanValue,
    Guid? ReferenceId,
    string? EnumValue);
