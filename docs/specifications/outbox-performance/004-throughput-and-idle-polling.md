# SPEC-OUTBOX-004: Throughput and idle-polling optimization

- Status: Deferred pending measurements
- Priority: Conditional
- Depends on: SPEC-OUTBOX-003

## Activation criteria

This specification MUST NOT be implemented merely because the earlier changes
exist. At least one of the following must be demonstrated from production-like
measurements:

- sustained batches reach `BatchSize` and backlog age grows;
- per-message `SaveChangesAsync` calls materially contribute to database load;
- empty dispatch selections remain a material share of database statements
  after telemetry decoupling; or
- the external transport's latency requires controlled parallelism to meet an
  agreed delivery objective.

The implementation review must include baseline publication throughput,
database statements per message, p95 oldest-message age, and duplicate-delivery
observations.

## Batched completion persistence

- **THR-001**: Completion batching MUST operate only on messages already owned
  through SPEC-OUTBOX-003 claims.
- **THR-002**: The worker MAY accumulate completed outcomes and persist them in
  bounded groups instead of calling `SaveChangesAsync` after every message.
- **THR-003**: Completion updates MUST retain claim-token fencing.
- **THR-004**: The maximum completion group size MUST be configurable, default
  to 10, and never exceed `BatchSize`.
- **THR-005**: If a grouped completion transaction fails, claimed rows MUST
  remain recoverable through lease expiry. The worker MUST NOT assume the
  external publications were rolled back.
- **THR-006**: Documentation MUST state that a larger completion group increases
  the potential duplicate-redelivery window after a process or database
  failure.

## Controlled publish concurrency

- **THR-007**: Publish concurrency MUST default to one to preserve current
  behavior.
- **THR-008**: If enabled, concurrency MUST be bounded by a validated setting
  between one and 16 and MUST NOT share a `DbContext` across concurrent tasks.
- **THR-009**: The selected `IEventTransport` implementation must explicitly be
  verified as safe for concurrent calls before a value above one is enabled.
- **THR-010**: Strict event ordering is not guaranteed when concurrency is
  greater than one. Environments requiring ordering MUST keep concurrency at
  one or adopt a separately specified aggregate-partitioning design.
- **THR-011**: The lease duration or lease-renewal strategy MUST cover the
  maximum expected processing duration for the claimed batch.

## Adaptive idle polling

- **IDL-001**: Adaptive idle polling MUST be disabled by default so the initial
  implementation preserves the current two-second maximum idle latency.
- **IDL-002**: When enabled, consecutive empty selections MAY increase the delay
  exponentially from `PollingInterval` up to `IdlePollingMaxInterval`, which
  defaults to 30 seconds.
- **IDL-003**: Receiving a non-empty batch or encountering expired work MUST
  immediately reset the delay to `PollingInterval`.
- **IDL-004**: Configuration and runbooks MUST clearly state that adaptive idle
  polling can delay the first event after an idle period by as much as
  `IdlePollingMaxInterval`.
- **IDL-005**: A PostgreSQL `LISTEN/NOTIFY` wake-up mechanism MAY be specified in
  a future document to retain low latency while reducing empty polls. It is not
  authorized by this specification because it introduces trigger, connection,
  reconnect, and missed-notification behavior that requires separate design.

## Acceptance scenarios

### AC-THR-01: Bounded completion writes

Given 50 successfully published claimed messages and a completion group size of
10, when the batch completes normally, then at most five grouped completion
transactions are used and every update is fenced by the active claim token.

### AC-THR-02: Failed completion remains recoverable

Given externally published messages whose grouped completion transaction
fails, when their leases expire, then another worker can reclaim them and the
system does not leave them permanently in `PROCESSING`.

### AC-THR-03: Concurrency limit is respected

Given publish concurrency of four and a transport that records active calls,
when a batch is processed, then no more than four calls are active at once and
no `DbContext` instance is used concurrently.

### AC-IDL-01: Empty polling backs off and resets

Given adaptive polling is enabled, when several empty selections occur, then
the delay grows but never exceeds the configured maximum; after a non-empty
selection, the next delay returns to the base polling interval.

## Success criteria

The optimization is accepted only if a production-like benchmark shows a
material improvement in the activation metric without increasing publication
failures, stale claims, or database lock waits. "Material" must be declared
before implementation; a recommended threshold is at least 25% fewer database
statements per published message or at least 25% higher sustained throughput.
