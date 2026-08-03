# SPEC-037: Swagger descriptions and examples

- Status: Done
- Commit: `458809a0bfd0eeeaf93822c12a148a9260841878`
- Completed: 2026-08-01

## Intent

Produce a self-explanatory OpenAPI document with consistent operation,
parameter, schema, property, response, authorization, and example metadata.

## Implemented requirements

- **OAS-001**: Every controller operation MUST receive a summary and description
  derived from its action and route, including required authorization policies.
- **OAS-002**: Parameters and schema properties MUST receive human-readable
  descriptions and representative examples or allowed values.
- **OAS-003**: Response entries MUST explain the represented outcome.
- **OAS-004**: Swagger filters MUST be registered in API startup and operate
  without changing runtime HTTP contracts.
- **OAS-005**: Contract tests MUST validate documentation completeness across
  the generated Swagger document.

## Acceptance criteria

- `/swagger/v1/swagger.json` generates successfully.
- Operations, parameters, request/response schemas, and properties meet the
  documented description and example rules.

## Evidence

- `src/Compendium.API/OpenApi/CompendiumSwaggerDocumentation.cs`
- `src/Compendium.API/Program.cs`
- `tests/Compendium.ContractTests/SwaggerDocumentationTests.cs`
