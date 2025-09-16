using BookReviewApp.Api.Models.Request;
using BookReviewApp.Api.Models.Response;
using BookReviewApp.Api.Services;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace BookReviewApp.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(JwtProvider jwtProvider) : ControllerBase
{
    private readonly JwtProvider _jwtProvider = jwtProvider;

    [HttpPost("login")]
    [SwaggerOperation(
        Summary = "User login",
        Description = "Authenticate user with email and password and return JWT token with expiration."
    )]
    [SwaggerResponse(StatusCodes.Status200OK, "Login successful", typeof(LoginResponse))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid request")]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "Invalid email or password")]
    public async Task<IActionResult> Login([FromBody] LoginRequest model)
    {
        var loginResponseRes = await _jwtProvider.Authenticate(model);

        return loginResponseRes.Failed ? Unauthorized(loginResponseRes.Message) : Ok(loginResponseRes.Data);
    }
}