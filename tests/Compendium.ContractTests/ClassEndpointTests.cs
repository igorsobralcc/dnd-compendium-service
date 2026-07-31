using System.Text.Json.Nodes;
using Compendium.CrossCutting.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace Compendium.ContractTests;

public sealed class ClassEndpointTests : IClassFixture<CompendiumApiFactory>
{
    private readonly CompendiumApiFactory factory;
    private readonly HttpClient client;

    public ClassEndpointTests(CompendiumApiFactory factory)
    {
        this.factory = factory;
        client = factory.CreateAdministrativeClient();
    }

    [Fact]
    public void Class_routes_are_split_between_class_and_subclass_controllers()
    {
        var writeActions = new HashSet<string>
        {
            "CreateClass",
            "UpdateClass",
            "ConfigureClassProgression",
            "CreateSubclass",
            "LinkSubclassFeature"
        };
        var actionNames = new HashSet<string>(writeActions)
        {
            "ListClasses",
            "GetClassDetails",
            "GetClassProgression",
            "ListSubclassesByClass",
            "GetSubclassDetails"
        };

        var endpoints = factory.Services.GetRequiredService<EndpointDataSource>()
            .Endpoints
            .Where(endpoint =>
                actionNames.Contains(
                    endpoint.Metadata.GetMetadata<IEndpointNameMetadata>()?.EndpointName
                    ?? string.Empty))
            .ToArray();

        Assert.Equal(10, endpoints.Length);
        Assert.Equal(
            6,
            endpoints.Count(endpoint => endpoint.Metadata
                .GetRequiredMetadata<ControllerActionDescriptor>()
                .ControllerName == "Classes"));
        Assert.Equal(
            4,
            endpoints.Count(endpoint => endpoint.Metadata
                .GetRequiredMetadata<ControllerActionDescriptor>()
                .ControllerName == "Subclasses"));

        foreach (var endpoint in endpoints)
        {
            var endpointName = endpoint.Metadata
                .GetRequiredMetadata<IEndpointNameMetadata>()
                .EndpointName!;
            var policies = endpoint.Metadata
                .GetOrderedMetadata<IAuthorizeData>()
                .Select(metadata => metadata.Policy)
                .ToArray();

            if (writeActions.Contains(endpointName))
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
    public async Task Class_openapi_operations_preserve_nested_routes_and_names()
    {
        var expectedOperations = new Dictionary<(string Path, string Method), string>
        {
            [("/api/compendium/classes", "post")] = "CreateClass",
            [("/api/compendium/classes/{code}", "put")] = "UpdateClass",
            [("/api/compendium/classes", "get")] = "ListClasses",
            [("/api/compendium/classes/{code}", "get")] = "GetClassDetails",
            [("/api/compendium/classes/{code}/progression", "put")] =
                "ConfigureClassProgression",
            [("/api/compendium/classes/{code}/progression", "get")] =
                "GetClassProgression",
            [("/api/compendium/classes/{classCode}/subclasses", "post")] =
                "CreateSubclass",
            [("/api/compendium/classes/{classCode}/subclasses", "get")] =
                "ListSubclassesByClass",
            [("/api/compendium/classes/{classCode}/subclasses/{subclassCode}", "get")] =
                "GetSubclassDetails",
            [("/api/compendium/classes/{classCode}/subclasses/{subclassCode}/features", "post")] =
                "LinkSubclassFeature"
        };

        var document = JsonNode.Parse(
            await client.GetStringAsync("/swagger/v1/swagger.json"));
        var paths = document!["paths"]!.AsObject();

        foreach (var ((path, method), operationName) in expectedOperations)
        {
            Assert.Equal(
                operationName,
                paths[path]![method]!["operationId"]!.GetValue<string>());
        }
    }
}
