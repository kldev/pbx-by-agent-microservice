using App.Shared.Web;
using Identity.Api.Features.AppUsers;
using Identity.Api.Features.Auth;
using Identity.Api.Features.Structure;
using Identity.Api.Features.Teams;
using Identity.Api.Seed;
using Identity.Data;
using Microsoft.EntityFrameworkCore;

namespace Identity.Api.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Database
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        services.AddDbContext<IdentityDbContext>(options =>
        {
            options.UseMySQL(connectionString!);
        });

        // Auth feature
        services.AddScoped<IAuthService, AuthService>();

        // Structure feature
        services.AddScoped<IStructureService, StructureService>();
        services.AddScoped<IStructureDataHandler, StructureDataHandler>();

        // Team feature
        services.AddScoped<ITeamService, TeamService>();
        services.AddScoped<ITeamDataHandler, TeamDataHandler>();

        // AppUser feature
        services.AddScoped<IAppUserService, AppUserService>();
        services.AddScoped<IAppUserDataHandler, AppUserDataHandler>();

        // Seed
        services.Configure<SeedSettings>(configuration.GetSection("Seed"));
        services.AddScoped<ISeedService, SeedService>();

        return services;
    }
}
