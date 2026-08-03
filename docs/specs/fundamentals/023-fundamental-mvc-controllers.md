# SPEC-023: Fundamental MVC controllers

- Status: Done
- Commit: `dda744f3a2cb66c2dd9fd1823cd858350fd3f51b`
- Completed: 2026-07-29

## Intent

Replace the consolidated fundamentals minimal-endpoint module with focused MVC
controllers while preserving its HTTP surface.

## Implemented requirements

- **MVC-FUND-001**: Abilities, ability-score methods, armor-training categories,
  hit dice, languages, proficiencies, and skills MUST each have an appropriate
  controller.
- **MVC-FUND-002**: Controller actions MUST preserve existing routes, methods,
  bodies, query parameters, result mapping, names, and authorization metadata.
- **MVC-FUND-003**: The legacy fundamental endpoint mapper MUST be removed from
  startup.

## Acceptance criteria

- The locked matrix reports no fundamental route additions, removals, or method
  changes.
- Fundamental create and read contract tests pass through MVC.

## Evidence

- `src/Compendium.API/Fundamentals/`
- `tests/Compendium.ContractTests/FundamentalEndpointTests.cs`
- Deletion of `src/Compendium.API/Fundamentals/FundamentalEndpoints.cs`
