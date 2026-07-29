using System.Text.Json.Nodes;
using Compendium.CrossCutting.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace Compendium.ContractTests;

public sealed class FundamentalEndpointTests : IClassFixture<CompendiumApiFactory>
{
    private readonly CompendiumApiFactory factory;
    private readonly HttpClient client;

    public FundamentalEndpointTests(CompendiumApiFactory factory)
    {
        this.factory = factory;
        client = factory.CreateAdministrativeClient();
    }

    [Fact]
    public void Fundamental_routes_are_mvc_actions_with_attribute_authorization()
    {
        var writeActions = new HashSet<string>
        {
            "CreateAbility",
            "UpdateAbility",
            "CreateSkill",
            "UpdateSkill",
            "CreateLanguage",
            "UpdateLanguage",
            "CreateProficiency",
            "UpdateProficiency",
            "CreateArmorTrainingCategory",
            "CreateHitDie",
            "CreateAbilityScoreMethod"
        };
        var actionNames = new HashSet<string>(writeActions)
        {
            "ListAbilities",
            "ListSkills",
            "ListLanguages",
            "ListProficiencies",
            "ListArmorTrainingCategories",
            "ListHitDice",
            "ListAbilityScoreMethods"
        };

        var endpoints = factory.Services.GetRequiredService<EndpointDataSource>()
            .Endpoints
            .Where(endpoint =>
                actionNames.Contains(
                    endpoint.Metadata.GetMetadata<IEndpointNameMetadata>()?.EndpointName
                    ?? string.Empty))
            .ToArray();

        Assert.Equal(18, endpoints.Length);

        foreach (var endpoint in endpoints)
        {
            Assert.NotNull(endpoint.Metadata.GetMetadata<ControllerActionDescriptor>());
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
    public async Task Fundamental_openapi_operations_preserve_contracts()
    {
        var expectedOperations = new Dictionary<(string Path, string Method), string>
        {
            [("/api/compendium/abilities", "post")] = "CreateAbility",
            [("/api/compendium/abilities/{code}", "put")] = "UpdateAbility",
            [("/api/compendium/abilities", "get")] = "ListAbilities",
            [("/api/compendium/skills", "post")] = "CreateSkill",
            [("/api/compendium/skills/{code}", "put")] = "UpdateSkill",
            [("/api/compendium/skills", "get")] = "ListSkills",
            [("/api/compendium/languages", "post")] = "CreateLanguage",
            [("/api/compendium/languages/{code}", "put")] = "UpdateLanguage",
            [("/api/compendium/languages", "get")] = "ListLanguages",
            [("/api/compendium/proficiencies", "post")] = "CreateProficiency",
            [("/api/compendium/proficiencies/{code}", "put")] = "UpdateProficiency",
            [("/api/compendium/proficiencies", "get")] = "ListProficiencies",
            [("/api/compendium/armor-training-categories", "post")] =
                "CreateArmorTrainingCategory",
            [("/api/compendium/armor-training-categories", "get")] =
                "ListArmorTrainingCategories",
            [("/api/compendium/hit-dice", "post")] = "CreateHitDie",
            [("/api/compendium/hit-dice", "get")] = "ListHitDice",
            [("/api/compendium/ability-score-methods", "post")] =
                "CreateAbilityScoreMethod",
            [("/api/compendium/ability-score-methods", "get")] =
                "ListAbilityScoreMethods"
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

        var proficiencyParameters = paths["/api/compendium/proficiencies"]!["get"]![
            "parameters"]!.AsArray();
        Assert.Contains(
            proficiencyParameters,
            parameter => parameter!["name"]!.GetValue<string>() == "type"
                && parameter["in"]!.GetValue<string>() == "query");
    }
}
