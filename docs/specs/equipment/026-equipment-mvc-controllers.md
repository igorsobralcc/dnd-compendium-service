# SPEC-026: Equipment MVC controllers

- Status: Done
- Commit: `d0ffe793e7dfa886565b822e5cd75e443cf417a1`
- Completed: 2026-07-31

## Intent

Replace equipment minimal endpoints with resource-oriented MVC controllers and
retain the established HTTP contract.

## Implemented requirements

- **MVC-EQP-001**: Equipment items, armor, weapons, tools, packs, starting
  equipment, weapon properties, and weapon masteries MUST be routed through
  dedicated controllers.
- **MVC-EQP-002**: Every migrated action MUST preserve method, route, payload,
  result status, operation name, and authorization metadata.
- **MVC-EQP-003**: The legacy equipment mapper MUST be removed from startup and
  no duplicate routes may remain.

## Acceptance criteria

- Equipment routes match the locked contract matrix.
- Equipment and front-controller contract tests pass for the migrated actions.

## Evidence

- `src/Compendium.API/Equipment/`
- `tests/Compendium.ContractTests/EquipmentEndpointTests.cs`
- `tests/Compendium.ContractTests/FrontControllerPipelineTests.cs`
