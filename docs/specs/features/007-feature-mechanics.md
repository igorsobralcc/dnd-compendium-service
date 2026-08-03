# SPEC-007: Features, effects, prerequisites, and choices

- Status: Done
- Commit: `bd6e6c866ccd92c76a6fc2b8d125f05d96d1e06c`
- Completed: 2026-07-24

## Intent

Model reusable game features and their mechanical consequences as structured
compendium content rather than untyped prose.

## Implemented requirements

- **FEAT-001**: The domain MUST model features, typed mechanical effects,
  prerequisites, choice sets, choice options, and typed mechanical values.
- **FEAT-002**: Feature use cases MUST create and query features, attach effects
  and prerequisites, and configure choices through repository ports.
- **FEAT-003**: Feature HTTP endpoints MUST expose the supported application
  operations.
- **FEAT-004**: EF Core mappings and a migration MUST preserve feature child
  collections, type discriminators, ordering, and uniqueness.

## Acceptance criteria

- Valid feature graphs can be assembled and invalid effect or choice data is
  rejected by domain rules.
- The migration represents features and all owned mechanical structures.

## Evidence

- `src/Compendium.Domain/Features/`
- `src/Compendium.Application/Features/`
- `src/Compendium.Infra/Persistence/Features/`
- `tests/Compendium.UnitTests/Features/FeatureTests.cs`
