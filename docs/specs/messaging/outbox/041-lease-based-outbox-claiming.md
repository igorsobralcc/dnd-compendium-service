# SPEC-041: Lease-based Outbox message claiming

- Status: Done
- Commit: `0c3585ed79b007f823738918e4e0586ed11dc686`
- Completed: 2026-08-03

## Intent

Prevent concurrent dispatchers from publishing the same live work while
allowing crashed workers' messages to recover through expiring leases.

## Implemented requirements

- **CLM-IMP-001**: Outbox state MUST add Processing plus nullable claim token,
  owner, processing-start, and lease-expiry fields and a partial lease index.
- **CLM-IMP-002**: A dispatcher MUST atomically claim ordered eligible rows with
  `FOR UPDATE SKIP LOCKED`, commit the claim, and only then call the transport.
- **CLM-IMP-003**: Each instance MUST have a stable log-safe worker identity;
  every claim attempt MUST use a new fencing token.
- **CLM-IMP-004**: Lease duration and publish timeout MUST be configurable and
  validated; transport calls MUST use the bounded timeout.
- **CLM-IMP-005**: Long-running batches MUST renew unfinished leases, and a
  worker that cannot prove ownership MUST stop processing affected rows.
- **CLM-IMP-006**: Success and failure completion MUST match the active token so
  stale workers cannot overwrite a newer claim.
- **CLM-IMP-007**: Expired claims MUST be recoverable, Processing rows MUST count
  as unresolved, and recovery/stale ownership MUST be observable.

## Acceptance criteria

- Concurrent claim transactions return disjoint unexpired ownership sets.
- Expired work receives a new token without incrementing broker retry count.
- No database transaction remains open during external publish I/O.
- A stale token updates zero rows and cannot overwrite the current owner.

## Evidence

- `src/Compendium.Infra/Integration/OutboxDispatcher.cs`
- `src/Compendium.Infra/Integration/OutboxWorkerIdentity.cs`
- `src/Compendium.Infra/Persistence/Migrations/20260803202814_AddOutboxConcurrentClaims.cs`
- `docs/specifications/outbox-performance/003-concurrent-safe-claiming.md`
