using BackendTZ.Entities;

namespace BackendTZ.Repositories.Interfaces;

public interface IServiceRepository : IRepository<Service>
{
    /// <summary>
    /// Gets a service by its name asynchronously.
    /// </summary>
    /// <param name="name">The name of the service.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>The service with the specified name, or null if not found.</returns>
    Task<Service?> GetByNameAsync(string name, CancellationToken cancellationToken);

    /// <summary>
    /// Checks if a service is used in any active bookings or rooms asynchronously.
    /// </summary>
    /// <param name="serviceId">The ID of the service to check.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>True if the service is used in any active bookings or rooms; otherwise, false.</returns>
    Task<bool> IsUsedInActiveBookingsOrRoomsAsync(Guid serviceId, CancellationToken cancellationToken);

    /// <summary>
    /// Gets all active services asynchronously.
    /// </summary>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A list of all active services.</returns>
    Task<IReadOnlyList<Service>> GetAllActiveAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Gets services by their IDs asynchronously.
    /// </summary>
    /// <param name="ids">The IDs of the services to retrieve.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A list of services with the specified IDs.</returns>
    Task<IReadOnlyList<Service>> GetByIdsAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken);
}