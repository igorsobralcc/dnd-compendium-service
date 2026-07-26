using Compendium.Application.Translations;
using Compendium.Domain.SharedKernel;
using Compendium.Domain.Translations;
using Compendium.Infra.Persistence.Integration;
using Compendium.Application.Contracts.Events;
using Compendium.Application.Integration;
using Microsoft.EntityFrameworkCore;

namespace Compendium.Infra.Persistence.Translations;

internal sealed class TranslationRepository : ITranslationRepository
{
    private readonly CompendiumDbContext dbContext;
    private readonly IEventPublisher eventPublisher;
    public TranslationRepository(CompendiumDbContext dbContext, IEventPublisher eventPublisher)
    {
        this.dbContext = dbContext;
        this.eventPublisher = eventPublisher;
    }

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
        await eventPublisher.EnqueueAsync(
            CompendiumEventNames.TranslationUpdatedV1,
            1,
            "translation",
            translation.Id.ToString(),
            correlationId,
            now,
            [
                new("entity_type", "text", TextValue: translation.EntityType.Value),
                new("entity_id", "reference", ReferenceValue: translation.EntityId.ToString()),
                new("locale", "text", TextValue: translation.Locale.Value),
                new("field", "text", TextValue: translation.Field.Value)
            ],
            cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
