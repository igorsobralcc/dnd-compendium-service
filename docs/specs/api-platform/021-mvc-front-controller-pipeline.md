# SPEC-021: MVC front-controller pipeline

- Status: Done
- Commit: `8100eebbf9da41190f2c79990d2557a99eb576d5`
- Completed: 2026-07-29

## Intent

Establish ASP.NET Core MVC as the API front controller while centralizing error
handling, security metadata, and request-pipeline composition.

## Implemented requirements

- **MVC-001**: API startup MUST register and map controllers through the
  cross-cutting composition layer.
- **MVC-002**: `CompendiumControllerBase` MUST provide consistent conversion of
  application results to HTTP responses.
- **MVC-003**: An exception handler MUST translate unhandled failures into the
  service error response without leaking implementation details.
- **MVC-004**: Internal-read and administrative-write requirements MUST be
  expressible as controller/action attributes.
- **MVC-005**: Correlation and request observability MUST remain active in the
  restructured pipeline.

## Acceptance criteria

- Controller discovery, error mapping, authorization, and observability are
  active after startup composition.
- The locked HTTP matrix is unchanged by the pipeline foundation.

## Evidence

- `src/Compendium.API/Controllers/CompendiumControllerBase.cs`
- `src/Compendium.CrossCutting/Http/CompendiumPipelineExtensions.cs`
- `src/Compendium.CrossCutting/Http/CompendiumExceptionHandler.cs`
- `tests/Compendium.ContractTests/FrontControllerPipelineTests.cs`
