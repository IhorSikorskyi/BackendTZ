namespace BackendTZ.Repositories.Interfaces;

/// <summary>
/// Defines a contract for a unit of work that manages the persistence of changes to the database.
/// </summary>
public interface IUnitOfWork
{
    /// <summary>
    /// Saves changes made in the context to the database asynchronously.
    /// </summary>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>The number of state entries written to the database.</returns>
    Task<int> SaveChangesAsync(CancellationToken ct = default);
}