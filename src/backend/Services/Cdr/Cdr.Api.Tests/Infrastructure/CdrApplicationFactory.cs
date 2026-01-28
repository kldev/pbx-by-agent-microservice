using App.Shared.Tests.Infrastructure;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using CdrService.Data;

namespace CdrService.Api.Tests.Infrastructure;

public class CdrApplicationFactory : WebApplicationFactory<Program>
{
    private readonly string _connectionString;
    private readonly Action<TestAuthHandlerOptions>? _configureAuth;

    // ReSharper disable once ConvertToPrimaryConstructor
    public CdrApplicationFactory(string connectionString, Action<TestAuthHandlerOptions>? configureAuth = null)
    {
        _connectionString = connectionString;
        _configureAuth = configureAuth;
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Usuń istniejący DbContext
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<CdrDbContext>));
            if (descriptor != null)
                services.Remove(descriptor);

            // Add test DbContext with MySQL container connection string
            services.AddDbContext<CdrDbContext>(options =>
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

    public async Task SeedTestDataAsync()
    {
        using var scope = Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<CdrDbContext>();

        await context.Database.EnsureCreatedAsync();
        await CdrTestDataSeeder.SeedAsync(context);
    }
}
