# SPEC-029: Internal query MVC controllers

- Status: Done
- Commit: `513ead824223bf58ccd509d0c413a8d543954419`
- Completed: 2026-07-31

## Intent

Move internal compendium read resources to MVC while preserving versioned
downstream contracts and internal authorization.

## Implemented requirements

- **MVC-IQ-001**: Character-creation options, mechanical entity details, and
  compendium changes MUST be exposed by focused controllers.
- **MVC-IQ-002**: Controllers MUST retain routes, verbs, parameters, V1 response
  shapes, status behavior, operation names, and internal-read policy metadata.
- **MVC-IQ-003**: The legacy internal-query endpoint mapper MUST be deleted and
  removed from startup.

## Acceptance criteria

- Internal routes remain identical to the locked contract matrix.
- Authorized callers receive the same versioned read models.
- Anonymous callers remain unable to access internal resources.

## Evidence

- `src/Compendium.API/InternalQueries/CharacterCreationOptionsController.cs`
- `src/Compendium.API/InternalQueries/MechanicalEntitiesController.cs`
- `src/Compendium.API/InternalQueries/CompendiumChangesController.cs`
