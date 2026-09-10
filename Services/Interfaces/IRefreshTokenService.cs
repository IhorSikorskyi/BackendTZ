using BackendTZ.Entities;

namespace BackendTZ.Services.Interfaces;

/// <summary>
/// Represents a service for managing refresh tokens, including generation, validation, and revocation.
/// </summary>
public interface IRefreshTokenService
{
    /// <summary>
    /// Generates a new refresh token as a string.
    /// </summary>
    /// <returns>The generated refresh token as a string.</returns>
    string GenerateRefreshToken();

    /// <summary>
    /// Creates and stores a new refresh token for a specific user.
    /// </summary>
    /// <param name="userId">The ID of the user for whom the refresh token is being created.</param>
    /// <param name="replacesTokenId">The ID of the token being replaced, if any.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>The generated refresh token as a string.</returns>
    Task<string> CreateAndStoreRefreshTokenAsync(
        Guid userId, Guid? replacesTokenId = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Validates a user's refresh token.
    /// </summary>
    /// <param name="rawRefreshToken">The raw refresh token to validate.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>The validated refresh token.</returns>
    Task<RefreshToken> ValidateUserRefreshTokenAsync(string rawRefreshToken, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Revokes all refresh tokens associated with a specific user.
    /// </summary>
    /// <param name="userId">The ID of the user whose tokens should be revoked.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task RevokeAllTokensForUserAsync(Guid userId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Revokes a specific refresh token for a user.
    /// </summary>
    /// <param name="tokenId">The ID of the token to be revoked.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains true if the token was successfully revoked; otherwise, false.</returns>
    Task<bool> RevokeTokenByIdAsync(Guid tokenId, CancellationToken cancellationToken = default);
}