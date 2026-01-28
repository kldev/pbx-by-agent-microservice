namespace Gateway.Api.Auth;

public static class AuthEndpoint
{
    public static void MapAuthEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/auth")
            .WithTags("Auth");

        group.MapPost("/login", Login)
            .WithName("Login")
            .Produces<LoginResponse>()
            .Produces<ApiErrorResponse>(400)
            .AllowAnonymous();
    }

    private static async Task<IResult> Login(
        LoginRequest request,
        IIdentityClient identityClient,
        IJwtService jwtService)
    {
        if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
        {
            return Results.BadRequest(new ApiErrorResponse
            {
                Code = "VALIDATION_ERROR",
                Message = "Email and password are required"
            });
        }

        var validationResult = await identityClient.ValidateLoginAsync(request);

        if (validationResult == null)
        {
            return Results.BadRequest(new ApiErrorResponse
            {
                Code = "SERVICE_ERROR",
                Message = "Unable to validate credentials"
            });
        }

        if (!validationResult.IsValid)
        {
            return Results.BadRequest(new ApiErrorResponse
            {
                Code = validationResult.ErrorCode ?? "AUTH_ERROR",
                Message = validationResult.ErrorMessage ?? "Authentication failed"
            });
        }

        var token = jwtService.GenerateToken(validationResult);
        var expiresAt = jwtService.GetExpiration();

        return Results.Ok(new LoginResponse(
            Token: token,
            Gid: validationResult.Gid!,
            Email: validationResult.Email!,
            FirstName: validationResult.FirstName!,
            LastName: validationResult.LastName!,
            Roles: validationResult.Roles ?? [],
            ExpiresAt: expiresAt
        ));
    }
}
