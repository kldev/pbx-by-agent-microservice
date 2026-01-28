using Microsoft.EntityFrameworkCore;
using CdrService.Api.Features.CallRecords;
using CdrService.Api.Features.CallStatuses;
using CdrService.Api.Features.TerminationCauses;
using CdrService.Api.Features.CallDirections;
using CdrService.Api.Seed;
using CdrService.Data;

namespace CdrService.Api.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Database
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        services.AddDbContext<CdrDbContext>(options =>
        {
            options.UseMySQL(connectionString!);
        });

        // CallRecords
        services.AddScoped<ICallRecordDataHandler, CallRecordDataHandler>();
        services.AddScoped<ICallRecordService, CallRecordService>();

        // CallStatuses
        services.AddScoped<ICallStatusDataHandler, CallStatusDataHandler>();
        services.AddScoped<ICallStatusService, CallStatusService>();

        // TerminationCauses
        services.AddScoped<ITerminationCauseDataHandler, TerminationCauseDataHandler>();
        services.AddScoped<ITerminationCauseService, TerminationCauseService>();

        // CallDirections
        services.AddScoped<ICallDirectionDataHandler, CallDirectionDataHandler>();
        services.AddScoped<ICallDirectionService, CallDirectionService>();

        // Seed
        services.AddScoped<ISeedService, SeedService>();

        return services;
    }
}
