# SPEC-012: SRD source-version import

- Status: Done
- Commit: `5695658a76191c84b185d6f0f320b76feb6aed01`
- Completed: 2026-07-25

## Intent

Import a structured SRD seed into one source version and validate its internal
consistency before publication.

## Implemented requirements

- **IMP-001**: An import command MUST accept a source version, correlation ID,
  and seed collections for fundamentals and equipment.
- **IMP-002**: Repeating the same source-version import MUST be idempotent and
  report whether content was already imported.
- **IMP-003**: A consistency checker MUST evaluate source-version contents and
  return persisted validation issues with severity and publish eligibility.
- **IMP-004**: Import records and validation issues MUST be persisted by an EF
  Core migration and coordinated by an infrastructure gateway.
- **IMP-005**: HTTP endpoints MUST expose import and validation operations.

## Acceptance criteria

- A valid seed is imported once and reports the number of imported entities.
- Reimporting does not duplicate the same source version.
- Consistency issues are deterministic and can block publication.

## Evidence

- `src/Compendium.Application/Importing/`
- `src/Compendium.Domain/Importing/CompendiumConsistencyChecker.cs`
- `src/Compendium.Infra/Persistence/Importing/`
- `tests/Compendium.UnitTests/Importing/CompendiumConsistencyCheckerTests.cs`
