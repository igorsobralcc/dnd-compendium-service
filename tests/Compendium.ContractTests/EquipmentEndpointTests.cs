using System.Text.Json.Nodes;
using Compendium.CrossCutting.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace Compendium.ContractTests;

public sealed class EquipmentEndpointTests : IClassFixture<CompendiumApiFactory>
{
    private readonly CompendiumApiFactory factory;
    private readonly HttpClient client;

    public EquipmentEndpointTests(CompendiumApiFactory factory)
    {
        this.factory = factory;
        client = factory.CreateAdministrativeClient();
    }

    [Fact]
    public void Equipment_routes_are_owned_by_eight_entity_controllers()
    {
        var expectedCounts = new Dictionary<string, int>
        {
            ["EquipmentItems"] = 4,
            ["Weapons"] = 4,
            ["WeaponProperties"] = 1,
            ["WeaponMasteries"] = 1,
            ["Armors"] = 4,
            ["Tools"] = 1,
            ["EquipmentPacks"] = 1,
            ["StartingEquipmentRules"] = 2
        };
        var endpoints = factory.Services.GetRequiredService<EndpointDataSource>()
            .Endpoints
            .Where(endpoint => endpoint.Metadata
                .GetMetadata<ControllerActionDescriptor>()
                ?.ControllerTypeInfo.Namespace == "Compendium.API.Equipment")
            .ToArray();

        Assert.Equal(18, endpoints.Length);

        foreach (var (controllerName, expectedCount) in expectedCounts)
        {
            Assert.Equal(
                expectedCount,
                endpoints.Count(endpoint => endpoint.Metadata
                    .GetRequiredMetadata<ControllerActionDescriptor>()
                    .ControllerName == controllerName));
        }

        foreach (var endpoint in endpoints)
        {
            var isWrite = endpoint.Metadata
                .GetRequiredMetadata<HttpMethodMetadata>()
                .HttpMethods
                .Any(method => method != HttpMethods.Get);
            var policies = endpoint.Metadata
                .GetOrderedMetadata<IAuthorizeData>()
                .Select(metadata => metadata.Policy)
                .ToArray();

            if (isWrite)
            {
                Assert.Contains(
                    CompendiumSecurity.AdministrativeWritePolicy,
                    policies);
            }
            else
            {
                Assert.DoesNotContain(
                    CompendiumSecurity.AdministrativeWritePolicy,
                    policies);
            }
        }
    }

    [Fact]
    public async Task Equipment_openapi_operations_preserve_paths_verbs_and_tag()
    {
        var expectedOperations = new (string Path, string Method)[]
        {
            ("/api/compendium/equipment", "post"),
            ("/api/compendium/equipment/{code}", "put"),
            ("/api/compendium/equipment", "get"),
            ("/api/compendium/equipment/{code}", "get"),
            ("/api/compendium/equipment/weapons/properties", "post"),
            ("/api/compendium/equipment/weapons/masteries", "post"),
            ("/api/compendium/equipment/weapons", "post"),
            ("/api/compendium/equipment/weapons/{equipmentItemId}/properties", "post"),
            ("/api/compendium/equipment/weapons", "get"),
            ("/api/compendium/equipment/weapons/{equipmentItemId}", "get"),
            ("/api/compendium/equipment/armors", "post"),
            ("/api/compendium/equipment/armors/{equipmentItemId}/ac-rule", "put"),
            ("/api/compendium/equipment/armors", "get"),
            ("/api/compendium/equipment/armors/{equipmentItemId}", "get"),
            ("/api/compendium/equipment/tools", "post"),
            ("/api/compendium/equipment/packs", "post"),
            ("/api/compendium/equipment/starting-rules", "post"),
            ("/api/compendium/equipment/starting-rules/{ownerType}/{ownerId}", "get")
        };

        var document = JsonNode.Parse(
            await client.GetStringAsync("/swagger/v1/swagger.json"));
        var paths = document!["paths"]!.AsObject();

        foreach (var (path, method) in expectedOperations)
        {
            var operation = paths[path]![method]!;
            Assert.Null(operation["operationId"]);
            Assert.Contains(
                operation["tags"]!.AsArray(),
                tag => tag!.GetValue<string>() == "Equipment");
        }
    }
}
