# syntax=docker/dockerfile:1.7

FROM mcr.microsoft.com/dotnet/sdk:10.0-noble AS build
WORKDIR /src

COPY ["src/Compendium.API/Compendium.API.csproj", "src/Compendium.API/"]
COPY ["src/Compendium.Application/Compendium.Application.csproj", "src/Compendium.Application/"]
COPY ["src/Compendium.Domain/Compendium.Domain.csproj", "src/Compendium.Domain/"]
COPY ["src/Compendium.Infra/Compendium.Infra.csproj", "src/Compendium.Infra/"]

RUN --mount=type=cache,id=compendium-nuget,target=/root/.nuget/packages \
    dotnet restore "src/Compendium.API/Compendium.API.csproj" \
    --runtime linux-x64

COPY . .

RUN --mount=type=cache,id=compendium-nuget,target=/root/.nuget/packages \
    dotnet publish "src/Compendium.API/Compendium.API.csproj" \
    --configuration Release \
    --output /app/publish \
    --runtime linux-x64 \
    --self-contained false \
    --no-restore \
    /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0-noble AS final

USER root
RUN apt-get update \
    && apt-get install --yes --no-install-recommends curl \
    && rm -rf /var/lib/apt/lists/*

WORKDIR /app
COPY --from=build --chown=app:app /app/publish .

ENV ASPNETCORE_ENVIRONMENT=Production \
    ASPNETCORE_HTTP_PORTS=8080 \
    DOTNET_EnableDiagnostics=0

EXPOSE 8080

HEALTHCHECK --interval=30s --timeout=5s --start-period=15s --retries=3 \
    CMD curl --fail --silent --show-error http://127.0.0.1:8080/health || exit 1

USER app
ENTRYPOINT ["dotnet", "Compendium.API.dll"]
