using Compendium.Domain.SharedKernel;

namespace Compendium.Domain.Translations;

public sealed class Translation
{
    private Translation() { }

    private Translation(
        CompendiumEntityId id,
        TranslatableEntityType entityType,
        CompendiumEntityId entityId,
        Locale locale,
        TranslationField field,
        TranslatedText text,
        DateTimeOffset now)
    {
        Id = id;
        EntityType = entityType;
        EntityId = entityId;
        Locale = locale;
        Field = field;
        Text = text;
        CreatedAtUtc = now;
        UpdatedAtUtc = now;
    }

    public CompendiumEntityId Id { get; private set; } = null!;
    public TranslatableEntityType EntityType { get; private set; } = null!;
    public CompendiumEntityId EntityId { get; private set; } = null!;
    public Locale Locale { get; private set; } = null!;
    public TranslationField Field { get; private set; } = null!;
    public TranslatedText Text { get; private set; } = null!;
    public DateTimeOffset CreatedAtUtc { get; private set; }
    public DateTimeOffset UpdatedAtUtc { get; private set; }

    public static Translation Create(
        TranslatableEntityType entityType,
        CompendiumEntityId entityId,
        Locale locale,
        TranslationField field,
        TranslatedText text,
        DateTimeOffset now) =>
        new(CompendiumEntityId.New(), entityType, entityId, locale, field, text, now);

    public void UpdateText(TranslatedText text, DateTimeOffset now)
    {
        Text = text;
        UpdatedAtUtc = now;
    }
}
