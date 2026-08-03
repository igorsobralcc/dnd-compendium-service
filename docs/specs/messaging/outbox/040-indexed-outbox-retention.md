# SPEC-040: Indexed Outbox retention cleanup

- Status: Done
- Commit: `175f7d45102fdf3f8545de19fb73f7e97359cfdd`
- Completed: 2026-08-03

## Intent

Keep active dispatch and retention queries efficient while providing a safe,
bounded, opt-in mechanism for deleting old published messages.

## Implemented requirements

- **RET-IMP-001**: A migration MUST create concurrent partial indexes for due
  Pending/Failed rows and Published retention candidates.
- **RET-IMP-002**: Cleanup MUST be disabled by default and expose validated
  retention, interval, batch-size, batch-count, and inter-batch-delay settings.
- **RET-IMP-003**: Each set-based batch MUST delete only Published rows older
  than the cutoff, use `FOR UPDATE SKIP LOCKED`, and rely on child-row cascade.
- **RET-IMP-004**: A run MUST stop at the configured batch bound, after a short
  batch, or on cancellation; it MUST NOT materialize message entities.
- **RET-IMP-005**: Deletions and failures MUST emit metrics and structured logs;
  cleanup errors MUST not stop dispatching.

## Acceptance criteria

- Default configuration deletes no rows.
- Enabled cleanup never removes Pending, Failed, Processing, or DeadLetter rows.
- Concurrent workers can skip locked candidates and each run is bounded.

## Evidence

- `src/Compendium.Infra/Integration/OutboxCleanupService.cs`
- `src/Compendium.Infra/Persistence/Migrations/20260803090000_AddOutboxPerformanceIndexes.cs`
- `tests/Compendium.IntegrationTests/Persistence/CompendiumMigrationTests.cs`
- `docs/specs/messaging/outbox/002-active-indexes-and-retention.md`
