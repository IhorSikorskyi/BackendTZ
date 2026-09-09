using BackendTZ.Data;
using BackendTZ.Entities;
using BackendTZ.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BackendTZ.Repositories.Implementations;

/// <summary>
/// Represents the implementation of the IUserRepository interface for managing User entities in the database.
/// </summary>
/// <param name="context">The database context used to access User entities.</param>
public class UserRepository(BookingDbContext context) : Repository<User>(context), IUserRepository
{
    /// <inheritdoc/>
    public Task<User?> GetUserByEmailAsync(string email, CancellationToken ct = default)
    {
        return context.Users.FirstOrDefaultAsync(u => u.Email == email, ct);
    }

    /// <inheritdoc/>
    public Task<bool> IsExistByEmailAsync(string email, CancellationToken ct = default)
    {
        return context.Users.AnyAsync(u => u.Email == email, ct);
    }
}