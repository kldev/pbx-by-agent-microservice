using System.Net;
using System.Net.Http.Json;
using App.Bps.Enum;
using App.Shared.Tests;
using App.Shared.Tests.Infrastructure;
using App.Shared.Web.BaseModel;
using Identity.Api.Features.Teams.Model;
using Identity.Api.Tests.Infrastructure;
using Xunit;
using Xunit.Abstractions;

namespace Identity.Api.Tests.Teams;

[Collection(IdentityDatabaseCollection.Name)]
public class TeamsEndpointTests : IDisposable
{
    private readonly IdentityMySqlTestContainer _container;
    private readonly IdentityApplicationFactory _factory;
    private readonly HttpClient _client;
    private readonly ITestOutputHelper _output;

    public TeamsEndpointTests(IdentityMySqlTestContainer container, ITestOutputHelper output)
    {
        _container = container;
        _output = output;
        _factory = new IdentityApplicationFactory(container.ConnectionString, _ =>
        {
            _.Roles = [nameof(AppRole.Root)];
            _.FirstName = "Test";
            _.LastName = "TestLast";
        });
        _client = _factory.CreateClient();
    }

    public void Dispose()
    {
        _client.Dispose();
        _factory.Dispose();
    }

    [Fact]
    public async Task GetByGid_ReturnsSuccess_WhenTeamExists()
    {
        // Act
        var response = await _client.PostAsJsonAsync("/api/teams/by-gid",
            new GetByGidRequest { Gid = TestFixtureIds.Gids.TestTeam1 });

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var result = await response.ReadWithJson<TeamResponse>(_output);
        Assert.NotNull(result);
        Assert.Equal(TestFixtureIds.Gids.TestTeam1, result.Gid);
        Assert.Equal("TEAM1", result.Code);
        Assert.Equal("Test Team 1", result.Name);
    }

    [Fact]
    public async Task GetByGid_ReturnsNotFound_WhenTeamDoesNotExist()
    {
        // Act
        var response = await _client.PostAsJsonAsync("/api/teams/by-gid",
            new GetByGidRequest { Gid = "non-existent-gid" });

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetList_ReturnsPagedResults()
    {
        // Act
        var response = await _client.PostAsJsonAsync("/api/teams/list",
            new TeamListFilter { PageNumber = 1, PageSize = 10, IsActive = true });

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetList_FiltersBySbuId()
    {
        // Act
        var response = await _client.PostAsJsonAsync("/api/teams/list",
            new TeamListFilter { PageNumber = 1, PageSize = 10, StructureId = TestFixtureIds.Ids.TestStructure1 });

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Create_ReturnsSuccess_WithValidRequest()
    {
        // Arrange
        var request = new CreateTeamRequest
        {
            Code = "NEWTEAM",
            Name = "New Test Team",
            StructureId = TestFixtureIds.Ids.TestStructure1
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/teams", request);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var result = await response.ReadWithJson<TeamResponse>(_output);
        Assert.NotNull(result);
        Assert.Equal("NEWTEAM", result.Code);
        Assert.Equal("New Test Team", result.Name);
        Assert.NotEmpty(result.Gid);
    }

    [Fact]
    public async Task Update_ReturnsSuccess_WhenTeamExists()
    {
        // Arrange - use TestTeam2 to avoid affecting other tests
        var updateRequest = new UpdateTeamRequest
        {
            Code = "TEAM2",
            Name = "Updated Team 2 Name",
            StructureId = TestFixtureIds.Ids.TestStructure2,
            IsActive = true
        };

        // Act
        var response = await _client.PutAsJsonAsync(
            $"/api/teams/{TestFixtureIds.Gids.TestTeam2}",
            updateRequest);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var result = await response.ReadWithJson<TeamResponse>(_output);
        Assert.NotNull(result);
        Assert.Equal("Updated Team 2 Name", result.Name);
    }

    [Fact]
    public async Task Update_ReturnsNotFound_WhenTeamDoesNotExist()
    {
        // Arrange
        var updateRequest = new UpdateTeamRequest
        {
            Code = "TEST",
            Name = "Test",
            StructureId = TestFixtureIds.Ids.TestStructure1,
            IsActive = true
        };

        // Act
        var response = await _client.PutAsJsonAsync("/api/teams/non-existent", updateRequest);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Delete_ReturnsSuccess_WhenTeamExists()
    {
        // First create a team to delete
        var createRequest = new CreateTeamRequest
        {
            Code = "TODEL",
            Name = "Team To Delete",
            StructureId = TestFixtureIds.Ids.TestStructure1
        };
        var createResponse = await _client.PostAsJsonAsync("/api/teams", createRequest);
        var createdTeam = await createResponse.ReadWithJson<TeamResponse>(_output);

        // Act
        var response = await _client.DeleteAsync($"/api/teams/{createdTeam!.Gid}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}

/// <summary>
/// Tests for Teams endpoints with different user roles
/// </summary>
[Collection(IdentityDatabaseCollection.Name)]
public class TeamsEndpointAuthorizationTests : IDisposable
{
    private readonly IdentityMySqlTestContainer _container;
    private readonly ITestOutputHelper _output;

    public TeamsEndpointAuthorizationTests(IdentityMySqlTestContainer container, ITestOutputHelper output)
    {
        _container = container;
        _output = output;
    }

    public void Dispose()
    {
    }

    [Fact]
    public async Task GetByGid_AllowedForAdmin()
    {
        // Arrange
        using var factory = new IdentityApplicationFactory(
            _container.ConnectionString,
            options => options.Roles = [nameof(AppRole.Admin)]);
        using var client = factory.CreateClient();

        // Act
        var response = await client.PostAsJsonAsync("/api/teams/by-gid",
            new GetByGidRequest { Gid = TestFixtureIds.Gids.TestTeam1 });

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Theory]
    [InlineData("SalesPerson")]
    [InlineData("HrPoland")]
    [InlineData("OpsPoland")]
    [InlineData("Finance")]
    public async Task GetByGid_ForbiddenForNonAdminRoles(string role)
    {
        // Arrange
        using var factory = new IdentityApplicationFactory(
            _container.ConnectionString,
            options => options.Roles = [role]);
        using var client = factory.CreateClient();

        // Act
        var response = await client.PostAsJsonAsync("/api/teams/by-gid",
            new GetByGidRequest { Gid = TestFixtureIds.Gids.TestTeam1 });

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Theory]
    [InlineData("SalesPerson")]
    [InlineData("HrPoland")]
    public async Task Create_ForbiddenForNonAdminRole(string role)
    {
        // Arrange
        using var factory = new IdentityApplicationFactory(
            _container.ConnectionString,
            options => options.Roles = [role]);
        using var client = factory.CreateClient();

        var request = new CreateTeamRequest
        {
            Code = "TEST",
            Name = "Test",
            StructureId = TestFixtureIds.Ids.TestStructure1
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/teams", request);

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }
}
