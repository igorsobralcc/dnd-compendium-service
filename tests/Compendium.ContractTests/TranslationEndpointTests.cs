using System.Net;
using System.Net.Http.Json;

namespace Compendium.ContractTests;

public sealed class TranslationEndpointTests : IClassFixture<CompendiumApiFactory>
{
    private readonly HttpClient client;
    public TranslationEndpointTests(CompendiumApiFactory factory) => client = factory.CreateAdministrativeClient();

    [Fact]
    public async Task Upsert_translation_returns_bad_request_for_invalid_locale()
    {
        var response = await client.PutAsJsonAsync(
            $"/api/compendium/entities/feature/{Guid.CreateVersion7()}/translations/portuguese/name",
            new { Text = "Ataque Extra" });

        var body = await response.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Contains("translation.invalid_locale", body);
    }
}
