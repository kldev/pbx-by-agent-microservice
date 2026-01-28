using System.Net;
using App.Shared.Web.BaseModel;
using Common.Toolkit.Json;
using RateService.Api.Tests.Infrastructure;
using Xunit;

namespace RateService.Api.Tests;

[Collection(RateServiceDatabaseCollection.Name)]
public class HealthEndpointTests : IAsyncLifetime
{
    private readonly RateServiceApplicationFactory _factory;
    private readonly HttpClient _client;

    public HealthEndpointTests(RateServiceMySqlTestContainer container)
    {
        _factory = new RateServiceApplicationFactory(container.ConnectionString);
        _client = _factory.CreateClient();
    }

    public async Task InitializeAsync()
    {
        await _factory.SeedTestDataAsync();
    }

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task Health_ReturnsOk()
    {
        // Act
        var response = await _client.GetAsync("/health");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var content = await response.Content.ReadAsStringAsync();
        var result = JsonUtil.Read<HealthStatusResponse>(content);
        Assert.NotNull(result);
        Assert.Contains("healthy", result.Status);
        Assert.Contains("RateService", result.Service);
    }
}
