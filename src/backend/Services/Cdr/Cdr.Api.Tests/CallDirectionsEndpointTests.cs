using System.Net;
using System.Net.Http.Json;
using CdrService.Api.Features.CallDirections.Model;
using CdrService.Api.Tests.Infrastructure;
using Xunit;
using Xunit.Abstractions;

namespace CdrService.Api.Tests;

[Collection(CdrDatabaseCollection.Name)]
public class CallDirectionsEndpointTests : BaseCdrTest, IAsyncLifetime
{
    private readonly ITestOutputHelper _output;
    private readonly CdrApplicationFactory _factory;
    private readonly HttpClient _client;

    public CallDirectionsEndpointTests(CdrMySqlTestContainer container, ITestOutputHelper output)
    {
        _output = output;
        _factory = new CdrApplicationFactory(container.ConnectionString, ConfigureAuthForFullAccess);
        _client = _factory.CreateClient();
    }

    public async Task InitializeAsync()
    {
        await _factory.SeedTestDataAsync();
    }

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task GetList_ReturnsAllDirections()
    {
        // Act
        var response = await _client.PostAsync("/api/call-directions/list", null);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var directions = await response.Content.ReadFromJsonAsync<IEnumerable<CallDirectionResponse>>();
        Assert.NotNull(directions);
        Assert.NotEmpty(directions);
    }

    [Fact]
    public async Task GetByGid_ExistingDirection_ReturnsDirection()
    {
        // Act
        var response = await _client.GetAsync(
            $"/api/call-directions/{CdrTestDataSeeder.TestCallDirectionGid}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var direction = await response.Content.ReadFromJsonAsync<CallDirectionResponse>();
        Assert.NotNull(direction);
        Assert.Equal("OUTBOUND", direction.Code);
    }

    [Fact]
    public async Task GetByGid_NonExistingDirection_Returns404()
    {
        // Act
        var response = await _client.GetAsync("/api/call-directions/non-existing-gid");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
