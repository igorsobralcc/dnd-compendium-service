# SPEC-011: Translations and migration corrections

- Status: Done
- Commit: `dc497f9f051b29033ea2ab437ba04b092f1c5568`
- Completed: 2026-07-25

## Intent

Add localized entity fields while correcting class source updates and Outbox
field materialization needed by later integration events.

## Implemented requirements

- **TRN-001**: Translations MUST be uniquely addressable by entity type, entity
  ID, locale, and field, with validated locale, field, and text values.
- **TRN-002**: Application operations MUST upsert translations, list all values
  for an entity, and resolve a requested locale with an optional fallback.
- **TRN-003**: Translation HTTP endpoints and persistence mappings MUST expose
  and store the translation contract through a migration.
- **TRN-004**: Updating a CharacterClass MUST also update its RuleSource ID.
- **TRN-005**: Rehydrating an IntegrationOutboxField MUST accept stored text,
  reference, and enum values.

## Acceptance criteria

- Translation values can be inserted, updated, listed, and localized.
- Class source changes and typed Outbox field values survive persistence.
- Translation, class, contract, and migration tests cover the corrections.

## Evidence

- `src/Compendium.Domain/Translations/`
- `src/Compendium.Application/Translations/`
- `src/Compendium.Infra/Persistence/Migrations/20260725220037_AddTranslationsEpic.cs`
- `tests/Compendium.UnitTests/Translations/TranslationTests.cs`
