using Compendium.Domain.SharedKernel;
using Compendium.Domain.Translations;

namespace Compendium.Application.Translations;

public interface ITranslationRepository
{
    Task<Translation?> GetAsync(TranslatableEntityType entityType, CompendiumEntityId entityId, Locale locale, TranslationField field, CancellationToken cancellationToken);
    Task<IReadOnlyCollection<Translation>> ListAsync(TranslatableEntityType entityType, CompendiumEntityId entityId, CancellationToken cancellationToken);
    Task AddAsync(Translation translation, CancellationToken cancellationToken);
    Task SaveWithTranslationUpdatedEventAsync(Translation translation, string correlationId, CancellationToken cancellationToken);
}
