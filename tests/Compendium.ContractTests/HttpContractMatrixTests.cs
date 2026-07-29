using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace Compendium.ContractTests;

public sealed class HttpContractMatrixTests : IClassFixture<CompendiumApiFactory>
{
    private readonly CompendiumApiFactory factory;

    public HttpContractMatrixTests(CompendiumApiFactory factory) => this.factory = factory;

    [Fact]
    public void Matrix_matches_every_application_route()
    {
        _ = factory.CreateClient();
        var applicationParts = factory.Services.GetRequiredService<ApplicationPartManager>();
        Assert.Contains(
            applicationParts.ApplicationParts,
            part => part.Name == "Compendium.API");
        var discovered = factory.Services.GetRequiredService<EndpointDataSource>().Endpoints
            .OfType<RouteEndpoint>()
            .SelectMany(endpoint => endpoint.Metadata.GetMetadata<HttpMethodMetadata>()?.HttpMethods
                .Select(method => new RouteIdentity(
                    method,
                    NormalizeRoute(endpoint.RoutePattern.RawText!),
                    endpoint.Metadata.GetMetadata<IEndpointNameMetadata>()?.EndpointName))
                ?? [])
            .Where(route => IsApplicationRoute(route.Route))
            .OrderBy(route => route.Method)
            .ThenBy(route => route.Route)
            .ToArray();

        var expected = HttpContractMatrix.Routes
            .Select(contract => new RouteIdentity(contract.Method, contract.Route, contract.Name))
            .OrderBy(route => route.Method)
            .ThenBy(route => route.Route)
            .ToArray();

        Assert.Equal(83, expected.Length);
        Assert.Equal(expected, discovered);
    }

    [Fact]
    public async Task Every_protected_route_enforces_its_documented_policy()
    {
        using var anonymous = factory.CreateClient();
        using var internalService = factory.CreateInternalServiceClient();

        foreach (var contract in HttpContractMatrix.Routes.Where(route => route.AuthorizationPolicy is not null))
        {
            using var request = CreateRequest(contract);
            using var anonymousResponse = await anonymous.SendAsync(request);
            Assert.Equal(HttpStatusCode.Unauthorized, anonymousResponse.StatusCode);
            Assert.Equal("application/problem+json", anonymousResponse.Content.Headers.ContentType?.MediaType);

            if (contract.AuthorizationPolicy == Compendium.CrossCutting.Security.CompendiumSecurity.AdministrativeWritePolicy)
            {
                using var internalRequest = CreateRequest(contract);
                using var internalResponse = await internalService.SendAsync(internalRequest);
                Assert.Equal(HttpStatusCode.Forbidden, internalResponse.StatusCode);
            }
        }
    }

    [Fact]
    public async Task Public_reads_remain_anonymous()
    {
        using var client = factory.CreateClient();

        foreach (var contract in HttpContractMatrix.Routes.Where(route => route.AuthorizationPolicy is null))
        {
            using var request = CreateRequest(contract);
            request.Method = HttpMethod.Head;
            using var response = await client.SendAsync(request);
            Assert.NotEqual(HttpStatusCode.Unauthorized, response.StatusCode);
            Assert.NotEqual(HttpStatusCode.Forbidden, response.StatusCode);
        }
    }

    [Fact]
    public async Task Cancelled_request_propagates_cancellation()
    {
        using var client = factory.CreateClient();
        using var cancellation = new CancellationTokenSource();
        cancellation.Cancel();

        await Assert.ThrowsAnyAsync<OperationCanceledException>(() =>
            client.GetAsync("/api/compendium/rulesets/UNKNOWN", cancellation.Token));
    }

    private static HttpRequestMessage CreateRequest(HttpContract contract) =>
        new(
            new HttpMethod(contract.Method),
            contract.Route
                .Replace("{code}", "UNKNOWN")
                .Replace("{id:guid}", Guid.Empty.ToString())
                .Replace("{rulesetId:guid}", Guid.Empty.ToString())
                .Replace("{ruleSourceId:guid}", Guid.Empty.ToString())
                .Replace("{versionId:guid}", Guid.Empty.ToString())
                .Replace("{classCode}", "UNKNOWN")
                .Replace("{subclassCode}", "UNKNOWN")
                .Replace("{featureCode}", "UNKNOWN")
                .Replace("{prerequisiteId:guid}", Guid.Empty.ToString())
                .Replace("{entityKind}", "feature")
                .Replace("{entityType}", "feature")
                .Replace("{entityId:guid}", Guid.Empty.ToString())
                .Replace("{equipmentItemId:guid}", Guid.Empty.ToString())
                .Replace("{ownerType}", "class")
                .Replace("{ownerId:guid}", Guid.Empty.ToString())
                .Replace("{locale}", "en-US")
                .Replace("{field}", "name")
                .Replace("{sourceVersionId:guid}", Guid.Empty.ToString()));

    private static bool IsApplicationRoute(string route) =>
        route == "/" ||
        route.StartsWith("/api/compendium", StringComparison.Ordinal) ||
        route.StartsWith("/internal/compendium", StringComparison.Ordinal);

    private static string NormalizeRoute(string route) =>
        route.StartsWith('/')
            ? route
            : $"/{route}";

    private sealed record RouteIdentity(string Method, string Route, string? Name);
}
