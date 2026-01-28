using System.Net;
using System.Net.Http.Json;
using App.Shared.Tests;
using App.Shared.Tests.Infrastructure;
using App.Shared.Web.Security;
using Identity.Api.Features.Auth.Model;
using Identity.Api.Tests.Infrastructure;
using Xunit;
using Xunit.Abstractions;

namespace Identity.Api.Tests.Auth;

/// <summary>
/// Tests for /api/auth/validate-login endpoint.
/// This endpoint is called by Gateway to validate credentials - it does NOT return JWT token.
/// JWT token generation happens in Gateway.
/// </summary>
[Collection(IdentityDatabaseCollection.Name)]
public class ValidateLoginEndpointTests : IDisposable
{
    private readonly IdentityMySqlTestContainer _container;
    private readonly IdentityApplicationFactory _factory;
    private readonly HttpClient _client;
    private readonly ITestOutputHelper _output;

    public ValidateLoginEndpointTests(IdentityMySqlTestContainer container, ITestOutputHelper output)
    {
        _container = container;
        _output = output;
        _factory = new IdentityApplicationFactory(container.ConnectionString);
        _client = _factory.CreateClient();
    }

    public void Dispose()
    {
        _client.Dispose();
        _factory.Dispose();
    }

    [Fact]
    public async Task ValidateLogin_ReturnsValid_WithCorrectCredentials()
    {
        // Arrange
        var request = new LoginRequest
        {
            Email = "test.user1@example.com",
            Password = IdentityTestDataSeeder.TestPassword
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/validate-login", request);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var result = await response.ReadWithJson<ValidateLoginResponse>(_output);
        Assert.NotNull(result);
        Assert.True(result.IsValid);
        Assert.Equal(TestFixtureIds.Gids.TestUser1, result.Gid);
        Assert.Equal("test.user1@example.com", result.Email);
        Assert.Equal("Test", result.FirstName);
        Assert.Equal("User1", result.LastName);
        Assert.Null(result.ErrorCode);
    }

    [Fact]
    public async Task ValidateLogin_ReturnsInvalid_WithWrongPassword()
    {
        // Arrange
        var request = new LoginRequest
        {
            Email = "test.user1@example.com",
            Password = "WrongPassword123!"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/validate-login", request);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var result = await response.ReadWithJson<ValidateLoginResponse>(_output);
        Assert.NotNull(result);
        Assert.False(result.IsValid);
        Assert.NotNull(result.ErrorCode);
    }

    [Fact]
    public async Task ValidateLogin_ReturnsInvalid_WithNonExistentEmail()
    {
        // Arrange
        var request = new LoginRequest
        {
            Email = "nonexistent@example.com",
            Password = IdentityTestDataSeeder.TestPassword
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/validate-login", request);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var result = await response.ReadWithJson<ValidateLoginResponse>(_output);
        Assert.NotNull(result);
        Assert.False(result.IsValid);
        Assert.NotNull(result.ErrorCode);
    }

    [Fact]
    public async Task ValidateLogin_ReturnsInvalid_WhenUserIsInactive()
    {
        // Arrange - inactive user seeded as TestUser3
        var request = new LoginRequest
        {
            Email = "inactive.user@example.com",
            Password = IdentityTestDataSeeder.TestPassword
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/validate-login", request);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var result = await response.ReadWithJson<ValidateLoginResponse>(_output);
        Assert.NotNull(result);
        Assert.False(result.IsValid);
        Assert.NotNull(result.ErrorCode);
    }

    [Fact]
    public async Task ValidateLogin_IsAccessible_WithoutAuthentication()
    {
        // Arrange
        var request = new LoginRequest
        {
            Email = "test.user1@example.com",
            Password = IdentityTestDataSeeder.TestPassword
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/validate-login", request);

        // Assert - should not be 401 Unauthorized (endpoint is AllowAnonymous)
        Assert.NotEqual(HttpStatusCode.Unauthorized, response.StatusCode);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}

/// <summary>
/// Tests for /api/auth/me endpoint.
/// This endpoint reads user info from X-* headers set by Gateway.
/// </summary>
[Collection(IdentityDatabaseCollection.Name)]
public class GetMeEndpointTests : IDisposable
{
    private readonly IdentityMySqlTestContainer _container;
    private readonly IdentityApplicationFactory _factory;
    private readonly ITestOutputHelper _output;

    public GetMeEndpointTests(IdentityMySqlTestContainer container, ITestOutputHelper output)
    {
        _container = container;
        _output = output;
        _factory = new IdentityApplicationFactory(container.ConnectionString);
    }

    public void Dispose()
    {
        _factory.Dispose();
    }

    [Fact]
    public async Task GetMe_ReturnsSuccess_WithGatewayHeaders()
    {
        // Arrange - simulate Gateway headers
        using var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Add(HttpContextExtensions.UserGidHeader, TestFixtureIds.Gids.TestUser1);

        // Act
        var response = await client.PostAsync("/api/auth/me", null);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var result = await response.ReadWithJson<MeResponse>(_output);
        Assert.NotNull(result);
        Assert.Equal(TestFixtureIds.Gids.TestUser1, result.Gid);
        Assert.Equal("test.user1@example.com", result.Email);
        Assert.Equal("Test", result.FirstName);
        Assert.Equal("User1", result.LastName);
    }

    [Fact]
    public async Task GetMe_ReturnsUserInfo_ForDifferentUsers()
    {
        // Arrange - simulate Gateway headers for TestUser2
        using var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Add(HttpContextExtensions.UserGidHeader, TestFixtureIds.Gids.TestUser2);

        // Act
        var response = await client.PostAsync("/api/auth/me", null);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var result = await response.ReadWithJson<MeResponse>(_output);
        Assert.NotNull(result);
        Assert.Equal(TestFixtureIds.Gids.TestUser2, result.Gid);
        Assert.Equal("test.user2@example.com", result.Email);
    }

    [Fact]
    public async Task GetMe_ReturnsNotFound_WhenUserDoesNotExistInDatabase()
    {
        // Arrange - simulate Gateway headers for non-existent user
        using var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Add(HttpContextExtensions.UserGidHeader, "non-existent-gid");

        // Act
        var response = await client.PostAsync("/api/auth/me", null);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetMe_ReturnsUnauthorized_WithoutGatewayHeaders()
    {
        // Arrange - no X-User-Gid header
        using var client = _factory.CreateClient();

        // Act
        var response = await client.PostAsync("/api/auth/me", null);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}
