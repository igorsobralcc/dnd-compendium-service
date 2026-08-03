# SPEC-015: Observability and quality gates

- Status: Done
- Commit: `5b48ec5164e30dd2cf87cc677e988636f28c623a`
- Completed: 2026-07-26

## Intent

Make HTTP, database, and messaging behavior observable and enforce repeatable
build, test, coverage, and contract-quality checks.

## Implemented requirements

- **OBS-001**: Request middleware MUST record request count and duration with
  bounded route, method, and status dimensions and propagate correlation IDs.
- **OBS-002**: Database interception MUST record command duration and failures
  without exposing SQL payloads as high-cardinality metric attributes.
- **OBS-003**: Outbox dispatch MUST report publication outcomes and backlog
  through shared service telemetry.
- **OBS-004**: Health and metrics contracts MUST remain externally verifiable.
- **OBS-005**: CI MUST restore, build, test, and collect coverage using the
  committed quality workflow and run settings.
- **OBS-006**: HTTP contract versioning rules MUST be documented.

## Acceptance criteria

- Requests, database calls, and Outbox activity emit the expected metrics.
- Unit and contract tests verify metric instruments and the health surface.
- The repository quality workflow can run from a clean checkout.

## Evidence

- `.github/workflows/quality.yml`
- `src/Compendium.Application/Observability/CompendiumTelemetry.cs`
- `src/Compendium.API/Observability/RequestObservabilityMiddleware.cs`
- `tests/Compendium.UnitTests/Observability/CompendiumTelemetryTests.cs`
