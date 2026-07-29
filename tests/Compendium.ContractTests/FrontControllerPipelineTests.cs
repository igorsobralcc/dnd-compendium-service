using System.Net;
using Compendium.API.Controllers;
using Compendium.API.Security;
using Compendium.Application.Errors;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace Compendium.ContractTests;

public sealed class FrontControllerPipelineTests : IClassFixture<FrontControllerApiFactory>
{
    private readonly FrontControllerApiFactory factory;

    public FrontControllerPipelineTests(FrontControllerApiFactory factory)
    {
        this.factory = factory;
    }

    [Fact]
    public async Task Administrative_attribute_challenges_anonymous_requests()
    {
        var response = await factory.CreateClient()
            .PostAsync("/_contract-tests/front-controller/administrative", null);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Administrative_attribute_forbids_internal_read_credentials()
    {
        var response = await factory.CreateInternalServiceClient()
            .PostAsync("/_contract-tests/front-controller/administrative", null);

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Theory]
    [InlineData("not-found", HttpStatusCode.NotFound)]
    [InlineData("conflict", HttpStatusCode.Conflict)]
    public async Task Controller_base_maps_application_errors(
        string route,
        HttpStatusCode expectedStatus)
    {
        var response = await factory.CreateClient()
            .GetAsync($"/_contract-tests/front-controller/{route}");

        Assert.Equal(expectedStatus, response.StatusCode);
        Assert.Equal(
            "application/problem+json",
            response.Content.Headers.ContentType?.MediaType);
    }

    [Fact]
    public async Task Unhandled_exception_returns_sanitized_problem_details_with_correlation()
    {
        const string correlationId = "contract-correlation-123";
        using var request = new HttpRequestMessage(
            HttpMethod.Get,
            "/_contract-tests/front-controller/throws");
        request.Headers.Add("X-Correlation-ID", correlationId);

        var response = await factory.CreateClient().SendAsync(request);
        var body = await response.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
        Assert.Equal(
            "application/problem+json",
            response.Content.Headers.ContentType?.MediaType);
        Assert.Equal(
            correlationId,
            response.Headers.GetValues("X-Correlation-ID").Single());
        Assert.Contains("compendium.unexpected", body);
        Assert.DoesNotContain("sensitive exception detail", body);
        Assert.DoesNotContain("stack", body, StringComparison.OrdinalIgnoreCase);
    }
}

public sealed class FrontControllerApiFactory : CompendiumApiFactory
{
    protected override void ConfigureTestServices(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
            services.AddControllers()
                .AddApplicationPart(typeof(FrontControllerProbeController).Assembly));
    }
}

[ApiController]
[Route("_contract-tests/front-controller")]
public sealed class FrontControllerProbeController : CompendiumControllerBase
{
    [HttpPost("administrative")]
    [AdministrativeWrite]
    public IActionResult Administrative() => Ok();

    [HttpGet("not-found")]
    public IActionResult NotFoundResult() =>
        OkOrProblem(ApplicationResult<string>.Failure(new ApplicationError(
            "contract.not-found",
            "The contract resource was not found.",
            ApplicationErrorKind.NotFound)));

    [HttpGet("conflict")]
    public IActionResult ConflictResult() =>
        OkOrProblem(ApplicationResult<string>.Failure(new ApplicationError(
            "contract.conflict",
            "The contract resource conflicts with current state.",
            ApplicationErrorKind.Conflict)));

    [HttpGet("throws")]
    public IActionResult Throws() =>
        throw new InvalidOperationException("sensitive exception detail");
}
