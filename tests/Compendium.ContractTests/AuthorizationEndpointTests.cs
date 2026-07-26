using System.Net;
using System.Net.Http.Json;

namespace Compendium.ContractTests;

public sealed class AuthorizationEndpointTests : IClassFixture<CompendiumApiFactory>
{
    private readonly CompendiumApiFactory factory;

    public AuthorizationEndpointTests(CompendiumApiFactory factory) => this.factory = factory;

    [Fact]
    public async Task Administrative_write_without_credentials_returns_standardized_401()
    {
        var response = await factory.CreateClient().PostAsJsonAsync(
            "/api/compendium/rulesets",
            new { Code = "SRD_5_2_1", Name = "SRD", Version = "5.2.1", Status = 1 });
        var body = await response.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
        Assert.Contains("compendium.security.unauthenticated", body);
    }

    [Fact]
    public async Task Internal_service_cannot_execute_administrative_write()
    {
        var response = await factory.CreateInternalServiceClient().PostAsJsonAsync(
            "/api/compendium/rulesets",
            new { Code = "SRD_5_2_1", Name = "SRD", Version = "5.2.1", Status = 1 });
        var body = await response.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
        Assert.Contains("compendium.security.forbidden", body);
    }

    [Fact]
    public async Task Internal_read_without_credentials_returns_401()
    {
        var response = await factory.CreateClient().GetAsync(
            $"/internal/compendium/entities/feature/{Guid.CreateVersion7()}/mechanics");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Public_read_remains_anonymous()
    {
        var response = await factory.CreateClient().GetAsync("/api/compendium/rulesets/UNKNOWN");

        Assert.NotEqual(HttpStatusCode.Unauthorized, response.StatusCode);
        Assert.NotEqual(HttpStatusCode.Forbidden, response.StatusCode);
    }
}
