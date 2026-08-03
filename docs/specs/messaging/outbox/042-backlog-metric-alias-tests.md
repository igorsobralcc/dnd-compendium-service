# SPEC-042: Backlog metric alias contract coverage

- Status: Done
- Commit: `6390af1bdc9fd600f8e9f1f30399e18186530e2c`
- Completed: 2026-08-03

## Intent

Lock the Prometheus compatibility surface for both the legacy and replacement
Outbox backlog metric names.

## Implemented requirements

- **MET-CTR-001**: The metrics contract test MUST require the exported legacy
  name `compendium_outbox_pending`.
- **MET-CTR-002**: The same contract MUST require the replacement name
  `compendium_outbox_unresolved`.
- **MET-CTR-003**: Removing either alias from `/metrics` MUST fail the contract
  suite.

## Acceptance criteria

- The metrics endpoint contains both Prometheus-normalized gauge names.
- Existing health and metrics contract behavior remains unchanged otherwise.

## Evidence

- `tests/Compendium.ContractTests/HealthEndpointTests.cs`
