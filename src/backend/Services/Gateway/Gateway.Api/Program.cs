using System.Text;
using System.Text.Json.Serialization;
using App.Shared.Web.Swagger;
using Gateway.Api.Auth;
using Gateway.Api.Configuration;
using Gateway.Api.Swagger;
using Gateway.Api.Transforms;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// CORS - allow frontend origins (configurable via CORS_ORIGINS env var)
var corsOrigins = builder.Configuration["CORS_ORIGINS"]?
    .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
    ?? ["http://localhost:4300", "http://localhost:5173"];

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins(corsOrigins)
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

// JWT Settings
var jwtSettings = builder.Configuration.GetSection(JwtSettings.SectionName).Get<JwtSettings>()
    ?? throw new InvalidOperationException("JWT settings not configured");

builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection(JwtSettings.SectionName));

// Add JWT Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings.Issuer,
        ValidAudience = jwtSettings.Audience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Secret)),
        ClockSkew = TimeSpan.Zero
    };
});

// Configure JSON to serialize enums as strings
builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

builder.Services.AddAuthorization();

// Auth services
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddHttpClient<IIdentityClient, IdentityClient>(client =>
{
    var identityAddress = builder.Configuration["ReverseProxy:Clusters:identity-cluster:Destinations:identity-destination:Address"]
        ?? "http://identity:8080";
    client.BaseAddress = new Uri(identityAddress);
    client.Timeout = TimeSpan.FromSeconds(30);
});

// HttpClient for swagger aggregation
builder.Services.AddHttpClient("SwaggerClient", client =>
{
    client.Timeout = TimeSpan.FromSeconds(10);
});
builder.Services.AddScoped<SwaggerAggregator>();

// Add YARP Reverse Proxy with transforms
builder.Services
    .AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"))
    .AddTransforms<UserHeadersTransformProvider>();

// Swagger for Gateway endpoints (health, etc.)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "Gateway API", Version = "v1" });
    c.SchemaFilter<EnumSchemaFilter>();
});

var app = builder.Build();

// Swagger UI
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Gateway");
    c.SwaggerEndpoint("/api-docs/identity/swagger.json", "Identity API");
    c.SwaggerEndpoint("/api-docs/rate/swagger.json", "Rate API");
    c.SwaggerEndpoint("/api-docs/datasource/swagger.json", "DataSource API");
    c.SwaggerEndpoint("/api-docs/rcp/swagger.json", "RCP (Timesheets) API");
    c.SwaggerEndpoint("/api-docs/cdr/swagger.json", "CDR (Call data records) API");
    c.SwaggerEndpoint("/api-docs/answerrule/swagger.json", "Answer Rule API");
});

// CORS must be before Authentication
app.UseCors();

// Authentication & Authorization
app.UseAuthentication();
app.UseAuthorization();

// Health check endpoint
app.MapGet("/health", () => Results.Ok("Healthy"))
    .WithTags("Health")
    .ExcludeFromDescription();

app.MapGet("/", 
    () => Results.Ok("Hi, PBX by Agent API Claude Code")
    ).ExcludeFromDescription();

// Auth endpoints (login handled by Gateway)
app.MapAuthEndpoints();

// Swagger aggregation endpoints
app.MapSwaggerEndpoints();

// Map YARP reverse proxy
app.MapReverseProxy();

app.Run();
