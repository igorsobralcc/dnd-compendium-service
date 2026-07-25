using Compendium.Application.Translations;
using Compendium.Domain.SharedKernel;
using Compendium.Domain.Translations;

namespace Compendium.UnitTests.Translations;

public sealed class TranslationTests
{
    [Fact]
    public void Value_objects_normalize_locale_and_reject_invalid_values()
    {
        var locale = Locale.Create("pt_br");

        Assert.True(locale.IsSuccess);
        Assert.Equal("pt-BR", locale.Value.Value);
        Assert.True(Locale.Create("portuguese").IsFailure);
        Assert.True(TranslationField.Create("Display Name").IsFailure);
        Assert.True(TranslatedText.Create(" ").IsFailure);
    }

    [Fact]
    public void Translation_preserves_canonical_reference_and_updates_text()
    {
        var now = DateTimeOffset.Parse("2026-07-25T12:00:00Z");
        var translation = Translation.Create(
            TranslatableEntityType.Create("feature").Value,
            CompendiumEntityId.New(),
            Locale.Create("pt-BR").Value,
            TranslationField.Create("name").Value,
            TranslatedText.Create("Ataque Extra").Value,
            now);

        translation.UpdateText(TranslatedText.Create("Ataque Adicional").Value, now.AddMinutes(1));

        Assert.Equal("Ataque Adicional", translation.Text.Value);
        Assert.Equal(now.AddMinutes(1), translation.UpdatedAtUtc);
        Assert.Equal(now, translation.CreatedAtUtc);
    }

    [Fact]
    public async Task Upsert_creates_then_updates_same_entity_field_and_publishes_each_change()
    {
        var repository = new FakeTranslationRepository();
        var clock = new FakeClock(DateTimeOffset.Parse("2026-07-25T12:00:00Z"));
        var useCase = new UpsertTranslationUseCase(repository, clock);
        var entityId = Guid.CreateVersion7();

        var created = await useCase.ExecuteAsync(
            new("feature", entityId, "pt_br", "name", "Ataque Extra", "test-correlation"),
            CancellationToken.None);
        clock.UtcNow = clock.UtcNow.AddMinutes(1);
        var updated = await useCase.ExecuteAsync(
            new("feature", entityId, "pt-BR", "name", "Ataque Adicional", "test-correlation"),
            CancellationToken.None);

        Assert.True(created.IsSuccess);
        Assert.True(updated.IsSuccess);
        Assert.Equal(created.Value.Id, updated.Value.Id);
        Assert.Equal("Ataque Adicional", updated.Value.Text);
        Assert.Single(repository.Items);
        Assert.Equal(2, repository.Events.Count);
    }

    [Fact]
    public async Task Localized_query_prefers_requested_locale_and_falls_back_per_field()
    {
        var repository = new FakeTranslationRepository();
        var clock = new FakeClock(DateTimeOffset.UtcNow);
        var upsert = new UpsertTranslationUseCase(repository, clock);
        var entityId = Guid.CreateVersion7();
        await upsert.ExecuteAsync(new("feature", entityId, "en-US", "name", "Extra Attack"), default);
        await upsert.ExecuteAsync(new("feature", entityId, "en-US", "description", "Attack twice."), default);
        await upsert.ExecuteAsync(new("feature", entityId, "pt-BR", "name", "Ataque Extra"), default);

        var result = await new GetLocalizedEntityTranslationsQuery(repository)
            .ExecuteAsync("feature", entityId, "pt-BR", "en-US", default);

        Assert.True(result.IsSuccess);
        Assert.Collection(
            result.Value.Fields,
            description =>
            {
                Assert.Equal("description", description.Field);
                Assert.Equal("Attack twice.", description.Text);
                Assert.True(description.IsFallback);
            },
            name =>
            {
                Assert.Equal("name", name.Field);
                Assert.Equal("Ataque Extra", name.Text);
                Assert.False(name.IsFallback);
            });
    }

    private sealed class FakeClock(DateTimeOffset utcNow) : IClock
    {
        public DateTimeOffset UtcNow { get; set; } = utcNow;
    }

    private sealed class FakeTranslationRepository : ITranslationRepository
    {
        public List<Translation> Items { get; } = [];
        public List<(Guid TranslationId, string CorrelationId)> Events { get; } = [];

        public Task<Translation?> GetAsync(TranslatableEntityType entityType, CompendiumEntityId entityId, Locale locale, TranslationField field, CancellationToken cancellationToken) =>
            Task.FromResult(Items.SingleOrDefault(x => x.EntityType == entityType && x.EntityId == entityId && x.Locale == locale && x.Field == field));

        public Task<IReadOnlyCollection<Translation>> ListAsync(TranslatableEntityType entityType, CompendiumEntityId entityId, CancellationToken cancellationToken) =>
            Task.FromResult<IReadOnlyCollection<Translation>>(Items.Where(x => x.EntityType == entityType && x.EntityId == entityId).ToArray());

        public Task AddAsync(Translation translation, CancellationToken cancellationToken)
        {
            Items.Add(translation);
            return Task.CompletedTask;
        }

        public Task SaveWithTranslationUpdatedEventAsync(Translation translation, string correlationId, CancellationToken cancellationToken)
        {
            Events.Add((translation.Id.Value, correlationId));
            return Task.CompletedTask;
        }
    }
}
