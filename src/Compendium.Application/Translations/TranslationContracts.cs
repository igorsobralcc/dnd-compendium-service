namespace Compendium.Application.Translations;

public sealed record UpsertTranslationCommand(
    string EntityType,
    Guid EntityId,
    string Locale,
    string Field,
    string Text,
    string? CorrelationId = null);

public sealed record TranslationDto(
    Guid Id,
    string EntityType,
    Guid EntityId,
    string Locale,
    string Field,
    string Text,
    DateTimeOffset UpdatedAtUtc);

public sealed record LocalizedFieldDto(string Field, string Text, string ResolvedLocale, bool IsFallback);

public sealed record LocalizedEntityTranslationsDto(
    string EntityType,
    Guid EntityId,
    string RequestedLocale,
    string? FallbackLocale,
    IReadOnlyCollection<LocalizedFieldDto> Fields);
