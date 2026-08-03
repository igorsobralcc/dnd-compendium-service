# SPEC-004: Fundamental rule data

- Status: Done
- Commit: `5597ffef493691d6f88c9caabdfb40cedef5c5df`
- Completed: 2026-06-11

## Intent

Add source-linked fundamental D&D catalog data used by higher-level compendium
entities.

## Implemented requirements

- **FUND-001**: The domain MUST model abilities, skills, languages,
  proficiencies, hit dice, and armor-training categories with canonical codes
  and validation.
- **FUND-002**: Application commands and queries MUST manage fundamental data
  through repository interfaces and verify source references.
- **FUND-003**: HTTP endpoints MUST expose the supported create and read
  operations without leaking persistence models.
- **FUND-004**: EF Core mappings and migrations MUST persist the entities and
  enforce relational and canonical uniqueness.

## Acceptance criteria

- Fundamental entities reject invalid values and retain their source linkage.
- The database migration creates every fundamental table and the HTTP contracts
  remain executable.

## Evidence

- `src/Compendium.Domain/Fundamentals/`
- `src/Compendium.Application/Fundamentals/`
- `src/Compendium.Infra/Persistence/Fundamentals/`
- `tests/Compendium.UnitTests/Fundamentals/FundamentalRuleTests.cs`
