using BackendTZ.Data;
using BackendTZ.Entities;
using BackendTZ.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BackendTZ.Repositories.Implementations;

/// <summary>
/// Represents the implementation of the IRefreshTokenRepository interface for managing RefreshToken entities in the database.
/// </summary>
/// <param name="context">The database context used to access RefreshToken entities.</param>
public class RefreshTokenRepository(BookingDbContext context) : Repository<RefreshToken>(context), IRefreshTokenRepository
{
    /// <inheritdoc/>
    public async Task RevokeAllTokensForUserAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var activeTokens = await context.RefreshTokens
            .Where(t => t.UserId == userId && t.RevokedAt == null)
            .ToListAsync(cancellationToken);

        var now = DateTime.UtcNow;
        foreach (var token in activeTokens)
        {
            token.RevokedAt = now;
        }
    }

    /// <inheritdoc/>
    public async Task<bool> RevokeTokenByIdAsync(Guid tokenId, CancellationToken cancellationToken = default)
    {
        var token = await context.RefreshTokens
            .FirstOrDefaultAsync(t => t.Id == tokenId && t.RevokedAt == null, cancellationToken);

        if (token is null)
        {
            return false;
        }

        token.RevokedAt = DateTime.UtcNow;
        return true;
    }

    /// <inheritdoc/>
    public async Task RemoveOldTokensAsync(CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;

        var staleTokens = await context.RefreshTokens
            .Where(t => t.RefreshTokenExpiry < now || t.RevokedAt != null)
            .ToListAsync(cancellationToken);

        context.RefreshTokens.RemoveRange(staleTokens);
    }

    /// <inheritdoc/>
    public async Task<RefreshToken?> GetByHashAsync(string hash, CancellationToken cancellationToken = default)
    {
        return await context.RefreshTokens
            .FirstOrDefaultAsync(t => t.RefreshTokenHash == hash, cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<IList<RefreshToken>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await context.RefreshTokens
            .Where(t => t.UserId == userId && t.RevokedAt == null)
            .ToListAsync(cancellationToken);
    }
}