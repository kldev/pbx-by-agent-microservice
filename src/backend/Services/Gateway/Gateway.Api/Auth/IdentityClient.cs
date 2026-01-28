using System.Text;
using System.Text.Json;
using Common.Toolkit.Json;

namespace Gateway.Api.Auth;

public interface IIdentityClient
{
    Task<ValidateLoginResponse?> ValidateLoginAsync(LoginRequest request);
}

public class IdentityClient : IIdentityClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<IdentityClient> _logger;
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public IdentityClient(HttpClient httpClient, ILogger<IdentityClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<ValidateLoginResponse?> ValidateLoginAsync(LoginRequest request)
    {
        try
        {
            var json = JsonSerializer.Serialize(request);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("/api/auth/validate-login", content);
            var responseContent = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Identity validation failed with status {StatusCode}: {Content}",
                    response.StatusCode, responseContent);
                return null;
            }

            return JsonUtil.Read<ValidateLoginResponse?>(responseContent);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calling Identity service for login validation");
            return null;
        }
    }
}
