# SPEC-016: Security and authorization

- Status: Done
- Commit: `69c01e962f649116e6577aa7be8720f560155a2c`
- Completed: 2026-07-26

## Intent

Protect administrative writes and internal reads with configurable bearer-token
authorization while retaining anonymous operational endpoints.

## Implemented requirements

- **SEC-001**: The API MUST authenticate configured bearer tokens and derive
  authorization claims without persisting or logging raw credentials.
- **SEC-002**: Administrative mutation routes MUST require the administrative
  write policy.
- **SEC-003**: Internal compendium query routes MUST require the internal read
  policy.
- **SEC-004**: Root service status and health probes MUST remain anonymously
  accessible.
- **SEC-005**: Security configuration MUST be environment-overridable and the
  test host MUST support deterministic authenticated requests.

## Acceptance criteria

- Missing or invalid credentials are rejected on protected endpoints.
- Properly authorized callers reach the same underlying HTTP contracts.
- Anonymous health and status requests continue to succeed.

## Evidence

- `src/Compendium.API/Security/CompendiumSecurity.cs`
- `src/Compendium.API/appsettings.json`
- `tests/Compendium.ContractTests/AuthorizationEndpointTests.cs`
- `tests/Compendium.ContractTests/CompendiumApiFactory.cs`
