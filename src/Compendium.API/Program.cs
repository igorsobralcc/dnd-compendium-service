using Compendium.API.Classes;
using Compendium.API.Features;
using Compendium.API.Equipment;
using Compendium.API.Fundamentals;
using Compendium.API.Sources;
using Compendium.Application;
using Compendium.Infra;
using Compendium.Infra.Persistence;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services
    .AddHealthChecks()
    .AddCheck(
        "self",
        () => HealthCheckResult.Healthy("Compendium service is running."),
        tags: ["live", "ready"]);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("/", () => Results.Ok(new ServiceStatusResponse("dnd-compendium-service", "running")))
    .WithName("GetServiceStatus");

app.MapGet(
        "/internal/compendium/metadata",
        () => Results.Ok(new CompendiumMetadataResponse(
            "dnd-compendium-service",
            CompendiumDbContext.Schema,
            "v1")))
    .WithName("GetCompendiumMetadata");

app.MapSourceEndpoints();
app.MapFundamentalEndpoints();
app.MapClassEndpoints();
app.MapFeatureEndpoints();
app.MapEquipmentEndpoints();

app.MapHealthChecks(
    "/health",
    new HealthCheckOptions
    {
        Predicate = registration => registration.Tags.Contains("live")
    });

app.MapHealthChecks(
    "/health/ready",
    new HealthCheckOptions
    {
        Predicate = registration => registration.Tags.Contains("ready")
    });

app.Run();

public partial class Program
{
}

internal sealed record ServiceStatusResponse(string Service, string Status);

internal sealed record CompendiumMetadataResponse(string Service, string DatabaseSchema, string ApiVersion);
