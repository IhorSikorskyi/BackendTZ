using BackendTZ.Entities;

namespace BackendTZ.Repositories.Interfaces;

/// <summary>
/// Defines the interface for a repository that manages Room entities in the database.
/// </summary>
public interface IRoomRepository : IRepository<Room>
{
    /// <summary>
    /// Finds a room by its name.
    /// </summary>
    /// <param name="name">The name of the room.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>The room with the specified name, or null if no such room exists.</returns>
    Task<Room?> GetByNameAsync(string name, CancellationToken cancellationToken);

    /// <summary>
    /// Finds available rooms within the specified date range and with at least the specified minimum capacity.
    /// </summary>
    /// <param name="searchStart">The start date and time of the search range.</param>
    /// <param name="searchEnd">The end date and time of the search range.</param>
    /// <param name="minCapacity">The minimum capacity required for the room.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A list of rooms that are available within the specified date range and meet the minimum capacity requirement.</returns>
    Task<IList<Room>> FindAvailableAsync(
        DateTime searchStart, DateTime searchEnd, int minCapacity, CancellationToken cancellationToken);

    /// <summary>
    /// Checks if a room has any active bookings.
    /// </summary>
    /// <param name="roomId">The ID of the room.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>True if the room has active bookings; otherwise, false.</returns>
    Task<bool> HasActiveBookingsAsync(Guid roomId, CancellationToken cancellationToken);

    /// <summary>
    /// Loads rooms, optionally including soft-deleted (IsActive = false) ones and/or
    /// filtered to a single room, for reports that need a full room roster (e.g. utilization).
    /// </summary>
    /// <param name="includeInactive">Whether to include inactive (soft-deleted) rooms.</param>
    /// <param name="roomId">An optional room ID to filter the results to a specific room.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A read-only list of rooms that match the specified criteria.</returns> 
    Task<IReadOnlyList<Room>> GetRoomsAsync(
        bool includeInactive,
        Guid? roomId,
        CancellationToken cancellationToken);
}