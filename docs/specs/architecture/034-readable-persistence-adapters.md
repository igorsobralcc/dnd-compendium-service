# SPEC-034: Readable persistence adapters

- Status: Done
- Commit: `188b692ca1a259cf8e5a2f52db654d3ce71c94a7`
- Completed: 2026-08-01

## Intent

Make equipment and feature EF Core adapters easier to inspect and maintain
without changing the persisted model or repository behavior.

## Implemented requirements

- **READ-INF-001**: Entity configurations MUST present property mappings,
  conversions, keys, indexes, and relationships in a consistent readable form.
- **READ-INF-002**: Repository queries and writes MUST clearly expose includes,
  filters, ordering, tracking, and save boundaries.
- **READ-INF-003**: Existing table names, columns, constraints, conversions,
  query semantics, and repository contracts MUST remain compatible.
- **READ-INF-004**: A formatting-only adapter refactor MUST NOT add a migration.

## Acceptance criteria

- EF reports no model change requiring a migration.
- Existing equipment, feature, integration, and architecture tests retain their
  prior expectations.

## Evidence

- `src/Compendium.Infra/Persistence/Equipment/EquipmentConfigurations.cs`
- `src/Compendium.Infra/Persistence/Equipment/EquipmentRepositories.cs`
- `src/Compendium.Infra/Persistence/Features/FeatureConfigurations.cs`
- `src/Compendium.Infra/Persistence/Features/FeatureRepositories.cs`
