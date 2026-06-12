using System.Net;
using System.Net.Http.Json;
using Compendium.Domain.Sources;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Compendium.ContractTests;

public sealed class SourceEndpointTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient client;

    public SourceEndpointTests(WebApplicationFactory<Program> factory)
    {
        client = factory.CreateClient();
    }

    [Fact]
    public async Task Create_ruleset_returns_bad_request_for_invalid_code()
    {
        var response = await client.PostAsJsonAsync(
            "/api/compendium/rulesets",
            new
            {
                Code = "SRD 5 2 1",
                Name = "SRD 5.2.1",
                Version = "5.2.1",
                Status = RulesetStatus.Active
            });

        var body = await response.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Contains("compendium.sources.ruleset-code.invalid", body);
    }
}
