# SPEC-014: Integration events and transactional messaging

- Status: Done
- Commit: `c5e34d42b8b1ff00c9bc8918e681fd95afd83bd8`
- Completed: 2026-07-26

## Intent

Deliver integration events reliably with a transactional Outbox and consume
them idempotently through an Inbox.

## Implemented requirements

- **MSG-001**: Application-facing publisher and consumer ports MUST exchange a
  versioned IntegrationEventEnvelope with typed fields and correlation data.
- **MSG-002**: Domain/application changes that publish events MUST save the
  affected data and Outbox record in one database transaction.
- **MSG-003**: A hosted dispatcher MUST poll eligible Pending or Failed rows in
  configured batches, publish them through an event transport, and mark each
  Published, Failed, or DeadLetter using bounded retries and delay.
- **MSG-004**: The Inbox consumer MUST use event ID plus consumer name to avoid
  duplicate processing and persist processing outcomes.
- **MSG-005**: Poll interval, batch size, retry count, and retry delay MUST be
  configuration-driven.

## Acceptance criteria

- Successful writes enqueue an event and successful dispatch marks it published.
- Transport failures schedule retry or dead-letter the row at the configured
  limit.
- A duplicate Inbox delivery does not run the handler again.

## Evidence

- `src/Compendium.Application/Contracts/Events/IntegrationEventContracts.cs`
- `src/Compendium.Infra/Integration/`
- `src/Compendium.Infra/Persistence/Integration/`
- `tests/Compendium.IntegrationTests/Integration/IntegrationMessagingTests.cs`
