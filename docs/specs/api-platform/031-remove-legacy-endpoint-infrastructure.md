# SPEC-031: Remove legacy endpoint infrastructure

- Status: Done
- Commit: `7fe24f2ea5b285c2a53b3aac658f9432d2ed6861`
- Completed: 2026-07-31

## Intent

Remove route-shape authorization logic made obsolete after every application
route became controller-backed.

## Implemented requirements

- **LEG-001**: Authorization MUST be determined from controller/action endpoint
  metadata and standard ASP.NET authorization, not inferred from path prefixes
  and HTTP methods.
- **LEG-002**: `CompendiumRouteAuthorizationMiddleware` MUST be deleted and
  removed from pipeline composition.
- **LEG-003**: Every application route MUST carry a controller action descriptor.
- **LEG-004**: Swagger operations MUST remain identical to the locked route,
  method, and operation-name matrix.

## Acceptance criteria

- All 83 application routes are controller-backed.
- Runtime endpoint discovery and the Swagger document match the HTTP matrix.
- Protected routes retain their controller-declared policies.

## Evidence

- `src/Compendium.CrossCutting/Security/CompendiumSecurity.cs`
- `src/Compendium.CrossCutting/Http/CompendiumPipelineExtensions.cs`
- `tests/Compendium.ContractTests/HttpContractMatrixTests.cs`
