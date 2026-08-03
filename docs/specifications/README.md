# Development specifications

This directory contains normative, implementation-driving specifications for
changes that affect architecture, persistence, contracts, operations, or other
cross-cutting behavior.

## Spec-driven development rules

1. **Specify before coding.** A change starts with a `Proposed` specification
   containing scope, requirements, non-goals, acceptance scenarios, and
   verification expectations.
2. **Resolve material ambiguity.** Configuration defaults, failure behavior,
   compatibility, migrations, and rollout safety must be explicit before a
   specification becomes `Accepted`.
3. **Approve the contract.** Development begins only after the responsible
   reviewer changes the relevant specification to `Accepted`.
4. **Trace every change.** Pull requests and commits cite the specification and
   requirement IDs they implement. Tests cite the acceptance scenario IDs they
   verify when naming permits.
5. **Change the specification first.** If implementation reveals a changed
   requirement or scope, update and re-review the specification before changing
   production behavior.
6. **Deliver in slices.** A slice must leave the system deployable, preserve
   stated compatibility, and satisfy the applicable definition of done.
7. **Record evidence.** Nondeterministic performance assertions belong in
   reviewed query plans or benchmark artifacts, not brittle CI timing tests.

## Lifecycle

`Proposed` → `Accepted` → `In Progress` → `Implemented`

A replaced document becomes `Superseded` and links to its replacement. A
deferred specification must state the evidence or decision required to activate
it.

## Specification catalog

- [Outbox performance specification suite](outbox-performance/README.md) —
  decoupled telemetry, active indexes, retention, safe claiming, and conditional
  throughput improvements.
