# SPEC-006: Class and subclass catalog

- Status: Done
- Commit: `49b8b5cb8dde97312b7c6a5523658a321fee66f9`
- Completed: 2026-06-11

## Intent

Add character classes, subclasses, and level progression as versioned,
source-backed compendium content.

## Implemented requirements

- **CLS-001**: The domain MUST model classes, subclasses, level progression,
  core traits, proficiencies, primary abilities, spellcasting, spell slots,
  weapon mastery counts, and subclass feature references.
- **CLS-002**: Use cases MUST support class and subclass lifecycle operations
  plus detailed progression queries through repository ports.
- **CLS-003**: HTTP endpoints MUST expose class catalog operations and return
  application contracts.
- **CLS-004**: Persistence MUST retain ordered level data and all child rules
  with source/version relationships through a migration.

## Acceptance criteria

- Class aggregates enforce progression and relationship invariants.
- A stored class can be retrieved with its level progression and related rules.
- Contract, migration, and domain tests cover the introduced slice.

## Evidence

- `src/Compendium.Domain/Classes/`
- `src/Compendium.Application/Classes/`
- `src/Compendium.Infra/Persistence/Classes/`
- `tests/Compendium.UnitTests/Classes/`
