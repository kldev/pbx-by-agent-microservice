using Common.Toolkit.ResultPattern;
using Identity.Api.Features.Auth.Model;

namespace Identity.Api.Features.Auth;

public interface IAuthService
{
    Task<ValidateLoginResponse> ValidateLoginAsync(LoginRequest request);
    Task<Result<MeResponse>> GetMeAsync(string userGid);
    Task<Result<JdPrefillResponse>> GetJdPrefillAsync(string userGid);
}
