# SPEC-OUTBOX-001: Decoupled backlog telemetry

- Status: Accepted
- Priority: High
- Depends on: None

## Problem

`OutboxDispatcher.DispatchBatchAsync` performs an exact `LongCountAsync` during
every dispatch cycle. The value is only copied into an in-memory observable
gauge. This couples a relatively expensive operational measurement to a
latency-sensitive worker and runs it far more frequently than metrics are
normally scraped.

The gauge is also updated before the selected messages are processed, so the
value immediately overstates the remaining backlog during active dispatch.

## Required behavior

### Collector lifecycle

- **TEL-001**: The exact backlog query MUST be removed from
  `OutboxDispatcher.DispatchBatchAsync`.
- **TEL-002**: A dedicated hosted component MUST refresh the backlog metric
  independently of event dispatch.
- **TEL-003**: The collector MUST run once after application startup and then no
  more frequently than `BacklogMetricsInterval` after a successful or failed
  collection attempt.
- **TEL-004**: Collection attempts MUST NOT overlap within one application
  instance.
- **TEL-005**: Cancellation during shutdown MUST stop the collector without
  being logged as an application failure.

### Configuration

- **TEL-006**: `IntegrationMessagingOptions` MUST expose
  `BacklogMetricsInterval` with a default value of `00:01:00`.
- **TEL-007**: startup validation MUST reject intervals below five seconds or
  above one hour.
- **TEL-008**: `PollingInterval` MUST remain `00:00:02` by default; implementing
  this specification MUST NOT change dispatch latency.

Suggested configuration:

```json
{
  "IntegrationMessaging": {
    "PollingInterval": "00:00:02",
    "BacklogMetricsInterval": "00:01:00"
  }
}
```

### Metric semantics

- **TEL-009**: Until SPEC-OUTBOX-003 is implemented, unresolved backlog is the
  exact number of rows whose status is `PENDING` or `FAILED`, whether or not
  `available_at_utc` has arrived.
- **TEL-010**: After SPEC-OUTBOX-003 is implemented, unresolved backlog MUST
  also include `PROCESSING` rows.
- **TEL-011**: The existing `compendium.outbox.pending` gauge MUST remain
  available for one compatibility release and MUST observe the same cached
  value without causing another query.
- **TEL-012**: A replacement gauge named `compendium.outbox.unresolved` SHOULD
  be introduced with the description "Outbox messages not yet published or
  dead-lettered."
- **TEL-013**: Dashboards MUST aggregate this database-global gauge with `max`
  across service instances, not `sum`, because every instance observes the
  same shared database backlog.
- **TEL-014**: Metric callbacks MUST only read the last cached value from
  memory. They MUST NOT query PostgreSQL.

### Failure behavior

- **TEL-015**: If collection fails, the last successful gauge value MUST be
  retained.
- **TEL-016**: A failed collection MUST increment
  `compendium.outbox.backlog.collection.failures` and emit one structured
  warning or error containing the exception.
- **TEL-017**: Collector failure MUST NOT stop dispatching or terminate the
  application.
- **TEL-018**: Successful collection MUST record the collection duration in a
  histogram or through the existing database command instrumentation.

## Acceptance scenarios

### AC-TEL-01: Idle dispatcher does not count

Given an empty Outbox and a running dispatcher, when multiple dispatch cycles
complete, then the dispatcher executes its batch-selection query but does not
execute an exact backlog count.

### AC-TEL-02: Collector cadence

Given a one-minute backlog interval, when the collector starts, then it performs
one immediate collection and does not start a second collection before one
minute has elapsed after the first attempt.

### AC-TEL-03: Non-overlapping slow query

Given a collection that takes longer than the configured interval, when the
timer becomes due, then no second collection starts until the current one has
finished and the next interval has elapsed.

### AC-TEL-04: Failure preserves telemetry

Given a previously cached value of 12, when PostgreSQL rejects the next count,
then the gauge remains 12, the failure counter increases by one, dispatching
continues, and no unhandled exception escapes the collector.

### AC-TEL-05: Compatibility metrics share one query

Given both the deprecated and replacement gauges are enabled, when Prometheus
scrapes `/metrics`, then both expose the same value and the scrape causes zero
database queries.

## Verification

- Unit-test cadence and cancellation with a controllable `TimeProvider` or an
  equivalent deterministic clock.
- Integration-test the count predicate against PostgreSQL.
- Capture command telemetry in a hosted-service test to prove that repeated
  dispatch cycles do not execute `COUNT(*)`.
- Verify `/metrics` contains the compatibility and replacement metric names.

## Rollout

Deploy without changing `PollingInterval`. Compare database statement counts
for at least one normal traffic window. The specification succeeds when the
count statement falls from approximately 30 executions per minute per instance
to no more than one execution per minute per instance after startup.
