# SPEC-002: Bootstrap service architecture

- Status: Done
- Commit: `62c38e405394f90c98b9b0165aea3df87b8c6dfc`
- Completed: 2026-06-11

## Intent

Create a runnable .NET compendium service with layered projects, PostgreSQL
persistence, shared result primitives, health endpoints, and baseline tests.

## Implemented requirements

- **BOOT-001**: The solution MUST separate API, application, domain, and
  infrastructure concerns and target .NET 10.
- **BOOT-002**: The API MUST expose service status, metadata, liveness, and
  readiness endpoints with centralized application-error mapping.
- **BOOT-003**: Persistence MUST use the `compendium` schema and create Inbox,
  Outbox, and Outbox-field tables through an initial EF Core migration.
- **BOOT-004**: Shared IDs, results, errors, pagination, and clock abstractions
  MUST be available without infrastructure dependencies.
- **BOOT-005**: Unit, integration, and contract test projects MUST establish
  the verification layers.

## Acceptance criteria

- The solution builds and the API starts with configuration-based PostgreSQL.
- Health contract tests pass and migrations create the initial schema.
- ID and pagination invariants are covered by unit tests.

## Evidence

- `dnd-compendium-service.slnx`
- `src/Compendium.API/Program.cs`
- `src/Compendium.Infra/Persistence/Migrations/20260611210000_InitialCompendiumSchema.cs`
- `tests/Compendium.ContractTests/HealthEndpointTests.cs`
