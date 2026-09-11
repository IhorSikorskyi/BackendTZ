using BackendTZ.DTOs.Responses;
using BackendTZ.Entities;

namespace BackendTZ.Services.Interfaces;

/// <summary>
/// Defines the contract for generating JWT tokens for authenticated users.
/// </summary>
public interface IJwtTokenGenerator
{
    /// <summary>
    /// Generates a JWT token for the specified user.
    /// </summary>
    /// <param name="user">The user for whom to generate the token.</param>
    /// <returns>A JWT token as a string.</returns>
    AccessTokenResult GenerateAccessToken(User user);
}

/// <summary>
/// Represents the result of generating an access token, including the token itself and its expiration time.
/// </summary>
/// <param name="AccessToken">The generated access token.</param>
/// <param name="ExpiresAt">The expiration time of the access token.</param>
public record AccessTokenResult(string AccessToken, DateTime ExpiresAt);