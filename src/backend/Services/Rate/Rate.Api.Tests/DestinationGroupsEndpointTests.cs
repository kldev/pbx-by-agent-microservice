using System.Net;
using System.Net.Http.Json;
using App.Bps.Enum;
using App.Shared.Tests.Infrastructure;
using RateService.Api.Features.DestinationGroups.Model;
using RateService.Api.Tests.Infrastructure;
using Xunit;

namespace RateService.Api.Tests;

[Collection(RateServiceDatabaseCollection.Name)]
public class DestinationGroupsEndpointTests : BaseRateTest, IAsyncLifetime
{
    private readonly RateServiceApplicationFactory _factory;
    private readonly HttpClient _client;

    public DestinationGroupsEndpointTests(RateServiceMySqlTestContainer container)
    {
      
        
        _factory = new RateServiceApplicationFactory(container.ConnectionString, ConfigureAuthForFullAccess);
        _client = _factory.CreateClient();
    }

    public async Task InitializeAsync()
    {
        await _factory.SeedTestDataAsync();
    }

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task GetAll_ReturnsGroups()
    {
        // Act
        var response = await _client.GetAsync("/api/destination-groups");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var groups = await response.Content.ReadFromJsonAsync<IEnumerable<DestinationGroupResponse>>();
        Assert.NotNull(groups);
        Assert.NotEmpty(groups);
    }

    [Fact]
    public async Task GetById_ExistingGroup_ReturnsGroup()
    {
        // Act
        var response = await _client.GetAsync("/api/destination-groups/1");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var group = await response.Content.ReadFromJsonAsync<DestinationGroupResponse>();
        Assert.NotNull(group);
        Assert.Equal("TestGroup", group.Name);
    }

    [Fact]
    public async Task GetById_NonExistingGroup_Returns404()
    {
        // Act
        var response = await _client.GetAsync("/api/destination-groups/999");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Create_ValidRequest_ReturnsCreatedGroup()
    {
        // Arrange
        var request = new CreateDestinationGroupRequest
        {
            Name = $"NewGroup{Guid.NewGuid():N}",
            NamePL = "Nowa Grupa",
            NameEN = "New Group"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/destination-groups", request);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var group = await response.Content.ReadFromJsonAsync<DestinationGroupResponse>();
        Assert.NotNull(group);
        Assert.Contains("NewGroup", group.Name);
    }

    [Fact]
    public async Task Create_DuplicateName_Returns400()
    {
        // Arrange - TestGroup już istnieje z seed
        var request = new CreateDestinationGroupRequest
        {
            Name = "TestGroup",
            NamePL = "Testowa Grupa",
            NameEN = "Test Group"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/destination-groups", request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Update_ValidRequest_ReturnsUpdatedGroup()
    {
        // Arrange
        var updateRequest = new CreateDestinationGroupRequest
        {
            Name = "TestGroup",
            NamePL = "Zaktualizowana Grupa",
            NameEN = "Updated Group"
        };

        // Act
        var response = await _client.PutAsJsonAsync("/api/destination-groups/1", updateRequest);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var group = await response.Content.ReadFromJsonAsync<DestinationGroupResponse>();
        Assert.NotNull(group);
        Assert.Equal("Zaktualizowana Grupa", group.NamePL);
    }
}
