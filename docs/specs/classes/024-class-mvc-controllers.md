# SPEC-024: Class MVC controllers

- Status: Done
- Commit: `2e75ee86cef49e14f30ceee5583b46adfe5d4fb2`
- Completed: 2026-07-31

## Intent

Move class and subclass HTTP operations from minimal endpoints to MVC.

## Implemented requirements

- **MVC-CLS-001**: Class operations, including progression queries, MUST be
  exposed by `ClassesController`.
- **MVC-CLS-002**: Subclass operations MUST be exposed by
  `SubclassesController`.
- **MVC-CLS-003**: Routes, verbs, request/response contracts, result status
  mapping, endpoint names, and write authorization MUST remain compatible.
- **MVC-CLS-004**: The legacy class mapper MUST no longer be registered.

## Acceptance criteria

- Class and subclass operations match the locked HTTP matrix.
- Class contract tests cover successful and invalid controller requests.
- Only MVC owns the migrated routes.

## Evidence

- `src/Compendium.API/Classes/ClassesController.cs`
- `src/Compendium.API/Classes/SubclassesController.cs`
- `tests/Compendium.ContractTests/ClassEndpointTests.cs`
