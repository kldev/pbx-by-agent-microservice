using System.Text.Json.Serialization;
using App.Shared.Web.Middleware;
using App.Shared.Web.Security;
using App.Shared.Web.Swagger;
using Rcp.Api.Features;
using Rcp.Api.Infrastructure;
using Microsoft.AspNetCore.Authentication;

var builder = WebApplication.CreateBuilder(args);

// Configure JSON to serialize enums as strings
builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

// Add services to the container (with enum strings in Swagger)
builder.Services.AddSwaggerWithEnumStrings();

// Gateway authentication (reads X-User-* headers)
builder.Services.AddAuthentication(GatewayAuthHandler.SchemeName)
    .AddScheme<AuthenticationSchemeOptions, GatewayAuthHandler>(GatewayAuthHandler.SchemeName, null);
builder.Services.AddAuthorization();

// Application services (DbContext, features)
builder.Services.AddApplicationServices(builder.Configuration);

var app = builder.Build();

// Database migration on startup
await app.InitializeDatabaseAsync();

app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthentication();
app.UseAuthorization();
app.UseRequestLogging();

// Map endpoints
app.MapAppEndpoints();
app.Map("/health", () => Results.Ok("Healthy")).ExcludeFromDescription();

app.Run();

// Required for WebApplicationFactory in tests
public partial class Program { }
