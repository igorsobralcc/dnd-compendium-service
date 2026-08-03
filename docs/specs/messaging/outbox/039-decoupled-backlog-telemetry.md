# SPEC-039: Decoupled Outbox backlog telemetry

- Status: Done
- Commit: `8154fcd619ffe39126d1cdeb98d39c6f174e63bf`
- Completed: 2026-08-03

## Intent

Remove an exact backlog count from the latency-sensitive dispatch loop and
collect the same operational signal on an independent cadence.

## Implemented requirements

- **TEL-IMP-001**: `OutboxDispatcher` MUST NOT execute an exact backlog count.
- **TEL-IMP-002**: A hosted collector MUST count unresolved rows immediately on
  startup and then at `BacklogMetricsInterval`, defaulting to one minute.
- **TEL-IMP-003**: The interval MUST validate between five seconds and one hour,
  while dispatch polling remains two seconds by default.
- **TEL-IMP-004**: Collection failures MUST preserve the cached value, increment
  a failure counter, log the exception, and leave dispatch running.
- **TEL-IMP-005**: Deprecated `compendium.outbox.pending` and replacement
  `compendium.outbox.unresolved` gauges MUST read the same in-memory value; metric
  callbacks MUST perform no database work.

## Acceptance criteria

- Repeated dispatch cycles issue no count query.
- Both gauge instruments observe one cached backlog value.
- Collector cancellation is graceful and failures do not terminate the host.

## Evidence

- `src/Compendium.Infra/Integration/OutboxBacklogCollector.cs`
- `src/Compendium.Application/Observability/CompendiumTelemetry.cs`
- `tests/Compendium.UnitTests/Observability/CompendiumTelemetryTests.cs`
- `docs/specs/messaging/outbox/001-decoupled-backlog-telemetry.md`
