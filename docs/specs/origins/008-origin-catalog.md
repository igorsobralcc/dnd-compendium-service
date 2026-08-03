# SPEC-008: Species, backgrounds, and feats

- Status: Done
- Commit: `701d819800695b827546ffa0802b8584440a6fc1`
- Completed: 2026-07-24

## Intent

Add an origins catalog connecting species, backgrounds, and feats to source
versions and reusable feature definitions.

## Implemented requirements

- **ORG-001**: The domain MUST model Species, Background, and Feat aggregates,
  including background ability, proficiency, feat-grant, and starting-equipment
  rules.
- **ORG-002**: Application use cases MUST create, configure, link, list, and
  retrieve origin entities through repository ports.
- **ORG-003**: Origin endpoints MUST expose the supported use cases.
- **ORG-004**: Persistence MUST enforce canonical and relational uniqueness and
  introduce the origin schema through a migration.

## Acceptance criteria

- Origin aggregates link only valid source and feature references.
- Origin data and its child rules round-trip through persistence.
- Domain, migration, and HTTP contract tests cover the new slice.

## Evidence

- `src/Compendium.Domain/Origins/`
- `src/Compendium.Application/Origins/`
- `src/Compendium.Infra/Persistence/Origins/`
- `tests/Compendium.UnitTests/Origins/OriginTests.cs`
