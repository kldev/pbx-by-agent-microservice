using System.Net;
using System.Net.Http.Json;
using App.Bps.Enum;
using App.Shared.Tests.Infrastructure;
using Identity.Api.Tests.Infrastructure;
using Identity.Data;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Xunit.Abstractions;

namespace Identity.Api.Tests.RolesAndPermissions;

/// <summary>
/// Tests for roles and permissions seed and database structure.
/// </summary>
[Collection(IdentityDatabaseCollection.Name)]
public class RolesAndPermissionsTests : IDisposable
{
    private readonly IdentityMySqlTestContainer _container;
    private readonly IdentityApplicationFactory _factory;
    private readonly ITestOutputHelper _output;

    public RolesAndPermissionsTests(IdentityMySqlTestContainer container, ITestOutputHelper output)
    {
        _container = container;
        _output = output;
        _factory = new IdentityApplicationFactory(container.ConnectionString, _ =>
        {
            _.Roles = [nameof(AppRole.Root)];
            _.FirstName = "Test";
            _.LastName = "TestLast";
        });
    }

    public void Dispose()
    {
        _factory.Dispose();
    }

    [Fact]
    public void AllAppRolesAreSeededToDatabase()
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<IdentityDbContext>();

        // Act
        var rolesInDb = context.AppUserRoles.ToList();

        // Assert
        var allEnumRoles = Enum.GetValues<AppRole>();
        Assert.Equal(allEnumRoles.Length, rolesInDb.Count);

        foreach (var enumRole in allEnumRoles)
        {
            var dbRole = rolesInDb.FirstOrDefault(r => r.Id == (int)enumRole);
            Assert.NotNull(dbRole);
            Assert.Equal(enumRole.ToString(), dbRole.Name);
            Assert.True(dbRole.IsActive);
        }
    }

    [Fact]
    public void AllAppPermissionsAreSeededToDatabase()
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<IdentityDbContext>();

        // Act
        var permissionsInDb = context.AppPermissions.ToList();

        // Assert
        var allEnumPermissions = Enum.GetValues<AppPermission>();
        Assert.Equal(allEnumPermissions.Length, permissionsInDb.Count);

        foreach (var enumPerm in allEnumPermissions)
        {
            var dbPerm = permissionsInDb.FirstOrDefault(p => p.Id == (int)enumPerm);
            Assert.NotNull(dbPerm);
            Assert.Equal(enumPerm.ToString(), dbPerm.Name);
            Assert.True(dbPerm.IsActive);
        }
    }

    [Fact]
    public void RoleIdsMatchEnumValues()
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<IdentityDbContext>();

        // Act & Assert
        Assert.Equal((int)AppRole.Root, context.AppUserRoles.First(r => r.Name == "Root").Id);
        Assert.Equal((int)AppRole.Ops, context.AppUserRoles.First(r => r.Name == "Ops").Id);
        Assert.Equal((int)AppRole.Admin, context.AppUserRoles.First(r => r.Name == "Admin").Id);
        Assert.Equal((int)AppRole.Support, context.AppUserRoles.First(r => r.Name == "Support").Id);
        Assert.Equal((int)AppRole.User, context.AppUserRoles.First(r => r.Name == "User").Id);
    }

    [Fact]
    public void PermissionIdsMatchEnumValues()
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<IdentityDbContext>();

        // Act & Assert
        Assert.Equal((int)AppPermission.InvoicesBank, context.AppPermissions.First(p => p.Name == "InvoicesBank").Id);
        Assert.Equal((int)AppPermission.SensitiveData, context.AppPermissions.First(p => p.Name == "SensitiveData").Id);
    }

    [Fact]
    public void SpecialPermissionsAreMarkedCorrectly()
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<IdentityDbContext>();

        // Act
        var invoicesBankPerm = context.AppPermissions.First(p => p.Name == "InvoicesBank");
        var sensitiveDataPerm = context.AppPermissions.First(p => p.Name == "SensitiveData");

        // Assert - Both are special permissions
        Assert.True(invoicesBankPerm.IsSpecial);
        Assert.True(sensitiveDataPerm.IsSpecial);
    }

    [Fact]
    public void PermissionCategoriesAreSetCorrectly()
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<IdentityDbContext>();

        // Act & Assert
        Assert.Equal("Finance", context.AppPermissions.First(p => p.Name == "InvoicesBank").Category);
        Assert.Equal("System", context.AppPermissions.First(p => p.Name == "SensitiveData").Category);
    }
}

/// <summary>
/// Tests for authorization based on roles.
/// </summary>
[Collection(IdentityDatabaseCollection.Name)]
public class RoleBasedAuthorizationTests : IDisposable
{
    private readonly IdentityMySqlTestContainer _container;
    private readonly ITestOutputHelper _output;

    public RoleBasedAuthorizationTests(IdentityMySqlTestContainer container, ITestOutputHelper output)
    {
        _container = container;
        _output = output;
    }

    public void Dispose()
    {
    }

    [Fact]
    public async Task AdminEndpoint_AllowsAdminRole()
    {
        // Arrange
        using var factory = new IdentityApplicationFactory(
            _container.ConnectionString,
            options =>
            {
                options.UserId = TestFixtureIds.Ids.TestUser1;
                options.Gid = TestFixtureIds.Gids.TestUser1;
                options.Email = "admin@example.com";
                options.Roles = [Roles.Admin];
            });
        using var client = factory.CreateClient();

        // Act
        var response = await client.PostAsJsonAsync("/api/teams/list", new { page = 1, pageSize = 10 });

        // Assert - should not be forbidden
        Assert.NotEqual(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task AdminEndpoint_AllowsRootRole()
    {
        // Arrange
        using var factory = new IdentityApplicationFactory(
            _container.ConnectionString,
            options =>
            {
                options.UserId = TestFixtureIds.Ids.TestUser1;
                options.Gid = TestFixtureIds.Gids.TestUser1;
                options.Email = "root@example.com";
                options.Roles = [Roles.Root];
            });
        using var client = factory.CreateClient();

        // Act
        var response = await client.PostAsJsonAsync("/api/teams/list", new { page = 1, pageSize = 10 });

        // Assert - Root should have access
        Assert.NotEqual(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task AdminEndpoint_DeniesUserRole()
    {
        // Arrange
        using var factory = new IdentityApplicationFactory(
            _container.ConnectionString,
            options =>
            {
                options.UserId = TestFixtureIds.Ids.TestUser2;
                options.Gid = TestFixtureIds.Gids.TestUser2;
                options.Email = "user@example.com";
                options.Roles = [Roles.User];
            });
        using var client = factory.CreateClient();

        // Act
        var response = await client.PostAsJsonAsync("/api/teams/list", new { page = 1, pageSize = 10 });

        // Assert - User role should be forbidden from Teams endpoint
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task SbuReadEndpoint_AllowsOpsRole()
    {
        // Arrange - SBU read is allowed for Admin, Root, Ops
        using var factory = new IdentityApplicationFactory(
            _container.ConnectionString,
            options =>
            {
                options.UserId = TestFixtureIds.Ids.TestUser2;
                options.Gid = TestFixtureIds.Gids.TestUser2;
                options.Email = "ops@example.com";
                options.Roles = [Roles.Ops];
            });
        using var client = factory.CreateClient();

        // Act
        var response = await client.PostAsJsonAsync("/api/sbu/list", new { page = 1, pageSize = 10 });

        // Assert - Ops should have read access to SBU
        Assert.NotEqual(HttpStatusCode.Forbidden, response.StatusCode);
    }
}
