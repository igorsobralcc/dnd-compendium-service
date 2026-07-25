using Compendium.Application.Errors;
using Compendium.Domain.SharedKernel;
using Compendium.Domain.Translations;

namespace Compendium.Application.Translations;

public sealed class UpsertTranslationUseCase
{
    private readonly ITranslationRepository translations;
    private readonly IClock clock;

    public UpsertTranslationUseCase(ITranslationRepository translations, IClock clock)
    {
        this.translations = translations;
        this.clock = clock;
    }

    public async Task<ApplicationResult<TranslationDto>> ExecuteAsync(UpsertTranslationCommand command, CancellationToken cancellationToken)
    {
        var parsed = TranslationValidation.Parse(command.EntityType, command.EntityId, command.Locale, command.Field, command.Text);
        if (parsed.IsFailure) return ApplicationResult<TranslationDto>.Failure(parsed.Error);

        var input = parsed.Value;
        var translation = await translations.GetAsync(input.EntityType, input.EntityId, input.Locale, input.Field, cancellationToken);
        if (translation is null)
        {
            translation = Translation.Create(input.EntityType, input.EntityId, input.Locale, input.Field, input.Text, clock.UtcNow);
            await translations.AddAsync(translation, cancellationToken);
        }
        else
        {
            translation.UpdateText(input.Text, clock.UtcNow);
        }

        var correlationId = string.IsNullOrWhiteSpace(command.CorrelationId)
            ? Guid.CreateVersion7().ToString()
            : command.CorrelationId.Trim();
        await translations.SaveWithTranslationUpdatedEventAsync(translation, correlationId, cancellationToken);
        return ApplicationResult<TranslationDto>.Success(translation.ToDto());
    }
}

public sealed class GetTranslationsForEntityQuery
{
    private readonly ITranslationRepository translations;
    public GetTranslationsForEntityQuery(ITranslationRepository translations) => this.translations = translations;

    public async Task<ApplicationResult<IReadOnlyCollection<TranslationDto>>> ExecuteAsync(string entityTypeValue, Guid entityIdValue, CancellationToken cancellationToken)
    {
        var parsed = TranslationValidation.ParseEntity(entityTypeValue, entityIdValue);
        if (parsed.IsFailure) return ApplicationResult<IReadOnlyCollection<TranslationDto>>.Failure(parsed.Error);
        var result = await translations.ListAsync(parsed.Value.EntityType, parsed.Value.EntityId, cancellationToken);
        return ApplicationResult<IReadOnlyCollection<TranslationDto>>.Success(result.Select(x => x.ToDto()).ToArray());
    }
}

public sealed class GetLocalizedEntityTranslationsQuery
{
    private readonly ITranslationRepository translations;
    public GetLocalizedEntityTranslationsQuery(ITranslationRepository translations) => this.translations = translations;

    public async Task<ApplicationResult<LocalizedEntityTranslationsDto>> ExecuteAsync(string entityTypeValue, Guid entityIdValue, string localeValue, string? fallbackLocaleValue, CancellationToken cancellationToken)
    {
        var entity = TranslationValidation.ParseEntity(entityTypeValue, entityIdValue);
        if (entity.IsFailure) return ApplicationResult<LocalizedEntityTranslationsDto>.Failure(entity.Error);
        var locale = Locale.Create(localeValue);
        if (locale.IsFailure) return ApplicationResult<LocalizedEntityTranslationsDto>.Failure(TranslationErrors.FromDomain(locale.Error));

        Locale? fallback = null;
        if (!string.IsNullOrWhiteSpace(fallbackLocaleValue))
        {
            var parsedFallback = Locale.Create(fallbackLocaleValue);
            if (parsedFallback.IsFailure) return ApplicationResult<LocalizedEntityTranslationsDto>.Failure(TranslationErrors.FromDomain(parsedFallback.Error));
            fallback = parsedFallback.Value;
        }

        var all = await translations.ListAsync(entity.Value.EntityType, entity.Value.EntityId, cancellationToken);
        var requested = all.Where(x => x.Locale == locale.Value).ToDictionary(x => x.Field.Value);
        var fallbackValues = fallback is null
            ? new Dictionary<string, Translation>()
            : all.Where(x => x.Locale == fallback).ToDictionary(x => x.Field.Value);
        var fields = requested.Keys.Concat(fallbackValues.Keys).Distinct().Order()
            .Select(field => requested.TryGetValue(field, out var exact)
                ? new LocalizedFieldDto(field, exact.Text.Value, exact.Locale.Value, false)
                : new LocalizedFieldDto(field, fallbackValues[field].Text.Value, fallbackValues[field].Locale.Value, true))
            .ToArray();

        return ApplicationResult<LocalizedEntityTranslationsDto>.Success(
            new(entity.Value.EntityType.Value, entity.Value.EntityId.Value, locale.Value.Value, fallback?.Value, fields));
    }
}

file sealed record TranslationInput(TranslatableEntityType EntityType, CompendiumEntityId EntityId, Locale Locale, TranslationField Field, TranslatedText Text);
file sealed record TranslationEntityInput(TranslatableEntityType EntityType, CompendiumEntityId EntityId);

file static class TranslationValidation
{
    public static ApplicationResult<TranslationInput> Parse(string entityType, Guid entityId, string locale, string field, string text)
    {
        var entity = ParseEntity(entityType, entityId);
        if (entity.IsFailure) return ApplicationResult<TranslationInput>.Failure(entity.Error);
        var parsedLocale = Locale.Create(locale);
        if (parsedLocale.IsFailure) return ApplicationResult<TranslationInput>.Failure(TranslationErrors.FromDomain(parsedLocale.Error));
        var parsedField = TranslationField.Create(field);
        if (parsedField.IsFailure) return ApplicationResult<TranslationInput>.Failure(TranslationErrors.FromDomain(parsedField.Error));
        var parsedText = TranslatedText.Create(text);
        if (parsedText.IsFailure) return ApplicationResult<TranslationInput>.Failure(TranslationErrors.FromDomain(parsedText.Error));
        return ApplicationResult<TranslationInput>.Success(new(entity.Value.EntityType, entity.Value.EntityId, parsedLocale.Value, parsedField.Value, parsedText.Value));
    }

    public static ApplicationResult<TranslationEntityInput> ParseEntity(string entityType, Guid entityId)
    {
        var parsedType = TranslatableEntityType.Create(entityType);
        if (parsedType.IsFailure) return ApplicationResult<TranslationEntityInput>.Failure(TranslationErrors.FromDomain(parsedType.Error));
        var parsedId = CompendiumEntityId.Create(entityId);
        if (parsedId.IsFailure) return ApplicationResult<TranslationEntityInput>.Failure(TranslationErrors.FromDomain(parsedId.Error));
        return ApplicationResult<TranslationEntityInput>.Success(new(parsedType.Value, parsedId.Value));
    }
}

file static class TranslationErrors
{
    public static ApplicationError FromDomain(DomainError error) => new(error.Code, error.Message, error.Kind switch
    {
        DomainErrorKind.Conflict => ApplicationErrorKind.Conflict,
        DomainErrorKind.NotFound => ApplicationErrorKind.NotFound,
        _ => ApplicationErrorKind.Validation
    });
}

file static class TranslationMapping
{
    public static TranslationDto ToDto(this Translation translation) =>
        new(translation.Id.Value, translation.EntityType.Value, translation.EntityId.Value, translation.Locale.Value, translation.Field.Value, translation.Text.Value, translation.UpdatedAtUtc);
}
