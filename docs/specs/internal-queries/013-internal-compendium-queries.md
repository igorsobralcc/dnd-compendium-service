# SPEC-013: Internal compendium queries

- Status: Done
- Commit: `18410f38631fef0a121e809556ac5d7e597353a9`
- Completed: 2026-07-25

## Intent

Provide stable internal read models for downstream character creation, rules
evaluation, and incremental synchronization.

## Implemented requirements

- **IQ-001**: Internal APIs MUST provide character-creation options scoped by
  ruleset, source version, locale, and optional level.
- **IQ-002**: Internal APIs MUST provide mechanical entity details with typed
  feature, class, equipment, choices, prerequisites, and references.
- **IQ-003**: Internal APIs MUST expose paged compendium changes filtered by
  source version, entity type, timestamp, or revision.
- **IQ-004**: A query gateway MUST isolate SQL/EF projections from application
  contracts.
- **IQ-005**: Change records MUST be persisted with a monotonic revision through
  a migration.

## Acceptance criteria

- Internal endpoints return versioned V1 contracts rather than domain entities.
- Change queries paginate consistently and expose the next revision.
- Contract and migration tests exercise the internal API surface.

## Evidence

- `src/Compendium.Application/InternalQueries/`
- `src/Compendium.Infra/Persistence/InternalQueries/`
- `src/Compendium.API/InternalQueries/InternalCompendiumEndpoints.cs`
- `tests/Compendium.ContractTests/InternalCompendiumEndpointTests.cs`
