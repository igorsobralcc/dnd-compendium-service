# SPEC-009: Withdraw the origins feature slice

- Status: Done
- Commit: `0a4beb94caa2c9198592063b2fe703a60b103a8c`
- Completed: 2026-07-24

## Intent

Remove the origins slice introduced by the preceding commit and return the
service model, API, dependency graph, and migration history to the pre-origins
state.

## Implemented requirements

- **ROLL-001**: Species, Background, Feat, and related child types MUST be
  removed from domain, application, API, and infrastructure projects.
- **ROLL-002**: Origin endpoints and dependency registrations MUST NOT remain
  reachable.
- **ROLL-003**: The origin migration and model-snapshot entries MUST be removed
  so a new database does not create origin tables.
- **ROLL-004**: Origin-specific tests and contract expectations MUST be removed.
- **ROLL-005**: The repository-local EF tool manifest MUST be withdrawn and
  ignored.

## Acceptance criteria

- The solution compiles without Origins namespaces or registrations.
- EF migration discovery and schema assertions no longer include origin tables.

## Evidence

- Deletion of `src/Compendium.Domain/Origins/`
- Deletion of `src/Compendium.Infra/Persistence/Migrations/20260724180705_AddSpeciesBackgroundsAndFeats.cs`
- `src/Compendium.API/Program.cs`
- `.gitignore`
