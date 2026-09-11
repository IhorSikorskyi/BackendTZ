using BackendTZ.Data;
using BackendTZ.Entities;
using BackendTZ.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BackendTZ.Repositories.Implementations;

/// <summary>
/// Represents the implementation of the IRoomRepository interface for managing Room entities in the database.
/// </summary>
/// <param name="context">The database context used to access Room entities.</param>
public class RoomRepository(BookingDbContext context) : Repository<Room>(context), IRoomRepository
{
    ///<inheritdoc/>
    public async Task<Room?> GetByNameAsync(string name, CancellationToken cancellationToken)
    {
        return await context.Rooms
            .FirstOrDefaultAsync(r => r.Name == name, cancellationToken);
    }

    ///<inheritdoc/>
    public async Task<IList<Room>> FindAvailableAsync(
        DateTime searchStart, DateTime searchEnd, int minCapacity, CancellationToken cancellationToken)
    {
        return await context.Rooms
            .Where(r => r.IsAvailable && r.IsActive && r.Capacity >= minCapacity)
            .Where(r => !r.Bookings.Any(b =>
                b.Status != BookingStatus.Cancelled &&
                b.StartTime < searchEnd &&
                b.EndTime > searchStart))
            .ToListAsync(cancellationToken);
    }

    ///<inheritdoc/>
    public async Task<bool> HasActiveBookingsAsync(Guid roomId, CancellationToken cancellationToken)
    {
        var now = DateTime.UtcNow;

        return await context.Bookings.AnyAsync(b =>
                b.RoomId == roomId &&
                b.Status != BookingStatus.Cancelled &&
                b.EndTime > now,
            cancellationToken);
    }

    ///<inheritdoc/>
    public async Task<IReadOnlyList<Room>> GetRoomsAsync(
        bool includeInactive,
        Guid? roomId,
        CancellationToken cancellationToken)
    {
        var query = context.Rooms.AsQueryable();

        if (!includeInactive)
        {
            query = query.Where(r => r.IsActive);
        }

        if (roomId.HasValue)
        {
            query = query.Where(r => r.Id == roomId.Value);
        }

        return await query.ToListAsync(cancellationToken);
    }
}