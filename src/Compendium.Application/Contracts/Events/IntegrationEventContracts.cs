namespace Compendium.Application.Contracts.Events;

public static class CompendiumEventNames
{
    public const string SourceVersionImportedV1 = "compendium.source-version-imported.v1";
    public const string EntityUpdatedV1 = "compendium.entity-updated.v1";
    public const string TranslationUpdatedV1 = "compendium.translation-updated.v1";
}

public sealed record IntegrationEventField(
    string Name,
    string Type,
    string? TextValue = null,
    decimal? NumberValue = null,
    bool? BooleanValue = null,
    string? ReferenceValue = null,
    string? EnumValue = null);

public sealed record IntegrationEventEnvelope(
    Guid EventId,
    string Name,
    int Version,
    string AggregateType,
    string AggregateId,
    string CorrelationId,
    DateTimeOffset OccurredAtUtc,
    IReadOnlyCollection<IntegrationEventField> Fields);

public sealed record SourceVersionImportedV1(
    Guid SourceVersionId,
    Guid ImportId);

public sealed record EntityUpdatedV1(
    string EntityType,
    Guid EntityId,
    Guid? SourceVersionId,
    string ChangeType);

public sealed record TranslationUpdatedV1(
    string EntityType,
    Guid EntityId,
    string Locale,
    string Field);
