using App.Shared.Tests.Infrastructure;
using Rcp.Api.Infrastructure;
using Rcp.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Rcp.Api.Tests.Infrastructure;

/// <summary>
/// WebApplicationFactory for RCP API integration tests.
/// Configures test database, mock authentication, and mock external services.
/// </summary>
public class RcpApplicationFactory : WebApplicationFactory<Program>
{
    private readonly string _connectionString;
    private readonly Action<TestAuthHandlerOptions>? _configureAuth;

    public RcpApplicationFactory(string connectionString, Action<TestAuthHandlerOptions>? configureAuth = null)
    {
        _connectionString = connectionString;
        _configureAuth = configureAuth;
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Remove existing DbContext registration
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<RcpDbContext>));
            if (descriptor != null)
            {
                services.Remove(descriptor);
            }

            // Add test DbContext with MySQL container connection string
            services.AddDbContext<RcpDbContext>(options =>
            {
                options.UseMySQL(_connectionString);
            });

            // Replace DataSourceClient with mock (no external HTTP calls in tests)
            var dataSourceDescriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(IDataSourceClient));
            if (dataSourceDescriptor != null)
            {
                services.Remove(dataSourceDescriptor);
            }
            services.AddSingleton<IDataSourceClient>(new MockDataSourceClient(TestFixtureIds.Ids.TestUser1));

            // Configure test authentication
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = TestAuthHandler.AuthenticationScheme;
                options.DefaultChallengeScheme = TestAuthHandler.AuthenticationScheme;
            })
            .AddScheme<TestAuthHandlerOptions, TestAuthHandler>(
                TestAuthHandler.AuthenticationScheme,
                options =>
                {
                    _configureAuth?.Invoke(options);
                });
        });

        builder.UseEnvironment("Testing");
    }
}
