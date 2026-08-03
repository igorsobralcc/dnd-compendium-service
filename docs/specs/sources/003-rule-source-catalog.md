# SPEC-003: Rule source catalog

- Status: Done
- Commit: `b21dc54b54147be41f110e9f38487ae8fb297ad2`
- Completed: 2026-06-11

## Intent

Support canonical rule sources, rulesets, and versioned source releases across
the domain, application, HTTP, and persistence layers.

## Implemented requirements

- **SRC-001**: The domain MUST model RuleSource, Ruleset, and SourceVersion
  invariants with typed source value objects and domain errors.
- **SRC-002**: Application use cases MUST create, update, publish, retrieve,
  and list source data through repository ports.
- **SRC-003**: HTTP endpoints MUST expose the source use cases and translate
  failures through the service error contract.
- **SRC-004**: EF Core mappings and a migration MUST persist source entities,
  relationships, versions, and uniqueness constraints in the service schema.

## Acceptance criteria

- Valid source aggregates can be created and queried; invalid transitions fail
  with domain or application errors.
- The source migration is discoverable and source HTTP contracts respond.

## Evidence

- `src/Compendium.Domain/Sources/`
- `src/Compendium.Application/Sources/`
- `src/Compendium.API/Sources/SourceEndpoints.cs`
- `tests/Compendium.UnitTests/Sources/`
