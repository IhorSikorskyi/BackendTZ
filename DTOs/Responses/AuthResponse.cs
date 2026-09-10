using BackendTZ.Entities;

namespace BackendTZ.DTOs.Responses;

/// <summary>
/// Represents the base class for authentication responses.
/// </summary>
public abstract record AuthResponse;

/// <summary>
/// Response payload returned after successful registration or login,
/// containing the issued access token and basic user information.
/// </summary>
/// <param name="AccessToken">The JWT access token to be used for authenticated requests.</param>
/// <param name="ExpiresAt">The UTC date and time when the access token expires.</param>
/// <param name="RefreshToken">The refresh token to be used for obtaining a new access token.</param>
/// <param name="User">Basic information about the authenticated user.</param>
public record TokenResponse(
    string AccessToken,
    DateTime ExpiresAt,
    string RefreshToken,
    UserResponse User
) : AuthResponse;

/// <summary>
/// Represents public-facing information about a user, safe to expose in API responses.
/// </summary>
/// <param name="Id">The unique identifier of the user.</param>
/// <param name="Name">The name of the user.</param>
/// <param name="Email">The email address of the user.</param>
/// <param name="Role">The role assigned to the user.</param>
public record UserResponse(
    Guid Id,
    string Name,
    string Email,
    Role Role
);