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

    [Fact]
    public async Task Create_species_returns_bad_request_for_invalid_code()
    {
        var response = await client.PostAsJsonAsync(
            "/api/compendium/species",
            new
            {
                RuleSourceId = Guid.NewGuid(),
                SourceVersionId = Guid.NewGuid(),
                Code = "high elf",
                Name = "High Elf",
                Description = (string?)null
            });

        var body = await response.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Contains("compendium.origins.species-code.invalid", body);
    }

    [Fact]
    public async Task Create_feat_returns_bad_request_for_unknown_category()
    {
        var response = await client.PostAsJsonAsync(
            "/api/compendium/feats",
            new
            {
                RuleSourceId = Guid.NewGuid(),
                SourceVersionId = Guid.NewGuid(),
                Code = "ALERT",
                Name = "Alert",
                Description = (string?)null,
                Category = 999,
                Repeatable = false
            });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}
