using System.Net;
using System.Net.Http.Json;
using CdrService.Api.Features.CallStatuses.Model;
using CdrService.Api.Tests.Infrastructure;
using Xunit;
using Xunit.Abstractions;

namespace CdrService.Api.Tests;

[Collection(CdrDatabaseCollection.Name)]
public class CallStatusesEndpointTests : BaseCdrTest, IAsyncLifetime
{
    private readonly ITestOutputHelper _output;
    private readonly CdrApplicationFactory _factory;
    private readonly HttpClient _client;

    public CallStatusesEndpointTests(CdrMySqlTestContainer container, ITestOutputHelper output)
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
    public async Task GetList_ReturnsAllStatuses()
    {
        // Act
        var response = await _client.PostAsync("/api/call-statuses/list", null);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var statuses = await response.Content.ReadFromJsonAsync<IEnumerable<CallStatusResponse>>();
        Assert.NotNull(statuses);
        Assert.NotEmpty(statuses);
    }

    [Fact]
    public async Task GetByGid_ExistingStatus_ReturnsStatus()
    {
        // Act
        var response = await _client.GetAsync(
            $"/api/call-statuses/{CdrTestDataSeeder.TestCallStatusGid}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var status = await response.Content.ReadFromJsonAsync<CallStatusResponse>();
        Assert.NotNull(status);
        Assert.Equal("COMPLETED", status.Code);
    }

    [Fact]
    public async Task GetByGid_NonExistingStatus_Returns404()
    {
        // Act
        var response = await _client.GetAsync("/api/call-statuses/non-existing-gid");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
