# SPEC-001: Initialize repository

- Status: Done
- Commit: `2726e1ecfcc07efb08318f058cdf313ca3b4b6c3`
- Completed: 2026-06-11

## Intent

Establish the repository identity and a safe baseline for tracked files.

## Implemented requirements

- **INIT-001**: The repository MUST identify itself as `dnd-compendium-service`.
- **INIT-002**: Common generated .NET, IDE, build, test, and user-specific files
  MUST be excluded from source control.

## Acceptance criteria

- The root README names the service.
- A clean build or IDE session does not require generated artifacts to be
  committed.

## Evidence

- `README.md`
- `.gitignore`
