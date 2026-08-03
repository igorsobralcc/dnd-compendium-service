# Outbox performance specification suite

- Status: In Progress
- Created: 2026-08-01
- Scope owner: Compendium service maintainers
- Affected components: `Compendium.Infra`, `Compendium.Application`,
  `Compendium.CrossCutting`, `Compendium.API`, PostgreSQL

## Purpose

This suite defines the required changes for reducing avoidable Outbox database
load, preserving useful operational telemetry, bounding table growth, and making
dispatch safe when more than one application instance is running.

The documents are normative. `MUST`, `MUST NOT`, `SHOULD`, and `MAY` have their
usual requirements-language meanings. Every implementation change must cite
one or more requirement IDs in its pull request and every acceptance scenario
must be covered by an automated test or an explicitly recorded operational
verification.

## Current baseline

The current `OutboxDispatcher`:

1. starts polling immediately when the application starts;
2. selects at most 50 available `PENDING` or `FAILED` records;
3. executes an exact count of every `PENDING` or `FAILED` record;
4. publishes and persists each result sequentially; and
5. waits two seconds after the cycle finishes.

When idle, the exact count runs approximately 43,200 times per day per service
instance. The count only refreshes a cached Prometheus gauge; it does not
participate in dispatch decisions.

The table has a general index on `status`, but no access path tailored to the
active subset and no retention policy. The dispatcher also has no atomic claim
or lease, so multiple instances can select the same records.

## Target outcomes

| Outcome | Target |
| --- | --- |
| Exact backlog-count frequency | At most once per 60 seconds per instance by default, plus the initial collection |
| Idle count-query reduction | At least 95% from the current two-second loop |
| Delivery polling latency | Unchanged at two seconds until a separate optimization is enabled |
| Concurrent dispatch | A row can be owned by at most one unexpired claim at a time |
| Crash recovery | Expired claims become eligible without manual database repair |
| Table growth | Published records can be deleted in bounded, opt-in batches |
| Compatibility | Existing HTTP contracts and transactional Outbox writes remain unchanged |

## Delivery order

The specifications must be delivered in this order:

1. [SPEC-OUTBOX-001: Decoupled backlog telemetry](001-decoupled-backlog-telemetry.md)
2. [SPEC-OUTBOX-002: Active indexes and retention](002-active-indexes-and-retention.md)
3. [SPEC-OUTBOX-003: Concurrent-safe claiming and leases](003-concurrent-safe-claiming.md)
4. [SPEC-OUTBOX-004: Throughput and idle-polling optimization](004-throughput-and-idle-polling.md), only when production measurements justify it

SPEC-OUTBOX-001 and the index-only portion of SPEC-OUTBOX-002 may be released
together. Claiming must precede parallel publishing or batched completion
updates. Retention must remain disabled until the operational enablement gate in
SPEC-OUTBOX-002 is approved.

## Cross-specification decisions

- The two-second dispatch interval is a delivery-latency setting. It is not a
  telemetry refresh setting.
- Observable metric callbacks must only read in-memory state; they must not open
  database connections or perform asynchronous work.
- Outbox delivery remains at-least-once. Exactly-once broker delivery is not a
  goal. Consumers remain responsible for idempotency.
- Database transactions must not remain open while calling the external event
  transport.
- Strict global or per-aggregate event ordering is not introduced by this work.
  Existing best-effort `created_at_utc` ordering is preserved when claiming.
- `DEAD_LETTER` records are never automatically deleted by this suite.
- All new intervals and batch sizes must be configurable and validated at
  application startup.

## Global non-goals

- Replacing the Outbox pattern or the event transport abstraction.
- Changing API routes, payloads, authorization, or health endpoints.
- Providing a general-purpose job scheduler.
- Building an archival warehouse for published messages.
- Claiming exactly-once delivery guarantees.

## Traceability and delivery controls

The work breakdown and requirement-to-test map are maintained in
[implementation-plan.md](implementation-plan.md). A specification change must
be reviewed before implementation when it changes metric semantics, retention
policy, delivery guarantees, or the persisted Outbox state machine.
