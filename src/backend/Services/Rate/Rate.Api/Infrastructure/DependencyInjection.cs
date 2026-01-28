using Microsoft.EntityFrameworkCore;
using RateService.Api.Features.DestinationGroups;
using RateService.Api.Features.Rates;
using RateService.Api.Features.Tariffs;
using RateService.Api.Seed;
using RateService.Data;

namespace RateService.Api.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Database
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        services.AddDbContext<RateServiceDbContext>(options =>
        {
            options.UseMySQL(connectionString!);
        });

        
        // Tariffs
        services.AddScoped<ITariffDataHandler, TariffDataHandler>();
        services.AddScoped<ITariffService, TariffService>();

        // Rates
        services.AddScoped<IRateDataHandler, RateDataHandler>();
        services.AddScoped<IRateService, Features.Rates.RateService>();

        // DestinationGroups
        services.AddScoped<IDestinationGroupDataHandler, DestinationGroupDataHandler>();
        services.AddScoped<IDestinationGroupService, DestinationGroupService>();

        // Seed
        services.AddScoped<ISeedService, SeedService>();

        return services;
    }
}
