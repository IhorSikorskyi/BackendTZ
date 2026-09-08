using BackendTZ.Data;
using BackendTZ.Repositories.Interfaces;

namespace BackendTZ.Repositories.Implementations;

/// <summary>
/// Represents a unit of work for managing database operations.
/// </summary>
/// <param name="context">The database context.</param>
public class UnitOfWork(BookingDbContext context) : IUnitOfWork
{
    /// <inheritdoc/>
    public Task<int> SaveChangesAsync(CancellationToken ct = default) =>
        context.SaveChangesAsync(ct);
}