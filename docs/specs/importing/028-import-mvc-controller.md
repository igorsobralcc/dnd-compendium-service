# SPEC-028: Source-version import MVC controller

- Status: Done
- Commit: `42616684daad49e609407f1f52e90b712a3898bc`
- Completed: 2026-07-31

## Intent

Move source-version import and validation operations to MVC without changing
the administrative API contract.

## Implemented requirements

- **MVC-IMP-001**: Import, validate, and validation-issue operations MUST be
  exposed by `SourceVersionImportsController`.
- **MVC-IMP-002**: Actions MUST retain their established routes, methods,
  request/response bodies, status mapping, names, and administrative policy.
- **MVC-IMP-003**: The legacy import endpoint mapper MUST be deleted and removed
  from startup.

## Acceptance criteria

- The import surface matches the locked HTTP matrix.
- Existing application import idempotency and validation behavior is unchanged.
- No duplicate import routes are discovered.

## Evidence

- `src/Compendium.API/Importing/SourceVersionImportsController.cs`
- Deletion of `src/Compendium.API/Importing/ImportEndpoints.cs`
- `src/Compendium.API/Program.cs`
