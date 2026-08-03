# SPEC-010: Equipment catalog

- Status: Done
- Commit: `63f5f4d3b89730b7a7539e054e433da45f23f005`
- Completed: 2026-07-24

## Intent

Provide structured equipment data for items, armor, weapons, tools, packs, and
starting-equipment rules.

## Implemented requirements

- **EQP-001**: The domain MUST model equipment items, armor, weapons, weapon
  properties and masteries, tools, equipment packs, and starting-equipment
  choices with validated value objects.
- **EQP-002**: Application commands and queries MUST operate through equipment
  repository interfaces and explicit request/response contracts.
- **EQP-003**: Equipment endpoints MUST expose the supported catalog operations.
- **EQP-004**: Persistence mappings and a migration MUST retain subtype data,
  relationships, ordering, and canonical uniqueness.

## Acceptance criteria

- Each equipment kind can be created and retrieved with its specialized data.
- Invalid equipment combinations fail domain validation.
- The migration and unit tests cover the new equipment model.

## Evidence

- `src/Compendium.Domain/Equipment/`
- `src/Compendium.Application/Equipment/`
- `src/Compendium.Infra/Persistence/Equipment/`
- `tests/Compendium.UnitTests/Equipment/EquipmentTests.cs`
