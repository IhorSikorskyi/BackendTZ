using BackendTZ.DTOs.Requests;
using BackendTZ.DTOs.Responses;

namespace BackendTZ.Services.Interfaces;

/// <summary>
/// Defines the contract for authentication-related operations.
/// </summary>
public interface IAuthService
{
    /// <summary>
    /// Registers a new user account.
    /// </summary>
    /// <param name="request">The registration request containing user details.</param>
    /// <param name="ct">A cancellation token.</param>
    /// <returns>A <see cref="TokenResponse"/> containing the access token and user information.</returns>
    Task<TokenResponse> RegisterAsync(RegisterRequest request, CancellationToken ct = default);

    /// <summary>
    /// Logs in an existing user account.
    /// </summary>
    /// <param name="request">The login request containing user credentials.</param>
    /// <param name="ct">A cancellation token.</param>
    /// <returns>A <see cref="TokenResponse"/> containing the access token and user information.</returns>
    Task<TokenResponse> LoginAsync(LoginRequest request, CancellationToken ct = default);

    /// <summary>
    /// Logs out the currently authenticated user.
    /// </summary>
    /// <param name="ct">A cancellation token.</param>
    /// <returns>A <see cref="TokenResponse"/> containing the access token and user information.</returns>
    Task<TokenResponse> LogoutAsync(CancellationToken ct = default);
}