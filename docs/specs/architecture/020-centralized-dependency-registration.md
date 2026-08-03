# SPEC-020: Centralized dependency registration

- Status: Done
- Commit: `0dd0d989be8f484ca861a2335afc3183ee5cc80d`
- Completed: 2026-07-29

## Intent

Move service composition out of the application and infrastructure assemblies
into one cross-cutting registration entry point.

## Implemented requirements

- **DI-001**: API startup MUST register Compendium services through the
  cross-cutting composition extension.
- **DI-002**: Application use cases, repositories, persistence, messaging,
  telemetry, and hosted services MUST retain their established lifetimes.
- **DI-003**: Application and infrastructure internals needed for composition
  MUST be exposed only to the cross-cutting assembly and designated tests.
- **DI-004**: The Application project MUST NOT depend on infrastructure or DI
  implementation packages merely to self-register.

## Acceptance criteria

- Resolving the service graph produces every required application and
  infrastructure service with the intended lifetime.
- Registration tests detect omissions or duplicate composition regressions.

## Evidence

- `src/Compendium.CrossCutting/CompendiumServiceCollectionExtensions.cs`
- `src/Compendium.Application/Properties/AssemblyInfo.cs`
- `src/Compendium.Infra/Properties/AssemblyInfo.cs`
- `tests/Compendium.IntegrationTests/DependencyInjection/CompendiumServiceRegistrationTests.cs`
