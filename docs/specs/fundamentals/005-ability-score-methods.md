# SPEC-005: Ability score methods

- Status: Done
- Commit: `b806b32d2bf7fb9429998f3faabc246d054d34e0`
- Completed: 2026-06-11

## Intent

Represent configurable ability-score generation methods, including standard
arrays, point-buy costs, and dice-roll rules.

## Implemented requirements

- **ASM-001**: An ability-score method MUST own its ordered standard values,
  point-buy costs, and roll rules while enforcing method-specific invariants.
- **ASM-002**: Application use cases and contracts MUST create, update, list,
  and retrieve methods through a repository port.
- **ASM-003**: Fundamental HTTP APIs MUST expose the method operations.
- **ASM-004**: Persistence MUST map child rules and values with stable ordering
  and relational ownership, backed by an EF Core migration.

## Acceptance criteria

- Supported method configurations round-trip through the domain and database.
- Invalid score ranges, duplicate values, or inconsistent method rules are
  rejected and covered by unit tests.

## Evidence

- `src/Compendium.Domain/Fundamentals/AbilityScoreMethod.cs`
- `src/Compendium.Application/Fundamentals/AbilityScoreMethodUseCases.cs`
- `src/Compendium.Infra/Persistence/Fundamentals/AbilityScoreMethodConfiguration.cs`
- `tests/Compendium.UnitTests/Fundamentals/FundamentalRuleTests.cs`
