# SPEC-032: DDD architecture tests

- Status: Done
- Commit: `4837e742f70f7bed73abbcb403e1e99f392fc19e`
- Completed: 2026-08-01

## Intent

Turn the intended domain-driven, ports-and-adapters dependency rules into
executable repository gates.

## Implemented requirements

- **ARCH-001**: Domain MUST remain independent of application, infrastructure,
  API, ASP.NET, and EF Core.
- **ARCH-002**: Application MUST depend on domain abstractions but not on
  infrastructure, API, ASP.NET, or EF Core implementations.
- **ARCH-003**: Infrastructure MUST NOT reference the API, and composition MUST
  remain isolated in CrossCutting.
- **ARCH-004**: Repository and project-reference layout MUST conform to the
  defined layer map.
- **ARCH-005**: API controllers MUST inherit the shared controller base and
  follow established controller conventions.

## Acceptance criteria

- Architecture tests pass for the current project graph and source namespaces.
- A prohibited dependency or nonconforming API controller makes the suite fail.

## Evidence

- `tests/Compendium.ArchitectureTests/DependencyBoundaryTests.cs`
- `tests/Compendium.ArchitectureTests/ApiConventionTests.cs`
- `tests/Compendium.ArchitectureTests/RepositoryLayout.cs`
- `dnd-compendium-service.slnx`
