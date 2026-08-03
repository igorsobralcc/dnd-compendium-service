# SPEC-033: Readable equipment application layer

- Status: Done
- Commit: `f8da5f5247d8f385681e78485d5f6d8efee1585a`
- Completed: 2026-08-01

## Intent

Reformat and restructure equipment application code so contracts, repository
ports, validation, and use-case flow are reviewable without changing behavior.

## Implemented requirements

- **READ-APP-001**: Equipment commands, DTOs, errors, repository contracts, and
  use cases MUST use consistent formatting and descriptive layout.
- **READ-APP-002**: Validation, lookup, mutation, persistence, and mapping steps
  MUST be expressed as distinct readable operations.
- **READ-APP-003**: Public types, method signatures, result semantics, repository
  calls, and equipment behavior MUST remain compatible.

## Acceptance criteria

- The equipment HTTP and application contracts are unchanged.
- Existing equipment tests pass without behavioral expectation updates.
- The diff introduces no new database migration or API route.

## Evidence

- `src/Compendium.Application/Equipment/EquipmentContracts.cs`
- `src/Compendium.Application/Equipment/EquipmentErrors.cs`
- `src/Compendium.Application/Equipment/EquipmentUseCases.cs`
- `src/Compendium.Application/Equipment/IEquipmentRepositories.cs`
