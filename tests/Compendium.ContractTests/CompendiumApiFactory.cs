using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;

namespace Compendium.ContractTests;

public class CompendiumApiFactory : WebApplicationFactory<Program>
{
    public const string AdministrativeApiKey = "contract-test-administrator-key";
    public const string InternalServiceApiKey = "contract-test-internal-service-key";

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration((_, configuration) =>
            configuration.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Compendium:Security:AdministrativeApiKey"] = AdministrativeApiKey,
                ["Compendium:Security:InternalServiceApiKey"] = InternalServiceApiKey
            }));
        ConfigureTestServices(builder);
    }

    protected virtual void ConfigureTestServices(IWebHostBuilder builder)
    {
    }

    public HttpClient CreateAdministrativeClient()
    {
        var client = CreateClient();
        client.DefaultRequestHeaders.Add("X-API-Key", AdministrativeApiKey);
        return client;
    }

    public HttpClient CreateInternalServiceClient()
    {
        var client = CreateClient();
        client.DefaultRequestHeaders.Add("X-API-Key", InternalServiceApiKey);
        return client;
    }
}
