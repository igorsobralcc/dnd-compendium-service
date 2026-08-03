# SPEC-018: Containerization and operator documentation

- Status: Done
- Commit: `31edeb670c0563da3a090be1caad1f8634ca7304`
- Completed: 2026-07-29

## Intent

Make the service buildable as a production container and document the image
design and runtime usage before the architecture refactor.

## Implemented requirements

- **DOC-001**: A multi-stage Dockerfile MUST restore, publish, and run the API
  from an ASP.NET runtime image as a non-root user.
- **DOC-002**: The Docker build context MUST exclude source-control, IDE, test,
  output, and local-secret artifacts.
- **DOC-003**: The README MUST document container build and run workflows and
  the service's operational configuration.
- **DOC-004**: Dockerfile design, caching, security, health-check, and maintenance
  decisions MUST be recorded in dedicated guidance.

## Acceptance criteria

- The container image can be produced from the repository root and starts the
  API with externally supplied configuration.
- Build-only and local files are absent from the runtime image context.

## Evidence

- `Dockerfile`
- `.dockerignore`
- `docs/dockerfile-guidelines.md`
- `README.md`
