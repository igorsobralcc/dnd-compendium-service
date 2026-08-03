# Outbox implementation audit

- Audited: 2026-08-03
- Scope: `implementation-plan.md`
- Evidence source: repository code and automated tests

This audit distinguishes repository implementation from environment-specific
operational evidence. Production values and query plans are intentionally not
invented. Use [operational-verification.sql](operational-verification.sql) in a
sanitized production-like environment and attach the results to the deployment
review.

## Phase 0: Baseline

| Item | Result | Evidence or remaining action |
| --- | --- | --- |
| BASE-01 | Pending operational evidence | Record deployment replicas and Prometheus scrape interval in the deployment review. |
| BASE-02 | Pending operational evidence | Run sections 1 and 2 of the verification script over a representative statistics window. |
| BASE-03 | Pending operational evidence | Run section 3 and record daily volume outside the repository. |
| BASE-04 | Pending operational evidence | Run section 4 against sanitized production-like data. An empty developer database is not acceptable evidence. |

## Phase 1: Decoupled telemetry

All items IMP-TEL-01 through IMP-TEL-07 are implemented. The dispatcher has no
backlog count, `OutboxBacklogCollector` owns the count cadence, both gauges read
one cached value, startup validation is present, and unit/contract coverage
verifies the metric aliases. This phase remains complete.

## Phase 2: Indexes and retention

| Item | Result | Evidence or remaining action |
| --- | --- | --- |
| IMP-DB-01 | Implemented | `AddOutboxPerformanceIndexes` creates both partial indexes concurrently with migration transaction suppression. |
| IMP-DB-02 | Pending operational evidence | The candidate ordering matches DB-003; record before/after plans with section 4 of the verification script. |
| IMP-RET-01 | Implemented | Cleanup defaults to disabled and all settings are startup-validated. |
| IMP-RET-02 | Implemented | Cleanup uses bounded set-based `DELETE` batches with `FOR UPDATE SKIP LOCKED`. |
| IMP-RET-03 | Implemented | Counters, structured logs, cancellation, and short per-batch scopes are present. |
| IMP-RET-04 | Not complete | Model/configuration tests exist, but the required live PostgreSQL scenarios AC-RET-01 through AC-RET-04 have not been committed. They require an isolated PostgreSQL test database and must not run against the application database. |
| IMP-RET-05 | Implemented | Default and checked-in application configuration keep cleanup disabled. Environment enablement still requires the gate in SPEC-OUTBOX-002. |

## Phase 3: Concurrent-safe claiming

| Item | Result | Evidence or remaining action |
| --- | --- | --- |
| IMP-CLM-01 | Implemented | Processing state, nullable claim columns, mapping, migration, and partial lease index exist. |
| IMP-CLM-02 | Implemented | Lease, timeout, stable worker identity, defaults, and startup validation exist. |
| IMP-CLM-03 | Implemented | One CTE atomically orders, locks with `SKIP LOCKED`, updates, and returns claimed IDs. |
| IMP-CLM-04 | Implemented | Publication loads only IDs owned by the new claim token. |
| IMP-CLM-05 | Implemented | Claim token is an EF concurrency token; stale completion raises and is logged without overwriting the newer owner. |
| IMP-CLM-06 | Implemented after audit correction | Expired leases are reclaimed, counted, and logged; renewal and transport timeout are bounded. Before this audit the recovery counter was declared but never incremented. |
| IMP-CLM-07 | Implemented | Backlog collection includes `PROCESSING`. |
| IMP-CLM-08 | Not complete | Domain/model tests exist, but AC-CLM-01 through AC-CLM-08 still need synchronized, independent PostgreSQL connections in an isolated test database. |

## Phase 4: Conditional throughput work

GATE-THR-01 and IMP-THR-01 through IMP-THR-03 remain intentionally pending.
No activation measurements or predeclared improvement target exist in the
repository, so implementing adaptive polling, grouped completion, or concurrent
publishing would violate SPEC-OUTBOX-004. The two-second claim poll therefore
remains the authorized default.

## Audit conclusion

The repository implementation is complete for the authorized production code
except for the recovered-claim telemetry defect corrected by this audit. The
remaining plan items are evidence and live-PostgreSQL verification work, not
authorization to infer production measurements or enable Phase 4.
