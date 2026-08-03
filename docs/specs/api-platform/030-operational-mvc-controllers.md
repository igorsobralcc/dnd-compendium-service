# SPEC-030: Operational MVC controllers

- Status: Done
- Commit: `68ff48f920f5b9ad77691971e16022467a4a03a5`
- Completed: 2026-07-31

## Intent

Complete controller ownership of service status and metadata routes while
leaving health-check middleware intact.

## Implemented requirements

- **MVC-OPS-001**: The root service-status route MUST be owned by
  `ServiceStatusController` and remain anonymous.
- **MVC-OPS-002**: Compendium metadata MUST be owned by
  `CompendiumMetadataController` with its existing access policy.
- **MVC-OPS-003**: Route templates, verbs, response payloads, endpoint names,
  and status codes MUST remain compatible.
- **MVC-OPS-004**: Equivalent inline minimal-route declarations MUST be removed
  from startup.

## Acceptance criteria

- The operational routes match the locked HTTP matrix.
- Service status and metadata return their established response contracts.
- Health endpoints continue to be mapped through health-check middleware.

## Evidence

- `src/Compendium.API/Operations/ServiceStatusController.cs`
- `src/Compendium.API/Operations/CompendiumMetadataController.cs`
- `src/Compendium.API/Program.cs`
