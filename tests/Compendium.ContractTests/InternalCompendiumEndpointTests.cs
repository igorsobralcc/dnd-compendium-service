using System.Diagnostics;
using System.Net;
using System.Net.Http.Json;
using Compendium.Application.InternalQueries;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Compendium.ContractTests;

public sealed class InternalCompendiumEndpointTests : IClassFixture<InternalCompendiumApiFactory>
{
    private readonly HttpClient client;

    public InternalCompendiumEndpointTests(InternalCompendiumApiFactory factory) =>
        client = factory.CreateInternalServiceClient();

    [Fact]
    public async Task Character_creation_options_are_versioned_localized_and_fast()
    {
        var stopwatch = Stopwatch.StartNew();
        var response = await client.GetAsync(
            $"/internal/compendium/character-creation-options?ruleset_id={Guid.CreateVersion7()}&source_version_id={Guid.CreateVersion7()}&locale=pt-BR&level=1");
        stopwatch.Stop();
        var body = await response.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Contains("\"apiVersion\":\"v1\"", body);
        Assert.Contains("\"locale\":\"pt-BR\"", body);
        Assert.Contains("\"classes\"", body);
        Assert.True(stopwatch.Elapsed < TimeSpan.FromSeconds(2));
        Assert.True(response.Headers.Contains("X-Correlation-ID"));
    }

    [Fact]
    public async Task Mechanical_details_contract_is_typed_and_versioned()
    {
        var response = await client.GetAsync(
            $"/internal/compendium/entities/feature/{Guid.CreateVersion7()}/mechanics?locale=en-US");
        var body = await response.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Contains("\"entityType\":\"feature\"", body);
        Assert.Contains("\"effects\":[]", body);
        Assert.DoesNotContain("\"payload\"", body);
    }

    [Fact]
    public async Task Changes_contract_is_paginated_and_revision_based()
    {
        var response = await client.GetAsync("/internal/compendium/changes?revision=10&page=1&page_size=25");
        var result = await response.Content.ReadFromJsonAsync<CompendiumChangesV1>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(result);
        Assert.Equal("v1", result.ApiVersion);
        Assert.Equal(1, result.Page);
        Assert.Equal(25, result.PageSize);
    }
}

public sealed class InternalCompendiumApiFactory : CompendiumApiFactory
{
    protected override void ConfigureTestServices(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            services.RemoveAll<IInternalCompendiumQueryGateway>();
            services.AddSingleton<IInternalCompendiumQueryGateway, FakeInternalCompendiumQueryGateway>();
        });
    }
}

file sealed class FakeInternalCompendiumQueryGateway : IInternalCompendiumQueryGateway
{
    public Task<CharacterCreationOptionsV1> GetCharacterCreationOptionsAsync(
        CharacterCreationOptionsRequest request,
        CancellationToken cancellationToken) =>
        Task.FromResult(new CharacterCreationOptionsV1(
            "v1", request.RulesetId, request.SourceVersionId, request.Locale, request.Level,
            [new(Guid.CreateVersion7(), "FIGHTER", "Guerreiro")], [], [], [], [], [], [], []));

    public Task<MechanicalEntityDetailsV1?> GetMechanicalEntityDetailsAsync(
        string entityType,
        Guid entityId,
        string locale,
        CancellationToken cancellationToken) =>
        Task.FromResult<MechanicalEntityDetailsV1?>(new(
            "v1", "feature", entityId, "EXTRA_ATTACK", "Extra Attack", null,
            new(Guid.CreateVersion7(), Guid.CreateVersion7()),
            null, new(5, []), null, null, [], []));

    public Task<CompendiumChangesV1> ListChangesAsync(
        CompendiumChangesRequest request,
        CancellationToken cancellationToken) =>
        Task.FromResult(new CompendiumChangesV1(
            "v1", [], request.Page, request.PageSize, 0, null));
}
