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
    TokenResponse GenerateToken(User user);
}