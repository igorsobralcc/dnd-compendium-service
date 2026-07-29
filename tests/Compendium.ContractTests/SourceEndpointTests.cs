using System.Net;
using System.Net.Http.Json;
using System.Text.Json.Nodes;
using Compendium.CrossCutting.Security;
using Compendium.Domain.Sources;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace Compendium.ContractTests;

public sealed class SourceEndpointTests : IClassFixture<CompendiumApiFactory>
{
    private readonly CompendiumApiFactory factory;
    private readonly HttpClient client;

    public SourceEndpointTests(CompendiumApiFactory factory)
    {
        this.factory = factory;
        client = factory.CreateAdministrativeClient();
    }

    [Fact]
    public void Source_routes_are_mvc_actions_with_attribute_authorization()
    {
        var writeActions = new HashSet<string>
        {
            "CreateRuleset",
            "UpdateRuleset",
            "CreateRuleSource",
            "ActivateRuleSource",
            "DeactivateRuleSource",
            "CreateSourceVersion",
            "MarkSourceVersionAsCurrent"
        };
        var sourceActionNames = new HashSet<string>(writeActions)
        {
            "GetRulesetByCode",
            "ListRuleSourcesByRuleset",
            "GetCurrentSourceVersion",
            "ListSourceVersions"
        };

        var endpoints = factory.Services.GetRequiredService<EndpointDataSource>()
            .Endpoints
            .Where(endpoint =>
                sourceActionNames.Contains(
                    endpoint.Metadata.GetMetadata<IEndpointNameMetadata>()?.EndpointName
                    ?? string.Empty))
            .ToArray();

        Assert.Equal(11, endpoints.Length);

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
    public async Task Source_openapi_operations_preserve_paths_verbs_and_names()
    {
        var expectedOperations = new Dictionary<(string Path, string Method), string>
        {
            [("/api/compendium/rulesets", "post")] = "CreateRuleset",
            [("/api/compendium/rulesets/{code}", "put")] = "UpdateRuleset",
            [("/api/compendium/rulesets/{code}", "get")] = "GetRulesetByCode",
            [("/api/compendium/rule-sources", "post")] = "CreateRuleSource",
            [("/api/compendium/rule-sources/{id}/activate", "post")] = "ActivateRuleSource",
            [("/api/compendium/rule-sources/{id}/deactivate", "post")] = "DeactivateRuleSource",
            [("/api/compendium/rulesets/{rulesetId}/rule-sources", "get")] = "ListRuleSourcesByRuleset",
            [("/api/compendium/source-versions", "post")] = "CreateSourceVersion",
            [("/api/compendium/rule-sources/{ruleSourceId}/source-versions/{versionId}/current", "post")] =
                "MarkSourceVersionAsCurrent",
            [("/api/compendium/rule-sources/{ruleSourceId}/source-versions/current", "get")] =
                "GetCurrentSourceVersion",
            [("/api/compendium/rule-sources/{ruleSourceId}/source-versions", "get")] =
                "ListSourceVersions"
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

    [Fact]
    public async Task Create_ability_returns_bad_request_for_invalid_code()
    {
        var response = await client.PostAsJsonAsync(
            "/api/compendium/abilities",
            new
            {
                RuleSourceId = Guid.NewGuid(),
                SourceVersionId = Guid.NewGuid(),
                Code = "strength score",
                Name = "Strength"
            });

        var body = await response.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Contains("compendium.fundamentals.ability-code.invalid", body);
    }

    [Fact]
    public async Task Create_ability_score_method_returns_bad_request_for_invalid_standard_array()
    {
        var response = await client.PostAsJsonAsync(
            "/api/compendium/ability-score-methods",
            new
            {
                RuleSourceId = Guid.NewGuid(),
                SourceVersionId = Guid.NewGuid(),
                Code = "STANDARD_ARRAY",
                Name = "Standard Array",
                Type = 1,
                Rules = Array.Empty<object>(),
                StandardValues = new[] { 15, 14, 13 },
                PointBuyCosts = Array.Empty<object>(),
                RollRule = (object?)null
            });

        var body = await response.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Contains("compendium.fundamentals.ability-score-method.standard-array-values.required", body);
    }

    [Fact]
    public async Task Create_class_returns_bad_request_for_invalid_code()
    {
        var response = await client.PostAsJsonAsync(
            "/api/compendium/classes",
            new
            {
                RuleSourceId = Guid.NewGuid(),
                SourceVersionId = Guid.NewGuid(),
                Code = "fighter class",
                Name = "Fighter",
                Description = "Master of martial combat.",
                CoreTraits = new
                {
                    HitDieId = Guid.NewGuid(),
                    ArmorTrainingCategoryId = (Guid?)null,
                    SkillChoiceCount = 2
                },
                PrimaryAbilityIds = new[] { Guid.NewGuid() },
                Levels = new[]
                {
                    new
                    {
                        Level = 1,
                        ProficiencyBonus = 2,
                        SpellSlots = Array.Empty<object>(),
                        ProficiencyGrantIds = Array.Empty<Guid>(),
                        WeaponMasteryCount = (int?)null
                    }
                }
            });

        var body = await response.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Contains("compendium.classes.class-code.invalid", body);
    }
}
