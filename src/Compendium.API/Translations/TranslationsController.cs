using Compendium.API.Controllers;
using Compendium.API.Security;
using Compendium.Application.Translations;
using Microsoft.AspNetCore.Mvc;

namespace Compendium.API.Translations;

[Route("/api/compendium/entities/{entityType}/{entityId:guid}/translations")]
public sealed class TranslationsController(
    UpsertTranslationUseCase upsertTranslation,
    GetTranslationsForEntityQuery getTranslations,
    GetLocalizedEntityTranslationsQuery getLocalizedTranslations)
    : CompendiumControllerBase
{
    [HttpPut("{locale}/{field}", Name = "UpsertTranslation")]
    [AdministrativeWrite]
    public async Task<IActionResult> Upsert(
        string entityType,
        Guid entityId,
        string locale,
        string field,
        UpsertTranslationRequest request,
        CancellationToken cancellationToken)
    {
        var correlationId = Request.Headers["X-Correlation-ID"].FirstOrDefault();
        var result = await upsertTranslation.ExecuteAsync(
            new UpsertTranslationCommand(
                entityType,
                entityId,
                locale,
                field,
                request.Text,
                correlationId),
            cancellationToken);

        return OkOrProblem(result);
    }

    [HttpGet(Name = "GetTranslationsForEntity")]
    public async Task<IActionResult> GetAll(
        string entityType,
        Guid entityId,
        CancellationToken cancellationToken)
    {
        var result = await getTranslations.ExecuteAsync(
            entityType,
            entityId,
            cancellationToken);

        return OkOrProblem(result);
    }

    [HttpGet("localized", Name = "GetLocalizedEntityTranslations")]
    public async Task<IActionResult> GetLocalized(
        string entityType,
        Guid entityId,
        string locale,
        string? fallbackLocale,
        CancellationToken cancellationToken)
    {
        var result = await getLocalizedTranslations.ExecuteAsync(
            entityType,
            entityId,
            locale,
            fallbackLocale,
            cancellationToken);

        return OkOrProblem(result);
    }
}

public sealed record UpsertTranslationRequest(string Text);
