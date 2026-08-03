# SPEC-OUTBOX-002: Active indexes and retention

- Status: In Progress (implementation complete; production plan evidence and enablement pending)
- Priority: High
- Depends on: SPEC-OUTBOX-001 for the preferred release order

## Problem

The general `status` index contains terminal as well as active records. As the
table accumulates `PUBLISHED` rows, active-message access paths, vacuum work,
backups, and storage grow. There is no automated retention mechanism.

## Active-message indexes

- **DB-001**: A migration MUST add a PostgreSQL partial index for rows with
  status `PENDING` or `FAILED`.
- **DB-002**: The index MUST support filtering by `available_at_utc` and the
  dispatcher's best-effort `created_at_utc` ordering.
- **DB-003**: The first candidate index definition MUST be benchmarked as:

  ```sql
  CREATE INDEX CONCURRENTLY ix_integration_outbox_active_available_created
  ON compendium.integration_outbox (available_at_utc, created_at_utc)
  WHERE status IN ('PENDING', 'FAILED');
  ```

- **DB-004**: If production-like `EXPLAIN (ANALYZE, BUFFERS)` shows that sorting
  due rows dominates, the implementation MAY instead use an index beginning
  with `created_at_utc` and include `available_at_utc`. The chosen plan and
  evidence MUST be recorded in the pull request.
- **DB-005**: The existing general status index MUST remain during the first
  rollout. It MAY be removed in a later migration only after query statistics
  show no remaining useful access path.
- **DB-006**: Production index creation SHOULD use `CONCURRENTLY`. An EF Core
  migration using concurrent creation MUST suppress the migration transaction
  for that statement and document recovery from an invalid interrupted index.

## Retention policy

- **RET-001**: Published-message cleanup MUST be opt-in and disabled by default.
- **RET-002**: Default settings MUST be:

  | Setting | Default |
  | --- | --- |
  | `CleanupEnabled` | `false` |
  | `PublishedRetention` | `30.00:00:00` |
  | `CleanupInterval` | `01:00:00` |
  | `CleanupBatchSize` | `1000` |
  | `CleanupMaxBatchesPerRun` | `10` |
  | `CleanupInterBatchDelay` | `00:00:00.100` |

- **RET-003**: startup validation MUST reject non-positive intervals and batch
  sizes, retention below one day, or more than 100,000 deleted rows per run
  based on `CleanupBatchSize * CleanupMaxBatchesPerRun`.
- **RET-004**: Cleanup MUST delete only rows where status is `PUBLISHED` and
  `published_at_utc` is older than the calculated cutoff.
- **RET-005**: Cleanup MUST NOT delete `PENDING`, `FAILED`, `PROCESSING`, or
  `DEAD_LETTER` records.
- **RET-006**: Child `integration_outbox_fields` rows MUST be removed through
  the existing cascade relationship.
- **RET-007**: Each cleanup batch MUST use a short, independent transaction and
  MUST NOT load message entities or fields into EF Core memory.
- **RET-008**: Cleanup MUST stop after `CleanupMaxBatchesPerRun`, when fewer than
  `CleanupBatchSize` rows are deleted, or when shutdown is requested.
- **RET-009**: Concurrent cleanup workers MUST skip locked candidates or use an
  equivalent technique that prevents workers from blocking one another.
- **RET-010**: A partial index MUST support retention selection:

  ```sql
  CREATE INDEX CONCURRENTLY ix_integration_outbox_published_at
  ON compendium.integration_outbox (published_at_utc)
  WHERE status = 'PUBLISHED';
  ```

## Maintenance telemetry

- **RET-011**: Successful deletion MUST increment
  `compendium.outbox.cleanup.deleted` by the number of parent rows removed.
- **RET-012**: A failed cleanup run MUST increment
  `compendium.outbox.cleanup.failures`, log the error, and leave dispatching
  operational.
- **RET-013**: Logs MUST include cutoff, batch size, rows removed, elapsed time,
  and whether more eligible rows may remain.

## Acceptance scenarios

### AC-DB-01: Active query uses the intended access path

Given a production-like table dominated by published rows, when the dispatch
query is explained, then PostgreSQL uses the active partial index or a measured
plan with equal or lower buffers and execution time. The evidence is attached
to the implementation review; this is an operational verification rather than
a timing assertion in CI.

### AC-RET-01: Cleanup is safe by default

Given default configuration, when the application runs for longer than the
cleanup interval, then no Outbox rows are deleted.

### AC-RET-02: Eligible records are deleted in a bound

Given 2,500 expired published rows, a batch size of 1,000, and a maximum of two
batches, when one cleanup run completes, then exactly 2,000 parent rows and
their child field rows are deleted and at least 500 eligible rows remain.

### AC-RET-03: Non-terminal records survive

Given old records in every Outbox status, when cleanup runs, then only old
`PUBLISHED` rows are removed; all other statuses remain.

### AC-RET-04: Concurrent maintenance does not double count

Given two cleanup workers, when they run concurrently, then a row is deleted by
at most one worker, neither worker waits on locks held during another worker's
entire run, and deletion metrics equal the actual number removed.

## Operational enablement gate

Before setting `CleanupEnabled=true` in an environment, the operator MUST:

1. record current row counts grouped by status and age;
2. confirm whether published messages have audit or replay obligations;
3. take or verify a recoverable backup;
4. confirm the partial retention index is valid; and
5. start with the default bounded deletion rate while observing database locks,
   WAL volume, replication lag, and API latency.

No automated cleanup of dead letters may be enabled through configuration.
