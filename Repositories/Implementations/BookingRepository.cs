using BackendTZ.Data;
using BackendTZ.Entities;
using BackendTZ.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BackendTZ.Repositories.Implementations;

/// <summary>
/// Represents the implementation of the IBookingRepository interface for managing Booking entities in the database.
/// </summary>
/// <param name="context">The database context used to access Booking entities.</param>
public class BookingRepository(BookingDbContext context) : Repository<Booking>(context), IBookingRepository
{
    /// <inheritdoc/>
    public async Task<IReadOnlyList<Booking>> GetUserBookingsAsync
        (Guid userId, CancellationToken cancellationToken)
    {
        return await context.Bookings
            .Where(b => b.UserId == userId)
            .Include(b => b.Room)
            .Include(b => b.BookingServices)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<bool> IsAvailableAsync(
        Guid roomId, DateTime requestedStart, DateTime requestedEnd,
        Guid? excludeBookingId, CancellationToken cancellationToken)
    {
        var roomExists = await context.Rooms.AnyAsync(r => r.Id == roomId && r.IsAvailable && r.IsActive, cancellationToken);
        if (!roomExists) return false;

        var hasConflict = await context.Bookings.AnyAsync(b =>
                b.RoomId == roomId &&
                b.Id != excludeBookingId &&
                b.Status != BookingStatus.Cancelled &&
                b.StartTime < requestedEnd &&
                b.EndTime > requestedStart,
            cancellationToken);

        return !hasConflict;
    }

    ///<inheritdoc/>
    public async Task<IReadOnlyList<Booking>> GetBookingsForPeriodAsync(
        DateTime periodStartInclusive,
        DateTime periodEndExclusive,
        Guid? roomId,
        CancellationToken cancellationToken)
    {
        var query = context.Bookings
            .Include(b => b.Room)
            .Include(b => b.User)
            .Include(b => b.BookingServices)
            .ThenInclude(bs => bs.Service)
            .Where(b => b.StartTime >= periodStartInclusive && b.StartTime < periodEndExclusive);

        if (roomId.HasValue)
        {
            query = query.Where(b => b.RoomId == roomId.Value);
        }

        return await query.ToListAsync(cancellationToken);
    }
}