using DataSource.Data;
using Identity.Data;
using JdJobs.Data;
using JdSales.Data;
using Rcp.Data;
using Microsoft.EntityFrameworkCore;
using Testcontainers.MySql;
using Xunit;

namespace DataSource.Api.Tests.Infrastructure;

/// <summary>
/// MySQL test container that creates ALL microservice databases.
/// Used for testing DataSource views which span multiple databases.
/// </summary>
public class AllDatabasesTestContainer : IAsyncLifetime
{
    private readonly MySqlContainer _container;

    // Database names matching the view SQL files
    private const string IdentityDb = "jd_identity";
    private const string JdJobsDb = "jd_jobs";
    private const string JdSalesDb = "jd_sales";
    private const string RcpDb = "rcp";
    private const string DataSourceDb = "datasource";

    public string ConnectionString { get; private set; } = string.Empty;

    public AllDatabasesTestContainer()
    {
        _container = new MySqlBuilder()
            .WithImage("mysql:8.0")
            .WithDatabase(DataSourceDb)
            .WithUsername("root")
            .WithPassword("Test@123!")
            .Build();
    }

    public async Task InitializeAsync()
    {
        await _container.StartAsync();
        ConnectionString = _container.GetConnectionString();

        // Create all databases
        await CreateDatabasesAsync();

        // Apply migrations for each microservice
        await ApplyIdentityMigrationsAsync();
        await ApplyJdJobsMigrationsAsync();
        await ApplyJdSalesMigrationsAsync();
        await ApplyRcpMigrationsAsync();

        // Create DataSource views
        await CreateDataSourceViewsAsync();
    }

    private async Task CreateDatabasesAsync()
    {
        var connectionString = ConnectionString.Replace($"Database={DataSourceDb}", "Database=mysql");
        await using var connection = new MySql.Data.MySqlClient.MySqlConnection(connectionString);
        await connection.OpenAsync();

        var databases = new[] { IdentityDb, JdJobsDb, JdSalesDb, RcpDb };
        foreach (var db in databases)
        {
            await using var cmd = connection.CreateCommand();
            cmd.CommandText = $"CREATE DATABASE IF NOT EXISTS `{db}` CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;";
            await cmd.ExecuteNonQueryAsync();
        }
    }

    private async Task ApplyIdentityMigrationsAsync()
    {
        var options = new DbContextOptionsBuilder<IdentityDbContext>()
            .UseMySQL(GetConnectionString(IdentityDb))
            .Options;

        await using var context = new IdentityDbContext(options);
        await context.Database.MigrateAsync();

        // Seed basic data
        IdentityTestSeeder.SeedMinimalData(context);
        await context.SaveChangesAsync();
    }

    private async Task ApplyJdJobsMigrationsAsync()
    {
        var options = new DbContextOptionsBuilder<JdJobsDbContext>()
            .UseMySQL(GetConnectionString(JdJobsDb))
            .Options;

        await using var context = new JdJobsDbContext(options);
        await context.Database.MigrateAsync();

        // Seed dictionaries
        JdJobsTestSeeder.SeedMinimalData(context);
        await context.SaveChangesAsync();
    }

    private async Task ApplyJdSalesMigrationsAsync()
    {
        var options = new DbContextOptionsBuilder<JdSalesDbContext>()
            .UseMySQL(GetConnectionString(JdSalesDb))
            .Options;

        await using var context = new JdSalesDbContext(options);
        await context.Database.MigrateAsync();
    }

    private async Task ApplyRcpMigrationsAsync()
    {
        var options = new DbContextOptionsBuilder<RcpDbContext>()
            .UseMySQL(GetConnectionString(RcpDb))
            .Options;

        await using var context = new RcpDbContext(options);
        await context.Database.MigrateAsync();
    }

    private async Task CreateDataSourceViewsAsync()
    {
        var options = new DbContextOptionsBuilder<DataSourceDbContext>()
            .UseMySQL(GetConnectionString(DataSourceDb))
            .Options;

        await using var context = new DataSourceDbContext(options);

        // Read and execute all view SQL files
        var assembly = typeof(DataSourceDbContext).Assembly;
        var resourcePrefix = "DataSource.Data.Resources.Views.";

        var viewResources = assembly.GetManifestResourceNames()
            .Where(r => r.StartsWith(resourcePrefix) && r.EndsWith(".sql"))
            .OrderBy(r => r)
            .ToList();

        foreach (var resourceName in viewResources)
        {
            using var stream = assembly.GetManifestResourceStream(resourceName);
            if (stream == null) continue;

            using var reader = new StreamReader(stream);
            var sql = await reader.ReadToEndAsync();

            try
            {
                await context.Database.ExecuteSqlRawAsync(sql);
            }
            catch (Exception ex)
            {
                // Log but continue - some views might depend on others
                Console.WriteLine($"Warning: Failed to create view from {resourceName}: {ex.Message}");
            }
        }
    }

    private string GetConnectionString(string database)
    {
        return ConnectionString.Replace($"Database={DataSourceDb}", $"Database={database}");
    }

    public string GetDataSourceConnectionString() => GetConnectionString(DataSourceDb);

    public async Task DisposeAsync()
    {
        await _container.StopAsync();
        await _container.DisposeAsync();
    }
}
