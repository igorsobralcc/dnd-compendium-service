# SPEC-025: Feature MVC controllers

- Status: Done
- Commit: `45518226f0b75be16a83f0f5d680edbc37ee901f`
- Completed: 2026-07-31

## Intent

Move feature mechanics HTTP operations to focused MVC controllers without
altering consumer-visible behavior.

## Implemented requirements

- **MVC-FEAT-001**: Features, choice sets, effect schemas, and entity
  prerequisites MUST be exposed by dedicated controllers.
- **MVC-FEAT-002**: Controller actions MUST preserve route templates, verbs,
  request and response shapes, endpoint names, status semantics, and policies.
- **MVC-FEAT-003**: The legacy feature mapper MUST be removed from startup.

## Acceptance criteria

- Feature-mechanics routes match the locked contract matrix.
- Feature endpoint tests pass against MVC actions.
- Controller discovery produces no duplicate migrated routes.

## Evidence

- `src/Compendium.API/Features/FeaturesController.cs`
- `src/Compendium.API/Features/ChoiceSetsController.cs`
- `src/Compendium.API/Features/EffectSchemasController.cs`
- `tests/Compendium.ContractTests/FeatureEndpointTests.cs`
