# SPEC-035: MVC ports-and-adapters documentation

- Status: Done
- Commit: `e7145ec8bff1edd53acedb49d656fba4021afff9`
- Completed: 2026-08-01

## Intent

Document the service's pragmatic hexagonal architecture and the decision to use
MVC controllers as inbound HTTP adapters.

## Implemented requirements

- **ARCH-DOC-001**: Architecture documentation MUST describe domain,
  application, inbound adapter, outbound adapter, and composition boundaries.
- **ARCH-DOC-002**: The request flow from controller through use case and port
  to persistence adapter MUST be explicit.
- **ARCH-DOC-003**: Dependency rules, allowed framework usage, test strategy,
  and placement guidance for new code MUST be recorded.
- **ARCH-DOC-004**: An ADR MUST capture context, decision, consequences, and
  rejected alternatives for the pragmatic MVC ports-and-adapters approach.

## Acceptance criteria

- A contributor can determine where new domain behavior, use cases, controllers,
  repositories, and composition code belong.
- The ADR and architecture guide agree with executable architecture tests.

## Evidence

- `docs/architecture.md`
- `docs/adr/0001-pragmatic-hexagonal-architecture.md`
