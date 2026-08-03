# SPEC-017: HTTP contract matrix

- Status: Done
- Commit: `0bca19945b51596d4623724fe248c18faa6e4432`
- Completed: 2026-07-29

## Intent

Capture the existing HTTP surface before the endpoint-to-MVC refactor so route,
method, authorization, and response behavior cannot drift unnoticed.

## Implemented requirements

- **CTR-001**: A centralized matrix MUST enumerate the service's HTTP operations
  and their expected metadata.
- **CTR-002**: Automated tests MUST compare runtime endpoint discovery with the
  locked matrix.
- **CTR-003**: Tests MUST fail when a route is added, removed, duplicated, or
  changes its method or authorization contract without updating the matrix.

## Acceptance criteria

- Runtime endpoint metadata matches the recorded contract set exactly.
- The matrix provides a regression baseline for the forthcoming controller
  migration.

## Evidence

- `tests/Compendium.ContractTests/HttpContractMatrix.cs`
- `tests/Compendium.ContractTests/HttpContractMatrixTests.cs`
