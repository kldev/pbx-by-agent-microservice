using DataSource.Api.Features.DataSource;
using DataSource.Data;
using Microsoft.EntityFrameworkCore;

namespace DataSource.Api.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Database
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        services.AddDbContext<DataSourceDbContext>(options =>
        {
            options.UseMySQL(connectionString!);
        });

        // DataSource feature
        services.AddScoped<IDataSourceService, DataSourceService>();

        return services;
    }
}
