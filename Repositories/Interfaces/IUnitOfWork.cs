namespace BackendTZ.Repositories.Interfaces;

public interface IUnitOfWork
{
    /// <summary>
    /// Saves changes made in the context to the database asynchronously.
    /// </summary>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>The number of state entries written to the database.</returns>
    Task<int> SaveChangesAsync(CancellationToken ct = default);
}