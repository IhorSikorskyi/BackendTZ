using BackendTZ.Entities;

namespace BackendTZ.Repositories.Interfaces;

/// <summary>
/// Represents a repository for managing User entities in the database.
/// </summary>
public interface IUserRepository : IRepository<User>
{
    /// <summary>
    /// Gets a user by their email address asynchronously.
    /// </summary>
    /// <param name="email">The email address of the user.</param>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>The user with the specified email address, or null if not found.</returns>
    Task<User?> GetUserByEmailAsync(string email, CancellationToken ct = default);

    /// <summary>
    /// Checks if a user exists by their email address asynchronously.
    /// </summary>
    /// <param name="email">The email address of the user.</param>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>True if a user with the specified email address exists; otherwise, false.</returns>
    Task<bool> IsExistByEmailAsync(string email, CancellationToken ct = default);
}