# SPEC-022: Source MVC controllers

- Status: Done
- Commit: `f3d6425c39f3f5cf8c3cd31d727be9fde4214cea`
- Completed: 2026-07-29

## Intent

Replace source minimal endpoints with MVC controllers without changing the
public HTTP contract.

## Implemented requirements

- **MVC-SRC-001**: Rule source, ruleset, and source-version routes MUST be owned
  by dedicated controllers.
- **MVC-SRC-002**: Actions MUST preserve existing methods, route templates,
  payloads, status codes, operation names, and administrative-write metadata.
- **MVC-SRC-003**: The legacy source endpoint mapper MUST be removed from startup
  and source registrations needed by controllers MUST be composed centrally.

## Acceptance criteria

- The runtime source routes match the pre-refactor HTTP matrix.
- Existing source endpoint tests pass against controller actions.
- No duplicate minimal and controller endpoints are registered.

## Evidence

- `src/Compendium.API/Sources/RuleSourcesController.cs`
- `src/Compendium.API/Sources/RulesetsController.cs`
- `src/Compendium.API/Sources/SourceVersionsController.cs`
- `tests/Compendium.ContractTests/SourceEndpointTests.cs`
