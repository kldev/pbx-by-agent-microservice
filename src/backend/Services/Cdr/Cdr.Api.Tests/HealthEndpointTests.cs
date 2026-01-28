using System.Net;
using CdrService.Api.Tests.Infrastructure;
using Xunit;
using Xunit.Abstractions;

namespace CdrService.Api.Tests;

[Collection(CdrDatabaseCollection.Name)]
public class HealthEndpointTests : IAsyncLifetime
{
    private readonly ITestOutputHelper _output;
    private readonly CdrApplicationFactory _factory;
    private readonly HttpClient _client;

    public HealthEndpointTests(CdrMySqlTestContainer container, ITestOutputHelper output)
    {
        _output = output;
        _factory = new CdrApplicationFactory(container.ConnectionString);
        _client = _factory.CreateClient();
    }

    public Task InitializeAsync() => Task.CompletedTask;
    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task Health_ReturnsHealthy()
    {
        // Act
        var response = await _client.GetAsync("/health");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var content = await response.Content.ReadAsStringAsync();
        Assert.Contains("healthy", content);
        Assert.Contains("CdrService", content);
    }
}
