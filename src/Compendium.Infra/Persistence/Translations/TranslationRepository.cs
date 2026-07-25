using Compendium.Application.Translations;
using Compendium.Domain.SharedKernel;
using Compendium.Domain.Translations;
using Compendium.Infra.Persistence.Integration;
using Microsoft.EntityFrameworkCore;

namespace Compendium.Infra.Persistence.Translations;

internal sealed class TranslationRepository : ITranslationRepository
{
    private readonly CompendiumDbContext dbContext;
    public TranslationRepository(CompendiumDbContext dbContext) => this.dbContext = dbContext;

    public Task<Translation?> GetAsync(TranslatableEntityType entityType, CompendiumEntityId entityId, Locale locale, TranslationField field, CancellationToken cancellationToken) =>
        dbContext.Translations.SingleOrDefaultAsync(
            x => x.EntityType == entityType && x.EntityId == entityId && x.Locale == locale && x.Field == field,
            cancellationToken);

    public async Task<IReadOnlyCollection<Translation>> ListAsync(TranslatableEntityType entityType, CompendiumEntityId entityId, CancellationToken cancellationToken) =>
        await dbContext.Translations
            .Where(x => x.EntityType == entityType && x.EntityId == entityId)
            .OrderBy(x => x.Locale).ThenBy(x => x.Field)
            .ToArrayAsync(cancellationToken);

    public async Task AddAsync(Translation translation, CancellationToken cancellationToken) =>
        await dbContext.Translations.AddAsync(translation, cancellationToken);

    public async Task SaveWithTranslationUpdatedEventAsync(Translation translation, string correlationId, CancellationToken cancellationToken)
    {
        var now = translation.UpdatedAtUtc;
        var message = new IntegrationOutbox(
            "compendium.translation-updated.v1",
            1,
            "translation",
            translation.Id.ToString(),
            correlationId,
            now);
        message.Fields.Add(new(message.Id, "entity_type", "text", now, textValue: translation.EntityType.Value));
        message.Fields.Add(new(message.Id, "entity_id", "reference", now, referenceValue: translation.EntityId.ToString()));
        message.Fields.Add(new(message.Id, "locale", "text", now, textValue: translation.Locale.Value));
        message.Fields.Add(new(message.Id, "field", "text", now, textValue: translation.Field.Value));
        await dbContext.IntegrationOutbox.AddAsync(message, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
