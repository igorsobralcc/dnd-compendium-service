# ADR 0001: Pragmatic Hexagonal Architecture with MVC

- Status: Accepted
- Date: 2026-07-28

## Context

The service originally registered application behavior through many Minimal
API handlers. HTTP conversion, authorization decisions, dependency wiring, and
resource ownership were distributed across endpoint files. Application code
also needed durable boundaries that prevented ASP.NET Core and EF Core concerns
from moving into the business core.

The service needs explicit dependency direction without adding indirection that
does not serve a real boundary.

## Decision

Use a pragmatic Hexagonal Architecture:

- Domain contains business rules and has no framework dependencies.
- Application contains use cases and outbound ports and references only Domain.
- Infrastructure implements Application ports and owns persistence technology.
- API is an inbound MVC adapter and exposes one controller per entity or query
  resource.
- CrossCutting is the composition boundary and owns shared host concerns.
- ASP.NET Core MVC is the Front Controller for every application HTTP route.
- Application Minimal API handlers are prohibited. Framework-owned health and
  metrics mappings are allowed as technical host endpoints.
- Ports, interfaces, and base classes are introduced only for concrete
  boundaries or multiple meaningful implementations, not to satisfy a diagram.

The rules are executable in `tests/Compendium.ArchitectureTests` and the HTTP
surface is locked by `tests/Compendium.ContractTests`.

## Consequences

Positive consequences:

- Domain and Application remain independent of web and persistence frameworks.
- HTTP resource ownership and authorization metadata are discoverable from
  controllers.
- Infrastructure can change without moving persistence concerns into use cases.
- Architectural regressions fail in CI rather than relying on code review.

Trade-offs:

- CrossCutting intentionally references both Application and Infrastructure to
  compose the runtime graph.
- A controller-per-resource convention creates more files, but each file has a
  narrow and predictable responsibility.
- Some DTOs stay close to API controllers to avoid coupling transport contracts
  to the Application layer.
- Technical endpoints use framework mapping extensions, so `MapControllers` is
  the sole application routing mechanism rather than the sole endpoint mapping
  call in `Program.cs`.

## Alternatives considered

### Keep Minimal APIs

Rejected because the previous handlers duplicated HTTP conversion and made
resource ownership and authorization conventions harder to enforce uniformly.

### Add a mediator and generic controller hierarchy

Rejected for now. The current use cases are already explicit application entry
points, and generic abstractions would hide domain language without providing
multiple implementations or a required boundary.

### Let API reference Infrastructure directly

Rejected because it would allow controllers to bypass use cases and couple HTTP
behavior to EF Core or repository details.

## Operational guidance

Follow the checklist and diagrams in the
[architecture guide](../architecture.md). Any deliberate exception must update
this ADR, the architecture tests, and the HTTP contract tests in the same
change.
