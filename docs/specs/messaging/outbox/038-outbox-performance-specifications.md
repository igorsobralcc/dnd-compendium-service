# SPEC-038: Outbox performance specification suite

- Status: Done
- Commit: `4606747b1282a8eab988f6e9b1721f213f8b3ea8`
- Completed: 2026-08-03

## Intent

Define an implementation-ready performance and concurrency program for the
transactional Outbox before changing production behavior.

## Implemented requirements

- **SPEC-DOC-001**: The suite MUST specify independent backlog telemetry,
  compatible metric semantics, configuration, failure handling, and rollout.
- **SPEC-DOC-002**: It MUST specify active partial indexes, opt-in bounded
  retention, maintenance telemetry, and an operational enablement gate.
- **SPEC-DOC-003**: It MUST specify atomic claim leases, fencing, recovery,
  timeout, renewal, at-least-once behavior, and PostgreSQL concurrency tests.
- **SPEC-DOC-004**: Throughput and idle-polling optimizations MUST remain gated
  on measurements and explicitly documented tradeoffs.
- **SPEC-DOC-005**: An implementation plan MUST map requirements to delivery
  phases, evidence, verification, and definition of done.

## Acceptance criteria

- Each Outbox concern has requirement IDs and acceptance scenarios.
- Dependencies and delivery order are explicit.
- Deferred work cannot be mistaken for authorized implementation.

## Evidence

- `docs/specifications/README.md`
- `docs/specifications/outbox-performance/README.md`
- `docs/specifications/outbox-performance/001-decoupled-backlog-telemetry.md`
- `docs/specifications/outbox-performance/implementation-plan.md`
