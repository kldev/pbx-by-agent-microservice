using App.Shared.Tests.Infrastructure;
using Identity.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Identity.Api.Tests.Infrastructure;

/// <summary>
/// WebApplicationFactory for Identity API integration tests.
/// Configures test database and mock authentication.
/// </summary>
public class IdentityApplicationFactory : WebApplicationFactory<Program>
{
    private readonly string _connectionString;
    private readonly Action<TestAuthHandlerOptions>? _configureAuth;

    public IdentityApplicationFactory(string connectionString, Action<TestAuthHandlerOptions>? configureAuth = null)
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
                d => d.ServiceType == typeof(DbContextOptions<IdentityDbContext>));
            if (descriptor != null)
            {
                services.Remove(descriptor);
            }

            // Add test DbContext with MySQL container connection string
            services.AddDbContext<IdentityDbContext>(options =>
            {
                options.UseMySQL(_connectionString);
            });

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
