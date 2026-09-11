using BackendTZ.Entities;
using BackendTZ.Repositories.Interfaces;
using BackendTZ.Services.Interfaces;
using System.Security.Cryptography;
using System.Text;
using BackendTZ.Exceptions;

namespace BackendTZ.Services.Implementations;

/// <summary>
/// Represents a service for managing refresh tokens, including generation, validation, and revocation.
/// </summary>
/// <param name="refreshTokenRepository">The repository for managing refresh tokens.</param>
/// <param name="unitOfWork">The unit of work for managing transactions.</param>
/// <param name="configuration">The application configuration.</param>
public class RefreshTokenService(
    IRefreshTokenRepository refreshTokenRepository,
    IUnitOfWork unitOfWork,
    IConfiguration configuration) : IRefreshTokenService
{
    /// <inheritdoc/>
    public string GenerateRefreshToken()
    {
        var randomNumber = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }

    /// <inheritdoc/>
    public async Task<string> CreateAndStoreRefreshTokenAsync(
        Guid userId, Guid? replacesTokenId = null, CancellationToken cancellationToken = default)
    {
        var rawToken = GenerateRefreshToken();

        var refreshTokenEntity = new RefreshToken
        {
            UserId = userId,
            RefreshTokenHash = HashRefreshToken(rawToken),
            RefreshTokenExpiry = DateTime.UtcNow.AddDays(
                configuration.GetValue("AppSettings:RefreshTokenExpiryDays", defaultValue: 7))
        };

        await refreshTokenRepository.AddAsync(refreshTokenEntity, cancellationToken);

        if (replacesTokenId is not null)
        {
            var previous = await refreshTokenRepository.GetByIdAsync(replacesTokenId.Value, cancellationToken);
            if (previous is not null)
            {
                previous.ReplacedByTokenId = refreshTokenEntity.Id;
            }
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return rawToken;
    }

    /// <inheritdoc/>
    public async Task<RefreshToken> ValidateUserRefreshTokenAsync(string rawRefreshToken, CancellationToken cancellationToken = default)
    {
        var hashRefreshToken = HashRefreshToken(rawRefreshToken);

        var token = await refreshTokenRepository.GetByHashAsync(hashRefreshToken, cancellationToken)
                    ?? throw new NotFoundException("Invalid refresh token");

        if (token.RevokedAt is not null)
        {
            await refreshTokenRepository.RevokeAllTokensForUserAsync(token.UserId, cancellationToken);
            throw new TokenReuseDetectedException("Suspicious activity. Please try logging in again.");
        }

        if (token.RefreshTokenExpiry < DateTime.UtcNow)
        {
            throw new TokenExpiredException("Refresh token expired.");
        }

        return token;
    }

    /// <inheritdoc/>
    public async Task RevokeAllTokensForUserAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        await refreshTokenRepository.RevokeAllTokensForUserAsync(userId, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<bool> RevokeTokenByIdAsync(Guid tokenId, CancellationToken cancellationToken = default)
    {
        var token = await refreshTokenRepository.GetByIdAsync(tokenId, cancellationToken);
        if (token is null || token.RevokedAt is not null)
        {
            return false;
        }

        var revoked = await refreshTokenRepository.RevokeTokenByIdAsync(tokenId, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return revoked;
    }

    private static string HashRefreshToken(string refreshToken)
    {
        var hashBytes = SHA256.HashData(Encoding.UTF8.GetBytes(refreshToken));
        return Convert.ToBase64String(hashBytes);
    }
}