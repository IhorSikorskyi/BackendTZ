namespace BackendTZ.Data.Seed;

/// <summary>
/// Seeds the database with initial reference data (rooms and services)
/// required for the application to be usable out of the box.
/// </summary>
public interface IDataSeeder
{
    /// <summary>
    /// Seeds the database with initial reference data (services and rooms) required for the application to be usable out of the box.
    /// </summary>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous seeding operation.</returns>
    Task SeedAsync(CancellationToken cancellationToken = default);
}