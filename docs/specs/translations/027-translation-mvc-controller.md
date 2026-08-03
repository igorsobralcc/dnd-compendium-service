# SPEC-027: Translation MVC controller

- Status: Done
- Commit: `6f0586802ac1a99551bf736c72d588f88b3c4887`
- Completed: 2026-07-31

## Intent

Move translation upsert and localized read operations from minimal endpoints to
one MVC controller.

## Implemented requirements

- **MVC-TRN-001**: Translation operations MUST be exposed through
  `TranslationsController`.
- **MVC-TRN-002**: Routes, verbs, request fields, fallback-locale behavior,
  result mapping, names, and authorization MUST remain compatible.
- **MVC-TRN-003**: The legacy translation endpoint mapper MUST be deleted and
  removed from startup.

## Acceptance criteria

- The runtime translation surface remains identical to the locked contract.
- Translation upsert, listing, and localization behavior remain unchanged.

## Evidence

- `src/Compendium.API/Translations/TranslationsController.cs`
- Deletion of `src/Compendium.API/Translations/TranslationEndpoints.cs`
- `src/Compendium.API/Program.cs`
