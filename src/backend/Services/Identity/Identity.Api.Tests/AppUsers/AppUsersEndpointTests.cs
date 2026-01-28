using System.Net;
using System.Net.Http.Json;
using App.Bps.Enum;
using App.Shared.Tests;
using App.Shared.Tests.Infrastructure;
using App.Shared.Web.BaseModel;
using Identity.Api.Features.AppUsers.Model;
using Identity.Api.Tests.Infrastructure;
using Xunit;
using Xunit.Abstractions;

namespace Identity.Api.Tests.AppUsers;

/// <summary>
/// Functional tests for AppUsers endpoints (using Root role by default).
/// </summary>
[Collection(IdentityDatabaseCollection.Name)]
public class AppUsersEndpointTests : IDisposable
{
    private readonly IdentityMySqlTestContainer _container;
    private readonly IdentityApplicationFactory _factory;
    private readonly HttpClient _client;
    private readonly ITestOutputHelper _output;

    public AppUsersEndpointTests(IdentityMySqlTestContainer container, ITestOutputHelper output)
    {
        _container = container;
        _output = output;
        // Use Root role for functional tests
        _factory = new IdentityApplicationFactory(container.ConnectionString,
            options => options.Roles = [Roles.Root]);
        _client = _factory.CreateClient();
    }

    public void Dispose()
    {
        _client.Dispose();
        _factory.Dispose();
    }

    [Fact]
    public async Task GetByGid_ReturnsSuccess_WhenUserExists()
    {
        // Act
        var response = await _client.PostAsJsonAsync("/api/app-users/by-gid",
            new GetByGidRequest { Gid = TestFixtureIds.Gids.TestUser1 });

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var result = await response.ReadWithJson<AppUserResponse>(_output);
        Assert.NotNull(result);
        Assert.Equal(TestFixtureIds.Gids.TestUser1, result.Gid);
        Assert.Equal("Test", result.FirstName);
        Assert.Equal("User1", result.LastName);
        Assert.Equal("test.user1@example.com", result.Email);
    }

    [Fact]
    public async Task GetByGid_ReturnsNotFound_WhenUserDoesNotExist()
    {
        // Act
        var response = await _client.PostAsJsonAsync("/api/app-users/by-gid",
            new GetByGidRequest { Gid = "non-existent-gid" });

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetList_ReturnsPagedResults()
    {
        // Act
        var response = await _client.PostAsJsonAsync("/api/app-users/list",
            new AppUserListFilter { PageNumber = 1, PageSize = 10, IsActive = true });

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetList_FiltersBySbuId()
    {
        // Act
        var response = await _client.PostAsJsonAsync("/api/app-users/list",
            new AppUserListFilter { PageNumber = 1, PageSize = 10, StructureId = TestFixtureIds.Ids.TestStructure1 });

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetList_FiltersByDepartment()
    {
        // Act
        var response = await _client.PostAsJsonAsync("/api/app-users/list",
            new AppUserListFilter { PageNumber = 1, PageSize = 10, Department = Department.Support });

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Create_ReturnsSuccess_WithValidRequest()
    {
        // Arrange
        var request = new CreateAppUserRequest
        {
            FirstName = "New",
            LastName = "User",
            Email = $"new.user.{Guid.NewGuid():N}@example.com",
            Password = "Test123!",
            StructureId = TestFixtureIds.Ids.TestStructure1,
            TeamId = TestFixtureIds.Ids.TestTeam1,
            Department = Department.Support,
            Roles = [Roles.Support, Roles.User]
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/app-users", request);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var result = await response.ReadWithJson<AppUserResponse>(_output);
        Assert.NotNull(result);
        Assert.Equal("New", result.FirstName);
        Assert.Equal("User", result.LastName);
        Assert.NotEmpty(result.Gid);
    }

    [Fact]
    public async Task Update_ReturnsSuccess_WhenUserExists()
    {
        // First create a user to update
        var createRequest = new CreateAppUserRequest
        {
            FirstName = "Update",
            LastName = "Test",
            Email = $"update.test.{Guid.NewGuid():N}@example.com",
            Password = "Test123!",
            StructureId = TestFixtureIds.Ids.TestStructure1,
            Roles = [Roles.User]
        };
        var createResponse = await _client.PostAsJsonAsync("/api/app-users", createRequest);
        var createdUser = await createResponse.ReadWithJson<AppUserResponse>(_output);

        // Arrange
        var updateRequest = new UpdateAppUserRequest
        {
            FirstName = "Updated",
            LastName = "Name",
            Email = createdUser!.Email,
            StructureId = TestFixtureIds.Ids.TestStructure1,
            IsActive = true,
            Roles = [Roles.User]
        };

        // Act
        var response = await _client.PutAsJsonAsync(
            $"/api/app-users/{createdUser.Gid}",
            updateRequest);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var result = await response.ReadWithJson<AppUserResponse>(_output);
        Assert.NotNull(result);
        Assert.Equal("Updated", result.FirstName);
        Assert.Equal("Name", result.LastName);
    }

    [Fact]
    public async Task Update_ReturnsNotFound_WhenUserDoesNotExist()
    {
        // Arrange
        var updateRequest = new UpdateAppUserRequest
        {
            FirstName = "Test",
            LastName = "User",
            Email = "test@example.com",
            StructureId = TestFixtureIds.Ids.TestStructure1,
            IsActive = true,
            Roles = [Roles.User]
        };

        // Act
        var response = await _client.PutAsJsonAsync("/api/app-users/non-existent", updateRequest);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task ChangePassword_ReturnsSuccess_WhenUserExists()
    {
        // First create a user
        var createRequest = new CreateAppUserRequest
        {
            FirstName = "Password",
            LastName = "Test",
            Email = $"password.test.{Guid.NewGuid():N}@example.com",
            Password = "OldPassword123!",
            StructureId = TestFixtureIds.Ids.TestStructure1,
            Roles = [Roles.User]
        };
        var createResponse = await _client.PostAsJsonAsync("/api/app-users", createRequest);
        var createdUser = await createResponse.ReadWithJson<AppUserResponse>(_output);

        // Arrange
        var changePasswordRequest = new ChangePasswordRequest
        {
            NewPassword = "NewPassword123!"
        };

        // Act
        var response = await _client.PutAsJsonAsync(
            $"/api/app-users/{createdUser!.Gid}/password",
            changePasswordRequest);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Delete_ReturnsSuccess_WhenUserExists()
    {
        // First create a user to delete
        var createRequest = new CreateAppUserRequest
        {
            FirstName = "Delete",
            LastName = "Test",
            Email = $"delete.test.{Guid.NewGuid():N}@example.com",
            Password = "Test123!",
            StructureId = TestFixtureIds.Ids.TestStructure1,
            Roles = [Roles.User]
        };
        var createResponse = await _client.PostAsJsonAsync("/api/app-users", createRequest);
        var createdUser = await createResponse.ReadWithJson<AppUserResponse>(_output);

        // Act
        var response = await _client.DeleteAsync($"/api/app-users/{createdUser!.Gid}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}

/// <summary>
/// Authorization tests for AppUsers endpoints.
/// Tests role-based access control.
/// </summary>
[Collection(IdentityDatabaseCollection.Name)]
public class AppUsersEndpointAuthorizationTests : IDisposable
{
    private readonly IdentityMySqlTestContainer _container;
    private readonly ITestOutputHelper _output;

    public AppUsersEndpointAuthorizationTests(IdentityMySqlTestContainer container, ITestOutputHelper output)
    {
        _container = container;
        _output = output;
    }

    public void Dispose()
    {
    }

    // ===== GetByGid - any logged-in user =====

    [Theory]
    [InlineData("Root")]
    [InlineData("Admin")]
    [InlineData("Ops")]
    [InlineData("Support")]
    [InlineData("User")]
    public async Task GetByGid_AllowedForAnyLoggedInUser(string role)
    {
        // Arrange
        using var factory = new IdentityApplicationFactory(
            _container.ConnectionString,
            options => options.Roles = [role]);
        using var client = factory.CreateClient();

        // Act
        var response = await client.PostAsJsonAsync("/api/app-users/by-gid",
            new GetByGidRequest { Gid = TestFixtureIds.Gids.TestUser1 });

        // Assert - should not be forbidden
        Assert.NotEqual(HttpStatusCode.Forbidden, response.StatusCode);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    // ===== GetList - Root only =====

    [Fact]
    public async Task GetList_AllowedForRoot()
    {
        // Arrange
        using var factory = new IdentityApplicationFactory(
            _container.ConnectionString,
            options => options.Roles = [Roles.Root]);
        using var client = factory.CreateClient();

        // Act
        var response = await client.PostAsJsonAsync("/api/app-users/list",
            new AppUserListFilter { PageNumber = 1, PageSize = 10 });

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Theory]
    [InlineData("Admin")]
    [InlineData("Ops")]
    [InlineData("Support")]
    [InlineData("User")]
    public async Task GetList_ForbiddenForNonRoot(string role)
    {
        // Arrange
        using var factory = new IdentityApplicationFactory(
            _container.ConnectionString,
            options => options.Roles = [role]);
        using var client = factory.CreateClient();

        // Act
        var response = await client.PostAsJsonAsync("/api/app-users/list",
            new AppUserListFilter { PageNumber = 1, PageSize = 10 });

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    // ===== Create - Root only =====

    [Theory]
    [InlineData("Admin")]
    [InlineData("Ops")]
    [InlineData("Support")]
    [InlineData("User")]
    public async Task Create_ForbiddenForNonRoot(string role)
    {
        // Arrange
        using var factory = new IdentityApplicationFactory(
            _container.ConnectionString,
            options => options.Roles = [role]);
        using var client = factory.CreateClient();

        var request = new CreateAppUserRequest
        {
            FirstName = "Test",
            LastName = "User",
            Email = "test@example.com",
            Password = "Test123!",
            StructureId = TestFixtureIds.Ids.TestStructure1,
            Roles = [Roles.User]
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/app-users", request);

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    // ===== Delete - Root only =====

    [Theory]
    [InlineData("Admin")]
    [InlineData("Ops")]
    [InlineData("Support")]
    public async Task Delete_ForbiddenForNonRoot(string role)
    {
        // Arrange
        using var factory = new IdentityApplicationFactory(
            _container.ConnectionString,
            options => options.Roles = [role]);
        using var client = factory.CreateClient();

        // Act
        var response = await client.DeleteAsync($"/api/app-users/{TestFixtureIds.Gids.TestUser1}");

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    // ===== ChangePassword - Root or self =====

    [Fact]
    public async Task ChangePassword_AllowedForRoot()
    {
        // Arrange - Root can change any user's password
        using var factory = new IdentityApplicationFactory(
            _container.ConnectionString,
            options =>
            {
                options.UserId = TestFixtureIds.Ids.TestUser1;
                options.Gid = TestFixtureIds.Gids.TestUser1;
                options.Roles = [Roles.Root];
            });
        using var client = factory.CreateClient();

        // Act - Root changes TestUser2's password
        var response = await client.PutAsJsonAsync(
            $"/api/app-users/{TestFixtureIds.Gids.TestUser2}/password",
            new ChangePasswordRequest { NewPassword = "NewPass123!" });

        // Assert
        Assert.NotEqual(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task ChangePassword_AllowedForSelf()
    {
        // Arrange - User can change their own password
        using var factory = new IdentityApplicationFactory(
            _container.ConnectionString,
            options =>
            {
                options.UserId = TestFixtureIds.Ids.TestUser1;
                options.Gid = TestFixtureIds.Gids.TestUser1;
                options.Roles = ["User"]; // Not Root
            });
        using var client = factory.CreateClient();

        // Act - User changes their own password
        var response = await client.PutAsJsonAsync(
            $"/api/app-users/{TestFixtureIds.Gids.TestUser1}/password",
            new ChangePasswordRequest { NewPassword = "NewPass123!" });

        // Assert
        Assert.NotEqual(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task ChangePassword_ForbiddenForOtherUser()
    {
        // Arrange - Regular user cannot change another user's password
        using var factory = new IdentityApplicationFactory(
            _container.ConnectionString,
            options =>
            {
                options.UserId = TestFixtureIds.Ids.TestUser1;
                options.Gid = TestFixtureIds.Gids.TestUser1;
                options.Roles = ["User"]; // Not Root
            });
        using var client = factory.CreateClient();

        // Act - User tries to change TestUser2's password
        var response = await client.PutAsJsonAsync(
            $"/api/app-users/{TestFixtureIds.Gids.TestUser2}/password",
            new ChangePasswordRequest { NewPassword = "NewPass123!" });

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }
}
