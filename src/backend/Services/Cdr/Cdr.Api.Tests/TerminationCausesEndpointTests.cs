using System.Net;
using System.Net.Http.Json;
using CdrService.Api.Features.TerminationCauses.Model;
using CdrService.Api.Tests.Infrastructure;
using Xunit;
using Xunit.Abstractions;

namespace CdrService.Api.Tests;

[Collection(CdrDatabaseCollection.Name)]
public class TerminationCausesEndpointTests : BaseCdrTest, IAsyncLifetime
{
    private readonly ITestOutputHelper _output;
    private readonly CdrApplicationFactory _factory;
    private readonly HttpClient _client;

    public TerminationCausesEndpointTests(CdrMySqlTestContainer container, ITestOutputHelper output)
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
    public async Task GetList_ReturnsAllCauses()
    {
        // Act
        var response = await _client.PostAsync("/api/termination-causes/list", null);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var causes = await response.Content.ReadFromJsonAsync<IEnumerable<TerminationCauseResponse>>();
        Assert.NotNull(causes);
        Assert.NotEmpty(causes);
    }

    [Fact]
    public async Task GetByGid_ExistingCause_ReturnsCause()
    {
        // Act
        var response = await _client.GetAsync(
            $"/api/termination-causes/{CdrTestDataSeeder.TestTerminationCauseGid}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var cause = await response.Content.ReadFromJsonAsync<TerminationCauseResponse>();
        Assert.NotNull(cause);
        Assert.Equal("NORMAL_CLEARING", cause.Code);
        Assert.Equal(16, cause.Q850Code);
    }

    [Fact]
    public async Task GetByGid_NonExistingCause_Returns404()
    {
        // Act
        var response = await _client.GetAsync("/api/termination-causes/non-existing-gid");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
