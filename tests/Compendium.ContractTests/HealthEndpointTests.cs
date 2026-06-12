using System.Net;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Compendium.ContractTests;

public sealed class HealthEndpointTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient client;

    public HealthEndpointTests(WebApplicationFactory<Program> factory)
    {
        client = factory.CreateClient();
    }

    [Fact]
    public async Task Health_endpoint_returns_healthy_status()
    {
        var response = await client.GetAsync("/health");

        var body = await response.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("Healthy", body);
    }

    [Fact]
    public async Task Metadata_endpoint_exposes_service_contract_basics()
    {
        var response = await client.GetAsync("/internal/compendium/metadata");

        var body = await response.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Contains("\"service\":\"dnd-compendium-service\"", body);
        Assert.Contains("\"databaseSchema\":\"compendium\"", body);
        Assert.Contains("\"apiVersion\":\"v1\"", body);
    }
}
