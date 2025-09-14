using BookReviewApp.Api.Models.Request;
using BookReviewApp.Api.Models.Response;
using BookReviewApp.Core.Models;
using BookReviewApp.Domain.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BookReviewApp.Api.Services;

public class JwtProvider(IConfiguration config, UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
{
    private readonly UserManager<AppUser> _userManager = userManager;
    private readonly SignInManager<AppUser> _signInManager = signInManager;
    private readonly IConfiguration _configuration = config;

    public async Task<Result<LoginResponse>> Authenticate(LoginRequest loginRequest)
    {
        var user = await _userManager.FindByEmailAsync(loginRequest.Email);
        if (user == null)
        {
            return Result<LoginResponse>.CreateFailed("Invalid credentials");
        }

        var result = await _signInManager.CheckPasswordSignInAsync(user, loginRequest.Password, false);
        if (!result.Succeeded)
        {
            return Result<LoginResponse>.CreateFailed("Invalid credentials");
        }

        var issuer = _configuration["JwtConfig:Issuer"];
        var audience = _configuration["JwtConfig:Audience"];
        var key = _configuration["JwtConfig:Key"];
        var tokenValidityMins = _configuration.GetValue<int>("JwtConfig:TokenValidityMins");

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id),
            new Claim(JwtRegisteredClaimNames.Email, user.Email ?? ""),
            new Claim(ClaimTypes.NameIdentifier, user.Id)
        };

        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key!));
        var creds = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(tokenValidityMins),
            signingCredentials: creds
        );

        var response = new LoginResponse
        {
            Email = user.Email!,
            JwtToken = new JwtSecurityTokenHandler().WriteToken(token),
            ExpiresIn = tokenValidityMins * 60
        };

        return Result<LoginResponse>.CreateSuccessful(response, "success");
    }
}