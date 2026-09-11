using BackendTZ.DTOs.Responses;
using BackendTZ.DTOs.Requests;
using BackendTZ.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BackendTZ.Controllers;

/// <summary>
/// Controller responsible for handling authentication-related operations such as registration, login, logout, and token refresh.
/// </summary>
/// <param name="authService">The authentication service used to perform authentication operations.</param>
[ApiController]
[Route("api/auth")]
public class AuthController(IAuthService authService) : BaseController
{
    private const string RefreshTokenCookieName = "refreshToken";

    /// <summary>
    /// Registers a new user and returns a token response.
    /// </summary>
    /// <param name="request">The registration request containing user details.</param>
    /// <param name="ct">A token to cancel the operation.</param>
    /// <returns>The token response containing access and refresh tokens.</returns>
    [HttpPost("register")]
    [ProducesResponseType(typeof(TokenResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<TokenResponse>> Register(RegisterRequest request, CancellationToken ct)
    {
        var response = await authService.RegisterAsync(request, ct);

        SetRefreshTokenCookie(response);

        return StatusCode(StatusCodes.Status201Created, response);
    }

    /// <summary>
    /// Authenticates a user and returns a token response.
    /// </summary>
    /// <param name="request">The login request containing user credentials.</param>
    /// <param name="ct">A token to cancel the operation.</param>
    /// <returns>The token response containing access and refresh tokens.</returns>
    [HttpPost("login")]
    [ProducesResponseType(typeof(TokenResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<TokenResponse>> Login(LoginRequest request, CancellationToken ct)
    {
        var response = await authService.LoginAsync(request, ct);
        
        SetRefreshTokenCookie(response);

        return Ok(response);
    }

    /// <summary>
    /// Logs out the user by invalidating the refresh token and deleting the refresh token cookie.
    /// </summary>
    /// <param name="ct">A token to cancel the operation.</param>
    /// <returns>No content.</returns>
    [HttpPost("logout")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Logout(CancellationToken ct)
    {
        var refreshToken = Request.Cookies[RefreshTokenCookieName];
        
        if (string.IsNullOrEmpty(refreshToken))
        {
            return BadRequest(new { message = "Refresh token cookie is missing." });
        }

        await authService.LogoutAsync(refreshToken, ct);

        Response.Cookies.Delete(RefreshTokenCookieName);

        return NoContent();
    }

    /// <summary>
    /// Refreshes the access token using the provided refresh token.
    /// </summary>
    /// <param name="request">The refresh request containing the refresh token.</param>
    /// <param name="ct">A token to cancel the operation.</param>
    /// <returns>The token response containing the new access and refresh tokens.</returns>
    [HttpPost("refresh")]
    [ProducesResponseType(typeof(TokenResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<TokenResponse>> Refresh(RefreshRequest request, CancellationToken ct)
    {
        var response = await authService.RefreshAccessTokenAsync(request.RefreshToken, ct);
        return Ok(response);
    }

    private void SetRefreshTokenCookie(TokenResponse response)
    {
        Response.Cookies.Append(RefreshTokenCookieName, response.RefreshToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Path = "/api/auth/refresh",
            Expires = response.ExpiresAt
        });
    }
}