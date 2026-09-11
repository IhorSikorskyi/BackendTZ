using BackendTZ.Entities;
using BackendTZ.Services.Interfaces;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BackendTZ.Services.Implementations;

/// <summary>
/// Represents a service for generating JWT access tokens and associated refresh tokens for authenticated users.
/// </summary>
/// <param name="configuration">The application configuration.</param>
public class JwtTokenGenerator(IConfiguration configuration) : IJwtTokenGenerator
{
    /// <inheritdoc/>
    public AccessTokenResult GenerateAccessToken(User user)
    {
        var expiresAt = DateTime.UtcNow.AddMinutes(
            Convert.ToDouble(configuration["AppSettings:ExpirationTime"]));

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Email, user.Email),
            new(ClaimTypes.Role, user.Role.ToString())
        };

        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(
                configuration["AppSettings:AccessToken"]
                ?? throw new InvalidOperationException("AppSettings:AccessToken не налаштовано.")));

        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: configuration["AppSettings:Issuer"],
            audience: configuration["AppSettings:Audience"],
            claims: claims,
            expires: expiresAt,
            signingCredentials: credentials);

        var accessToken = new JwtSecurityTokenHandler().WriteToken(token);

        return new AccessTokenResult(accessToken, expiresAt);
    }
}