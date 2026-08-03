# SPEC-019: Cross-cutting composition project

- Status: Done
- Commit: `c4c0872673c97ab85d49d94016a56fad261155b8`
- Completed: 2026-07-29

## Intent

Introduce a composition layer for concerns that coordinate API, application,
and infrastructure without weakening the domain boundary.

## Implemented requirements

- **XCT-001**: The solution MUST contain a `Compendium.CrossCutting` project.
- **XCT-002**: The API MUST reference the composition project so startup can be
  migrated to centralized registrations and pipeline configuration.
- **XCT-003**: The new project MUST reference application and infrastructure but
  MUST NOT introduce a reverse dependency into the domain.

## Acceptance criteria

- The solution builds with the new project in its project graph.
- The project is available as the single composition entry point for subsequent
  refactors.

## Evidence

- `src/Compendium.CrossCutting/Compendium.CrossCutting.csproj`
- `src/Compendium.API/Compendium.API.csproj`
- `dnd-compendium-service.slnx`
