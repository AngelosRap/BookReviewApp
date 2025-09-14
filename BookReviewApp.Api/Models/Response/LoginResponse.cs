using Swashbuckle.AspNetCore.Annotations;

namespace BookReviewApp.Api.Models.Response;

[SwaggerSchema("Response returned after a successful login", Title = "LoginResponse")]
public class LoginResponse
{
    [SwaggerSchema("Email of the authenticated user")]
    public string? Email { get; set; }

    [SwaggerSchema("JWT token for authentication")]
    public string? JwtToken { get; set; }

    [SwaggerSchema("Token expiration time in seconds")]
    public int ExpiresIn { get; set; }
}
