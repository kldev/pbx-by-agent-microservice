using AnswerRule.Data;
using App.Shared.Tests.Infrastructure;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace AnswerRule.Api.Tests.Infrastructure;

public class AnswerRuleApplicationFactory : WebApplicationFactory<Program>
{
    private readonly string _connectionString;
    private readonly Action<TestAuthHandlerOptions>? _configureAuth;

    public AnswerRuleApplicationFactory(string connectionString, Action<TestAuthHandlerOptions>? configureAuth = null)
    {
        _connectionString = connectionString;
        _configureAuth = configureAuth;
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Remove existing DbContext
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<AnswerRuleDbContext>));
            if (descriptor != null)
                services.Remove(descriptor);

            // Add test DbContext with MySQL container connection string
            services.AddDbContext<AnswerRuleDbContext>(options =>
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
