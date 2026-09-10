using BackendTZ.Entities;

namespace BackendTZ.Repositories.Interfaces;

/// <summary>
/// Repository interface for managing refresh tokens in the application.
/// </summary>
public interface IRefreshTokenRepository : IRepository<RefreshToken>
{
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

    /// <summary>
    /// Removes old refresh tokens that are no longer valid.
    /// </summary>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task RemoveOldTokensAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a refresh token by its hash.
    /// </summary>
    /// <param name="hash">The hash of the refresh token.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the refresh token if found; otherwise, null.</returns>
    Task<RefreshToken?> GetByHashAsync(string hash, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a refresh token by the user ID.
    /// </summary>
    /// <param name="userId">The ID of the user.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the refresh token if found; otherwise, null.</returns>
    Task<IList<RefreshToken>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
}