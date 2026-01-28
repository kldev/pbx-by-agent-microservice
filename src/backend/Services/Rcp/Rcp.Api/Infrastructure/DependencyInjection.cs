using Rcp.Api.Features.TimeEntry;
using Rcp.Data;
using Microsoft.EntityFrameworkCore;

namespace Rcp.Api.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Database
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        services.AddDbContext<RcpDbContext>(options =>
        {
            options.UseMySQL(connectionString!);
        });

        // DataSource HTTP client (for subordinates lookup)
        var dataSourceUrl = configuration["Services:DataSource"] ?? "http://datasource:8080";
        services.AddHttpClient<IDataSourceClient, DataSourceClient>(client =>
        {
            client.BaseAddress = new Uri(dataSourceUrl);
            client.Timeout = TimeSpan.FromSeconds(30);
        });

        // TimeEntry feature
        services.AddScoped<IRcpDataHandler, RcpDataHandler>();
        services.AddScoped<IRcpService, RcpService>();
        services.AddScoped<IRcpExcelExportService, RcpExcelExportService>();

        return services;
    }
}
