using App.Shared.Web;
using FinCosts.Api.Features.Costs;
using FinCosts.Api.Seed;
using FinCosts.Data;
using Microsoft.EntityFrameworkCore;
using ISeedService = FinCosts.Api.Seed.ISeedService;


namespace FinCosts.Api.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<SeedSettings>(configuration.GetSection(SeedSettings.SectionName));
        
        // Database
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        services.AddDbContext<FinCostsDbContext>(options =>
        {
            options.UseMySQL(connectionString!);
        });
        
        // Costs Service 
        // not data handler yet
        services.AddScoped<ICostsService, CostsService>();
        services.AddScoped<ISeedService, SeedService>();
        
        
        
        return services;
    }
}
