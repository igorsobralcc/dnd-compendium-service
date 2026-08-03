# SPEC-036: Structured service README

- Status: Done
- Commit: `a0959cc76793330e798dffd32183f51cbfe9bcbc`
- Completed: 2026-08-01

## Intent

Turn the root README into a coherent entry point for contributors, operators,
and API consumers.

## Implemented requirements

- **README-001**: The README MUST explain service scope and non-responsibilities.
- **README-002**: It MUST summarize architecture, project layout, domain
  capabilities, HTTP surface, security, messaging, observability, persistence,
  configuration, and contract versioning.
- **README-003**: It MUST provide runnable local, test, migration, container,
  and operational commands that match repository paths.
- **README-004**: It MUST link detailed architecture and operator documentation
  instead of duplicating all detail.

## Acceptance criteria

- A new contributor can build, test, run, configure, and navigate the service
  starting from the root README.
- Commands and links reference files that exist in the repository.

## Evidence

- `README.md`
