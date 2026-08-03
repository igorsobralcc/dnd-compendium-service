# Outbox performance implementation plan

This plan converts the normative specifications into independently reviewable
delivery slices. Requirement IDs remain authoritative if this plan and a
specification ever conflict.

Repository status and outstanding operational evidence were independently
checked in [implementation-audit.md](implementation-audit.md). The read-only
[operational-verification.sql](operational-verification.sql) captures the
required baseline and query-plan evidence without changing Outbox data.

## Phase 0: Baseline

- [ ] **BASE-01** Record deployment instance count and Prometheus scrape
  interval.
- [ ] **BASE-02** Record the count statement's calls, mean time, total time, and
  rows examined over a representative window.
- [ ] **BASE-03** Record Outbox row counts by status and age, table/index sizes,
  oldest unresolved age, and daily published volume.
- [ ] **BASE-04** Capture `EXPLAIN (ANALYZE, BUFFERS)` for the count and dispatch
  query against sanitized production-like data.

Baseline collection is read-only and must not delay the telemetry decoupling
when production statistics are unavailable. Missing values must be marked as
unknown rather than estimated as facts.

## Phase 1: Decouple telemetry

- [x] **IMP-TEL-01** Add and validate `BacklogMetricsInterval`.
- [x] **IMP-TEL-02** Implement the independent, non-overlapping backlog
  collector with cancellation and failure handling.
- [x] **IMP-TEL-03** Remove `LongCountAsync` from `DispatchBatchAsync`.
- [x] **IMP-TEL-04** Add the unresolved gauge and compatibility alias backed by
  one cached value.
- [x] **IMP-TEL-05** Add failure telemetry and structured logs.
- [x] **IMP-TEL-06** Add deterministic unit, integration, and `/metrics` tests
  covering AC-TEL-01 through AC-TEL-05.
- [x] **IMP-TEL-07** Update operational documentation with the `max`, not `sum`,
  multi-instance aggregation rule.

Requirements: TEL-001 through TEL-018.

## Phase 2: Indexes and bounded retention

- [x] **IMP-DB-01** Add the active and published-retention partial indexes in an
  EF Core migration.
- [ ] **IMP-DB-02** Capture plans before and after the migration and document the
  selected active index ordering.
- [x] **IMP-RET-01** Add validated cleanup configuration with cleanup disabled.
- [x] **IMP-RET-02** Implement bounded set-based deletion without materializing
  entities.
- [x] **IMP-RET-03** Add cleanup telemetry, logging, cancellation, and concurrent
  worker behavior.
- [ ] **IMP-RET-04** Add PostgreSQL integration tests for AC-RET-01 through
  AC-RET-04.
- [x] **IMP-RET-05** Keep cleanup disabled until the operational enablement gate
  is signed off for each environment.

Requirements: DB-001 through DB-006 and RET-001 through RET-013.

## Phase 3: Concurrent-safe claiming

- [x] **IMP-CLM-01** Add the processing status, claim columns, expired-lease
  index, EF mapping, and migration.
- [x] **IMP-CLM-02** Add and validate lease and worker-identity configuration.
- [x] **IMP-CLM-03** Implement atomic ordered claiming with skip-locked behavior.
- [x] **IMP-CLM-04** Load and publish only records owned by the returned claim.
- [x] **IMP-CLM-05** Implement token-fenced success and failure completion.
- [x] **IMP-CLM-06** Implement expired-lease recovery, guarded lease renewal,
  bounded publish attempts, and stale-owner logging.
- [x] **IMP-CLM-07** Include processing rows in unresolved telemetry.
- [ ] **IMP-CLM-08** Add true concurrent PostgreSQL tests for AC-CLM-01 through
  AC-CLM-08.

Requirements: CLM-001 through CLM-027.

## Phase 4: Conditional throughput work

- [ ] **GATE-THR-01** Demonstrate at least one SPEC-OUTBOX-004 activation
  criterion and declare the target improvement before writing production code.
- [ ] **IMP-THR-01** Implement only the measured optimization: completion
  grouping, bounded publish concurrency, adaptive polling, or a justified
  combination.
- [ ] **IMP-THR-02** Run failure-injection and benchmark scenarios before
  enabling non-default settings.
- [ ] **IMP-THR-03** Document duplicate window, ordering, transport concurrency,
  and idle-latency consequences.

Requirements: THR-001 through THR-011 and IDL-001 through IDL-005.

## Requirement-to-verification matrix

| Requirement area | Required verification |
| --- | --- |
| TEL lifecycle/configuration | deterministic unit tests plus hosted-service integration test |
| TEL metric surface | contract test against `/metrics` |
| Active indexes | migration test plus recorded PostgreSQL explain plans |
| Retention predicates and bounds | PostgreSQL integration tests with parent/child rows |
| Concurrent cleanup | two independent PostgreSQL connections |
| Claim exclusivity and recovery | synchronized multi-connection PostgreSQL integration tests |
| Fenced completion | stale-token integration test asserting zero affected rows |
| No transaction over transport | blocking fake transport plus independent claim attempt |
| Throughput changes | predeclared production-like benchmark and failure injection |

## Definition of done for each phase

A phase is complete only when:

1. every applicable requirement has code or an explicit operational artifact;
2. every acceptance scenario passes or has attached operational evidence;
3. configuration defaults and validation are documented;
4. migrations have tested upgrade and downgrade behavior where supported;
5. logs and metrics avoid event payloads, errors are bounded, and identifiers
   remain safe for structured logging;
6. architecture and HTTP contracts remain unchanged; and
7. the complete repository gates pass:

```powershell
dotnet build dnd-compendium-service.slnx --no-restore
dotnet test tests/Compendium.ArchitectureTests/Compendium.ArchitectureTests.csproj --no-build
dotnet test dnd-compendium-service.slnx --no-build
dotnet ef migrations has-pending-model-changes --project src/Compendium.Infra/Compendium.Infra.csproj --startup-project src/Compendium.API/Compendium.API.csproj --no-build
```

## Review checklist

- [ ] Pull request identifies the specification and requirement IDs delivered.
- [ ] No implementation broadens scope beyond the selected phase.
- [ ] Query plans are evaluated using representative status distribution, not
  only an empty development database.
- [ ] Multi-instance metrics use `max` aggregation for the shared backlog.
- [ ] No transaction spans external transport I/O.
- [ ] No automatic dead-letter deletion exists.
- [ ] Retention remains disabled until environment-specific approval.
- [ ] At-least-once behavior and possible duplicate windows remain documented.
