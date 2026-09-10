using BackendTZ.DTOs.Requests;
using BackendTZ.DTOs.Responses;
using BackendTZ.Entities;
using BackendTZ.Exceptions;
using BackendTZ.Repositories.Interfaces;
using BackendTZ.Services.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace BackendTZ.Services.Implementations;

/// <summary>
/// Represents the implementation of the IAuthService interface for handling user authentication and authorization.
/// </summary>
/// <param name="userRepository">The repository for managing user data.</param>
/// <param name="refreshTokenService">The service for managing refresh tokens.</param>
/// <param name="unitOfWork">The unit of work for managing transactions.</param>
/// <param name="jwtTokenGenerator">The service for generating JWT access tokens.</param>
/// <param name="passwordHasher"></param>
public class AuthService(
    IUserRepository userRepository,
    IRefreshTokenService refreshTokenService,
    IUnitOfWork unitOfWork,
    IJwtTokenGenerator jwtTokenGenerator,
    IPasswordHasher<User> passwordHasher) : IAuthService
{
    /// <inheritdoc/>
    public async Task<TokenResponse> RegisterAsync(RegisterRequest request, CancellationToken ct = default)
    {
        if (await userRepository.IsExistByEmailAsync(request.Email, ct))
        {
            throw new InvalidOperationException("User with this email already exists.");
        }

        if (request.Password != request.ConfirmPassword)
        {
            throw new InvalidOperationException("Passwords do not match.");
        }

        var user = new User
        {
            Name = request.Name,
            Email = request.Email,
            Number = request.Number,
            PasswordHash = string.Empty
        };

        user.PasswordHash = passwordHasher.HashPassword(user, request.Password);

        await userRepository.AddAsync(user, ct);
        await unitOfWork.SaveChangesAsync(ct);

        return await IssueTokenResponseAsync(user, ct);
    }

    /// <inheritdoc/>
    public async Task<TokenResponse> LoginAsync(LoginRequest request, CancellationToken ct = default)
    {
        var user = await userRepository.GetUserByEmailAsync(request.Email, ct)
                   ?? throw new UnauthorizedAccessException("Invalid email or password.");

        var result = passwordHasher.VerifyHashedPassword(user, user.PasswordHash, request.Password);

        if (result == PasswordVerificationResult.Failed)
        {
            throw new UnauthorizedAccessException("Invalid email or password.");
        }

        return await IssueTokenResponseAsync(user, ct);
    }

    /// <inheritdoc/>
    public async Task<TokenResponse> RefreshAccessTokenAsync(string rawRefreshToken, CancellationToken ct = default)
    {
        var oldToken = await refreshTokenService.ValidateUserRefreshTokenAsync(rawRefreshToken, ct);

        var user = await userRepository.GetByIdAsync(oldToken.UserId, ct)
                   ?? throw new NotFoundException("User not found.");

        var response = await IssueTokenResponseAsync(user, ct, replacesTokenId: oldToken.Id);

        oldToken.RevokedAt = DateTime.UtcNow;
        await unitOfWork.SaveChangesAsync(ct);

        return response;
    }

    /// <inheritdoc/>
    public async Task LogoutAsync(string rawRefreshToken, CancellationToken ct = default)
    {
        var token = await refreshTokenService.ValidateUserRefreshTokenAsync(rawRefreshToken, ct);
        await refreshTokenService.RevokeTokenByIdAsync(token.Id, ct);
    }

    /// <summary>
    /// Issues a token response for the specified user, including an access token and a refresh token.
    /// </summary>
    /// <param name="user">The user for whom to issue the token response.</param>
    /// <param name="ct">The cancellation token.</param>
    /// <param name="replacesTokenId">The ID of the refresh token being replaced, if any.</param>
    /// <returns>A <see cref="TokenResponse"/> containing the access token, refresh token, and user information.</returns>
    private async Task<TokenResponse> IssueTokenResponseAsync(
        User user, CancellationToken ct, Guid? replacesTokenId = null)
    {
        var accessToken = jwtTokenGenerator.GenerateAccessToken(user);
        var rawRefreshToken = await refreshTokenService.CreateAndStoreRefreshTokenAsync(user.Id, replacesTokenId, ct);

        return new TokenResponse(
            AccessToken: accessToken.AccessToken,
            ExpiresAt: accessToken.ExpiresAt,
            RefreshToken: rawRefreshToken,
            User: new UserResponse(user.Id, user.Name, user.Email, user.Role));
    }
}