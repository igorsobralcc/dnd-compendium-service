using System.Text.Json.Nodes;
using Compendium.CrossCutting.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace Compendium.ContractTests;

public sealed class FeatureEndpointTests : IClassFixture<CompendiumApiFactory>
{
    private readonly CompendiumApiFactory factory;
    private readonly HttpClient client;

    public FeatureEndpointTests(CompendiumApiFactory factory)
    {
        this.factory = factory;
        client = factory.CreateAdministrativeClient();
    }

    [Fact]
    public void Feature_routes_are_owned_by_their_aggregate_controllers()
    {
        var writeActions = new HashSet<string>
        {
            "CreateFeature",
            "UpdateFeature",
            "AttachEffectToFeature",
            "CreateEffectSchema",
            "AddPrerequisiteToEntity",
            "RemovePrerequisiteFromEntity",
            "CreateChoiceSet",
            "AddChoiceOption",
            "AddChoiceFilter"
        };
        var actionNames = new HashSet<string>(writeActions)
        {
            "ListFeatures",
            "GetFeatureDetails",
            "GetFeatureEffects",
            "GetEntityPrerequisites",
            "GetChoiceSetDetails",
            "ListChoiceSetsBySourceEntity"
        };

        var endpoints = factory.Services.GetRequiredService<EndpointDataSource>()
            .Endpoints
            .Where(endpoint =>
                actionNames.Contains(
                    endpoint.Metadata.GetMetadata<IEndpointNameMetadata>()?.EndpointName
                    ?? string.Empty))
            .ToArray();

        Assert.Equal(15, endpoints.Length);
        AssertControllerActionCount(endpoints, "Features", 6);
        AssertControllerActionCount(endpoints, "EffectSchemas", 1);
        AssertControllerActionCount(endpoints, "EntityPrerequisites", 3);
        AssertControllerActionCount(endpoints, "ChoiceSets", 5);

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
    public async Task Feature_openapi_operations_preserve_paths_verbs_and_names()
    {
        var expectedOperations = new Dictionary<(string Path, string Method), string>
        {
            [("/api/compendium/features", "post")] = "CreateFeature",
            [("/api/compendium/features/{code}", "put")] = "UpdateFeature",
            [("/api/compendium/features", "get")] = "ListFeatures",
            [("/api/compendium/features/{code}", "get")] = "GetFeatureDetails",
            [("/api/compendium/effect-schemas", "post")] = "CreateEffectSchema",
            [("/api/compendium/features/{featureCode}/effects", "post")] =
                "AttachEffectToFeature",
            [("/api/compendium/features/{featureCode}/effects", "get")] =
                "GetFeatureEffects",
            [("/api/compendium/entity-prerequisites", "post")] =
                "AddPrerequisiteToEntity",
            [("/api/compendium/entity-prerequisites/{prerequisiteId}", "delete")] =
                "RemovePrerequisiteFromEntity",
            [("/api/compendium/entity-prerequisites/{entityKind}/{entityId}", "get")] =
                "GetEntityPrerequisites",
            [("/api/compendium/choice-sets", "post")] = "CreateChoiceSet",
            [("/api/compendium/choice-sets/{code}", "get")] =
                "GetChoiceSetDetails",
            [("/api/compendium/choice-sets/by-source/{entityKind}/{entityId}", "get")] =
                "ListChoiceSetsBySourceEntity",
            [("/api/compendium/choice-sets/{code}/options", "post")] =
                "AddChoiceOption",
            [("/api/compendium/choice-sets/{code}/filters", "post")] =
                "AddChoiceFilter"
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

    private static void AssertControllerActionCount(
        IEnumerable<Endpoint> endpoints,
        string controllerName,
        int expectedCount) =>
        Assert.Equal(
            expectedCount,
            endpoints.Count(endpoint => endpoint.Metadata
                .GetRequiredMetadata<ControllerActionDescriptor>()
                .ControllerName == controllerName));
}
