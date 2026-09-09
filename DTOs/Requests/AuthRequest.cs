using System.ComponentModel.DataAnnotations;

namespace BackendTZ.DTOs.Requests;

/// <summary>
/// Represents the base class for authentication requests.
/// </summary>
public abstract record AuthRequest;

/// <summary>
/// Request payload for registering a new user account.
/// </summary>
/// <param name="Name">The name of the user.</param>
/// <param name="Email">The email address of the user.</param>
/// <param name="Number">The phone number of the user.</param>
/// <param name="Password">The password for the user account.</param>
/// <param name="ConfirmPassword">The confirmation of the password.</param>
public record RegisterRequest(
    [property: Required, MaxLength(100)]
    string Name,

    [property: Required, EmailAddress, MaxLength(100)]
    string Email,

    [property: Phone, MaxLength(20)]
    string? Number,

    [property: Required, MinLength(8), MaxLength(100)]
    string Password,

    [property: Required]
    string ConfirmPassword
) : AuthRequest;

/// <summary>
/// Request payload for logging in an existing user account.
/// </summary>
/// <param name="Email"></param>
/// <param name="Password"></param>
public record LoginRequest(
    [property: Required, EmailAddress, MaxLength(100)]
    string Email,

    [property: Required, MinLength(8), MaxLength(100)]
    string Password
) : AuthRequest;