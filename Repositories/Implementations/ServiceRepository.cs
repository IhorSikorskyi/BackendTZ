using BackendTZ.Data;
using BackendTZ.Entities;
using BackendTZ.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BackendTZ.Repositories.Implementations;

/// <summary>
/// Repository for managing Service entities in the database.
/// </summary>
/// <param name="context">The database context.</param>
public class ServiceRepository(BookingDbContext context) : Repository<Service>(context), IServiceRepository
{
    /// <inheritdoc/>
    public async Task<Service?> GetByNameAsync(string name, CancellationToken cancellationToken)
    {
        return await context.Services.FirstOrDefaultAsync(s => s.Name == name, cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<bool> IsUsedInActiveBookingsOrRoomsAsync(Guid serviceId, CancellationToken cancellationToken)
    {
        var usedInRooms = await context.Rooms
            .AnyAsync(r => r.RoomServices.Any(rs => rs.ServiceId == serviceId), cancellationToken);

        var usedInFutureBookings = await context.Bookings
            .AnyAsync(b =>
                    b.BookingServices.Any(bs => bs.ServiceId == serviceId) &&
                    b.StartTime >= DateTime.UtcNow &&
                    b.Status != BookingStatus.Cancelled,
                cancellationToken);

        return usedInRooms || usedInFutureBookings;
    }

    /// <inheritdoc/>
    public async Task<IReadOnlyList<Service>> GetAllActiveAsync(CancellationToken cancellationToken)
    {
        return await context.Services
            .Where(s => s.IsActive)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<IReadOnlyList<Service>> GetByIdsAsync
        (IEnumerable<Guid> ids, CancellationToken cancellationToken)
    {
        return await context.Services
            .Where(s => ids.Contains(s.Id) && s.IsActive)
            .ToListAsync(cancellationToken);
    }
}