using Compendium.API.Errors;
using Compendium.Application.Translations;

namespace Compendium.API.Translations;

public static class TranslationEndpoints
{
    public static IEndpointRouteBuilder MapTranslationEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/compendium/entities/{entityType}/{entityId:guid}/translations");
        group.MapPut("/{locale}/{field}", UpsertTranslation).WithName("UpsertTranslation");
        group.MapGet("/", GetTranslations).WithName("GetTranslationsForEntity");
        group.MapGet("/localized", GetLocalizedTranslations).WithName("GetLocalizedEntityTranslations");
        return endpoints;
    }

    private static async Task<IResult> UpsertTranslation(
        string entityType,
        Guid entityId,
        string locale,
        string field,
        UpsertTranslationRequest request,
        HttpContext httpContext,
        UpsertTranslationUseCase useCase,
        CancellationToken cancellationToken)
    {
        var correlationId = httpContext.Request.Headers["X-Correlation-ID"].FirstOrDefault();
        var result = await useCase.ExecuteAsync(new(entityType, entityId, locale, field, request.Text, correlationId), cancellationToken);
        return result.IsSuccess ? Results.Ok(result.Value) : HttpErrorMapper.ToProblem(result.Error);
    }

    private static async Task<IResult> GetTranslations(
        string entityType,
        Guid entityId,
        GetTranslationsForEntityQuery query,
        CancellationToken cancellationToken)
    {
        var result = await query.ExecuteAsync(entityType, entityId, cancellationToken);
        return result.IsSuccess ? Results.Ok(result.Value) : HttpErrorMapper.ToProblem(result.Error);
    }

    private static async Task<IResult> GetLocalizedTranslations(
        string entityType,
        Guid entityId,
        string locale,
        string? fallbackLocale,
        GetLocalizedEntityTranslationsQuery query,
        CancellationToken cancellationToken)
    {
        var result = await query.ExecuteAsync(entityType, entityId, locale, fallbackLocale, cancellationToken);
        return result.IsSuccess ? Results.Ok(result.Value) : HttpErrorMapper.ToProblem(result.Error);
    }
}

public sealed record UpsertTranslationRequest(string Text);
