using DataSource.Api.Tests.Infrastructure;
using DataSource.Data;
using Microsoft.EntityFrameworkCore;
using Xunit;
using Xunit.Abstractions;

namespace DataSource.Api.Tests;

/// <summary>
/// Tests that verify DataSource views work correctly across all microservice databases.
/// </summary>
[Collection(DataSourceDatabaseCollection.Name)]
public class DataSourceViewsTests
{
    private readonly AllDatabasesTestContainer _container;
    private readonly ITestOutputHelper _output;

    // ReSharper disable once ConvertToPrimaryConstructor
    public DataSourceViewsTests(AllDatabasesTestContainer container, ITestOutputHelper output)
    {
        _container = container;
        _output = output;
    }

    private DataSourceDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<DataSourceDbContext>()
            .UseMySQL(_container.GetDataSourceConnectionString())
            .Options;

        return new DataSourceDbContext(options);
    }

    [Fact]
    public async Task AllMigrations_AreApplied_Successfully()
    {
        // This test passes if container initialization succeeded
        // (migrations are applied in InitializeAsync)
        Assert.NotEmpty(_container.ConnectionString);
        _output.WriteLine("All microservice migrations applied successfully");
    }

    [Fact]
    public async Task VwUsersAll_ReturnsSeededUsers()
    {
        await using var context = CreateContext();

        var users = await context.UsersAll.ToListAsync();

        _output.WriteLine($"Found {users.Count} users in vw_users_all");
        foreach (var user in users)
        {
            _output.WriteLine($"  - {user.Label} ({user.SubLabel})");
        }

        Assert.NotEmpty(users);
        Assert.Contains(users, u => u.Label.Contains("Kowalski"));
    }

    [Fact]
    public async Task VwBenefits_ReturnsSeededBenefits()
    {
        await using var context = CreateContext();

        var benefits = await context.Benefits.ToListAsync();

        _output.WriteLine($"Found {benefits.Count} benefits in vw_benefits");
        foreach (var benefit in benefits)
        {
            _output.WriteLine($"  - {benefit.Label} ({benefit.Category})");
        }

        Assert.NotEmpty(benefits);
    }

    [Fact]
    public async Task VwTools_ReturnsSeededTools()
    {
        await using var context = CreateContext();

        var tools = await context.Tools.ToListAsync();

        _output.WriteLine($"Found {tools.Count} tools in vw_tools");
        Assert.NotEmpty(tools);
    }

    [Fact]
    public async Task VwCertificates_ReturnsSeededCertificates()
    {
        await using var context = CreateContext();

        var certificates = await context.Certificates.ToListAsync();

        _output.WriteLine($"Found {certificates.Count} certificates in vw_certificates");
        Assert.NotEmpty(certificates);
    }

    [Fact]
    public async Task VwTraits_ReturnsSeededTraits()
    {
        await using var context = CreateContext();

        var traits = await context.Traits.ToListAsync();

        _output.WriteLine($"Found {traits.Count} traits in vw_traits");
        Assert.NotEmpty(traits);
    }

    [Fact]
    public async Task VwPositions_ReturnsSeededPositions()
    {
        await using var context = CreateContext();

        var positions = await context.Positions.ToListAsync();

        _output.WriteLine($"Found {positions.Count} positions in vw_positions");
        Assert.NotEmpty(positions);
    }

    [Fact]
    public async Task VwTeams_ReturnsSeededTeams()
    {
        await using var context = CreateContext();

        var teams = await context.Teams.ToListAsync();

        _output.WriteLine($"Found {teams.Count} teams in vw_teams");
        Assert.NotEmpty(teams);
    }

    [Fact]
    public async Task VwSbu_ReturnsSeededSbus()
    {
        await using var context = CreateContext();

        var sbus = await context.Sbu.ToListAsync();

        _output.WriteLine($"Found {sbus.Count} SBUs in vw_sbu");
        Assert.NotEmpty(sbus);
    }

    [Fact]
    public async Task VwUserSubordinates_ReturnsUsersWithSupervisors()
    {
        await using var context = CreateContext();

        var users = await context.UserSubordinates.ToListAsync();

        _output.WriteLine($"Found {users.Count} users in vw_user_subordinates");
        foreach (var user in users)
        {
            _output.WriteLine($"  - User: {user.Label} (RecordId: {user.RecordId}), Supervisor: {user.SupervisorId}");
        }

        // We seeded Anna with Jan as supervisor
        Assert.NotEmpty(users);
        Assert.Contains(users, u => u.SupervisorId != null);
    }
}
