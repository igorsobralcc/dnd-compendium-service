using Compendium.API.Classes;
using Compendium.API.Features;
using Compendium.API.Equipment;
using Compendium.API.Translations;
using Compendium.API.Importing;
using Compendium.API.InternalQueries;
using Compendium.CrossCutting;
using Compendium.CrossCutting.Http;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCompendiumServices(builder.Configuration);
builder.Services
    .AddHealthChecks()
    .AddCheck(
        "self",
        () => HealthCheckResult.Healthy("Compendium service is running."),
        tags: ["live", "ready"]);

var app = builder.Build();

app.UseCompendiumPipeline();

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
            "compendium",
            "v1")))
    .WithName("GetCompendiumMetadata");

app.MapClassEndpoints();
app.MapFeatureEndpoints();
app.MapEquipmentEndpoints();
app.MapTranslationEndpoints();
app.MapImportEndpoints();
app.MapInternalCompendiumEndpoints();
app.MapControllers();
app.MapPrometheusScrapingEndpoint("/metrics");

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
