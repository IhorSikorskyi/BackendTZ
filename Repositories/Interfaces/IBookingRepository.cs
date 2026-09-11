using BackendTZ.Entities;

namespace BackendTZ.Repositories.Interfaces;

/// <summary>
/// Defines the interface for a repository that manages Booking entities in the database.
/// </summary>
public interface IBookingRepository : IRepository<Booking>
{
    /// <summary>
    /// Gets all bookings for a specific user.
    /// </summary>
    /// <param name="userId">The ID of the user.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A read-only list of bookings for the specified user.</returns>
    Task<IReadOnlyList<Booking>> GetUserBookingsAsync
        (Guid userId, CancellationToken cancellationToken);

    /// <summary>
    /// Checks if a room is available for booking within a specified time range, optionally excluding a specific booking ID.
    /// </summary>
    /// <param name="roomId">The ID of the room to check availability for.</param>
    /// <param name="requestedStart">The start time of the requested booking.</param>
    /// <param name="requestedEnd">The end time of the requested booking.</param>
    /// <param name="excludeBookingId">An optional booking ID to exclude from the availability check.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>True if the room is available; otherwise, false.</returns>
    Task<bool> IsAvailableAsync(
        Guid roomId, DateTime requestedStart, DateTime requestedEnd,
        Guid? excludeBookingId, CancellationToken cancellationToken);

    /// <summary>
    /// Loads bookings whose start time falls within [periodStartInclusive, periodEndExclusive),
    /// including Room, User, and BookingServices+Service navigation properties needed for reporting.
    /// </summary>
    /// <param name="periodStartInclusive">The start of the period (inclusive) to filter bookings.</param>
    /// <param name="periodEndExclusive">The end of the period (exclusive) to filter bookings.</param>
    /// <param name="roomId">An optional room ID to filter bookings by a specific room.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A read-only list of bookings that fall within the specified period and match the optional room filter.</returns>
    Task<IReadOnlyList<Booking>> GetBookingsForPeriodAsync(
        DateTime periodStartInclusive,
        DateTime periodEndExclusive,
        Guid? roomId,
        CancellationToken cancellationToken);
}