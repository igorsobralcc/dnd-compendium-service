# Service Dockerfile Standard

This document records the decisions adopted in `dnd-compendium-service` and should be used as a
reference for future .NET services.

## Image and Platform

- Use official Microsoft images from `mcr.microsoft.com/dotnet`.
- Use the same major .NET version declared in the project.
- Use the Ubuntu `noble` variant.
- Build explicitly for `linux/amd64`.
- Use an SDK image only in the build stage and an ASP.NET Runtime image in the final stage.
- Do not include tests in the Dockerfile. The pipeline must run them before building/publishing the
  image.

## Build

- Keep the build context at the repository root.
- Copy the project files first and run `dotnet restore` to take advantage of layer caching.
- Use the BuildKit NuGet cache during `restore` and `publish`.
- Publish in `Release`, as `framework-dependent`, for `linux-x64`.
- Maintain a `.dockerignore` with an allowlist to avoid sending unnecessary source files and
  artifacts to the daemon.

Example:

```bash
docker buildx build \
  --platform linux/amd64 \
  --tag service-name:local \
  --load \
  .
```

The pipeline must enable BuildKit or use `docker buildx`.

## Runtime and Security

- Run the application as a non-root user.
- Use internal port `8080`.
- Set `ASPNETCORE_ENVIRONMENT=Production` as the default.
- Do not copy the SDK, source code, tests, IDE settings, or Git repository into the final image.
- Do not write connection strings, API keys, or other secrets in the Dockerfile.
- Provide secrets through protected pipeline variables or the platform's secrets mechanism.
- Do not use build arguments for secrets because they may remain in the image history.
- Pin the image's major version line (`10.0`) and rebuild regularly to incorporate patches.

## Operations

- Declare a `HEALTHCHECK` against the service's liveness endpoint.
- Use the application's readiness endpoint on platforms that support separate probes.
- Apply migrations manually, outside the API startup process.
- Provide the connection string through `ConnectionStrings__ConnectionName`.
- Publish the host port only at runtime; it must not be fixed in the Dockerfile.

## Recommended Pipeline

The expected order is:

1. Restore dependencies.
2. Compile.
3. Run tests and coverage validations.
4. Build the image for `linux/amd64`.
5. Scan the image with the security scanner adopted by the team.
6. Publish the image to the registry with an immutable tag, such as the commit SHA.
7. Apply migrations through a manual/controlled step.
8. Deploy the image, supplying configuration and secrets at runtime.
9. Verify the `/health` and `/health/ready` endpoints.

Environment tags, such as `staging` or `latest`, may be added, but they must not replace the
immutable tag used for rollback and auditing.
