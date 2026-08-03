# SPEC-OUTBOX-003: Concurrent-safe claiming and leases

- Status: Accepted
- Priority: High before horizontal scaling
- Depends on: SPEC-OUTBOX-002 active-message indexing

## Problem

The dispatcher currently selects active records without locking or changing
their state before publication. Two application instances can select the same
records and publish them concurrently. A process crash also has no explicit
ownership or recovery model.

## Persisted state model

- **CLM-001**: `IntegrationMessageStatus` MUST add `PROCESSING`.
- **CLM-002**: `integration_outbox` MUST add nullable columns:

  | Column | Type | Purpose |
  | --- | --- | --- |
  | `claim_token` | `uuid` | fencing token for one claim attempt |
  | `processing_owner` | `varchar(128)` | diagnostic service-instance identity |
  | `processing_started_at_utc` | `timestamptz` | claim start time |
  | `lease_expires_at_utc` | `timestamptz` | crash-recovery boundary |

- **CLM-003**: The columns MUST be nullable for existing rows and MUST be set
  together whenever status becomes `PROCESSING`.
- **CLM-004**: Successful or failed completion MUST clear all claim fields.
- **CLM-005**: A partial index MUST support expired-lease discovery:

  ```sql
  CREATE INDEX CONCURRENTLY ix_integration_outbox_processing_lease
  ON compendium.integration_outbox (lease_expires_at_utc, created_at_utc)
  WHERE status = 'PROCESSING';
  ```

## Configuration

- **CLM-006**: `IntegrationMessagingOptions` MUST expose
  `ProcessingLeaseDuration`, defaulting to `00:02:00`.
- **CLM-007**: `IntegrationMessagingOptions` MUST expose
  `PublishAttemptTimeout`, defaulting to `00:00:30`.
- **CLM-008**: Each application instance MUST have a stable, log-safe
  `processing_owner` value for its lifetime. The value MUST distinguish
  concurrently running instances.
- **CLM-009**: startup validation MUST require a lease of at least 30 seconds, a
  positive publish timeout, and a publish timeout shorter than half the lease.
- **CLM-010**: Every transport call MUST use a linked cancellation token bounded
  by `PublishAttemptTimeout` in addition to application shutdown.

## Atomic claim algorithm

- **CLM-011**: A dispatcher MUST atomically claim at most `BatchSize` rows in a
  short PostgreSQL transaction.
- **CLM-012**: Eligible rows are:

  1. `PENDING` or `FAILED` with `available_at_utc <= now`; or
  2. `PROCESSING` with `lease_expires_at_utc <= now`.

- **CLM-013**: Selection MUST preserve best-effort `created_at_utc` ordering and
  use `FOR UPDATE SKIP LOCKED` or an equivalent atomic update pattern.
- **CLM-014**: Claiming MUST assign a newly generated `claim_token`, owner,
  processing start, and lease expiry before the transaction commits.
- **CLM-015**: Publishing MUST occur only after the claim transaction commits.
- **CLM-016**: The database transaction MUST NOT remain open during calls to
  `IEventTransport.PublishAsync`.
- **CLM-017**: An expired claim MAY be reclaimed without incrementing the broker
  failure retry count. Reclaiming MUST replace the claim token so the old worker
  is fenced out.
- **CLM-018**: Before publishing the next message, a worker MUST renew the lease
  for its unfinished rows when less than half of `ProcessingLeaseDuration`
  remains. Renewal MUST match the current claim token.
- **CLM-019**: A failed or zero-row lease renewal MUST stop that worker from
  publishing the affected rows because ownership can no longer be proven.

A conforming implementation may use a CTE shaped like the following; exact SQL
and EF Core integration are implementation details:

```sql
WITH candidates AS (
    SELECT id
    FROM compendium.integration_outbox
    WHERE
        (status IN ('PENDING', 'FAILED') AND available_at_utc <= @now)
        OR
        (status = 'PROCESSING' AND lease_expires_at_utc <= @now)
    ORDER BY created_at_utc
    FOR UPDATE SKIP LOCKED
    LIMIT @batch_size
)
UPDATE compendium.integration_outbox AS o
SET status = 'PROCESSING',
    claim_token = @claim_token,
    processing_owner = @owner,
    processing_started_at_utc = @now,
    lease_expires_at_utc = @lease_expiry,
    updated_at_utc = @now
FROM candidates AS c
WHERE o.id = c.id
RETURNING o.id;
```

## Completion and fencing

- **CLM-020**: A completion update MUST match both the message ID and current
  `claim_token` while status is `PROCESSING`.
- **CLM-021**: A worker whose token no longer matches MUST NOT overwrite the
  newer owner's state. It MUST log a structured stale-claim warning.
- **CLM-022**: Successful publication MUST transition the owned row to
  `PUBLISHED`, set `published_at_utc`, and clear claim fields.
- **CLM-023**: Failed publication MUST apply the existing retry-count,
  `FAILED`/`DEAD_LETTER`, retry-delay, and error-truncation behavior, then clear
  claim fields.
- **CLM-024**: Cancellation after external publication but before the completion
  update MAY cause later redelivery after lease expiry. This is expected
  at-least-once behavior and MUST be documented in logs and consumer guidance.

## Telemetry changes

- **CLM-025**: The unresolved backlog gauge from SPEC-OUTBOX-001 MUST include
  `PROCESSING` rows.
- **CLM-026**: Metrics SHOULD expose the number of active processing claims and
  expired leases recovered.
- **CLM-027**: Logs for claim, publish, failure, stale completion, and lease
  recovery MUST carry `event_id`, `correlation_id`, `claim_token`, and
  `processing_owner` where available.

## Acceptance scenarios

### AC-CLM-01: Two workers claim disjoint batches

Given 100 eligible messages and two workers claiming 50 concurrently, when both
claim transactions commit, then each worker owns 50 unique IDs and the
intersection is empty.

### AC-CLM-02: Unexpired work is not stolen

Given a processing row with a future lease expiry, when another worker claims a
batch, then that row is not returned or modified.

### AC-CLM-03: Expired work is recovered

Given a processing row whose lease expired, when another worker claims, then it
receives the row with a new claim token and owner without increasing
`retry_count`.

### AC-CLM-04: Old owner is fenced

Given worker B reclaimed an expired row from worker A, when worker A later
attempts to mark the row published using its old token, then zero rows are
updated and worker B's claim remains intact.

### AC-CLM-05: Broker failure retains retry behavior

Given an owned row and a transport failure, when the completion is persisted,
then retry count increases, status becomes `FAILED` or `DEAD_LETTER` according
to `MaxRetries`, `available_at_utc` follows `RetryDelay`, and claim fields are
cleared.

### AC-CLM-06: No transaction spans broker I/O

Given a transport that blocks, when a claimed message is being published, then
the claim transaction is already committed and unrelated workers can claim
other rows without waiting for the transport call.

### AC-CLM-07: Long batches retain ownership

Given a sequential batch whose total processing time exceeds one lease period,
when the worker remains healthy, then it renews the unfinished claims before
expiry and another worker cannot claim them.

### AC-CLM-08: Publish attempts are bounded

Given a transport call that does not complete, when `PublishAttemptTimeout`
elapses, then the call is cancelled, existing retry behavior is applied when
ownership still matches, and no database transaction has remained open during
the wait.

## Verification

Concurrency and lease scenarios MUST run against PostgreSQL, not an in-memory
provider. Tests must use independent connections and synchronize workers so the
claims genuinely overlap. Consumer idempotency tests remain required because
claiming prevents concurrent ownership but cannot remove the publish/commit
failure window.
