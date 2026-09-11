using BackendTZ.Data;
using BackendTZ.Entities;
using Microsoft.EntityFrameworkCore;

namespace BackendTZ.Services.BackgroundServices;

/// <summary>
/// A background service that periodically synchronizes the availability of rooms based on their bookings.
/// </summary>
/// <param name="scopeFactory">The factory used to create service scopes.</param>
/// <param name="configuration">The application configuration.</param>
/// <param name="logger">The logger used to log information and errors.</param>
public class RoomAvailabilitySyncService(
    IServiceScopeFactory scopeFactory,
    IConfiguration configuration,
    ILogger<RoomAvailabilitySyncService> logger) : BackgroundService
{
    /// <summary>
    /// Executes the background service, periodically synchronizing the availability of rooms based on their bookings.
    /// </summary>
    /// <param name="stoppingToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var minutes = configuration.GetValue("AppSettings:RoomAvailabilitySync", defaultValue: 30);

        if (minutes <= 0)
        {
            logger.LogWarning(
                "Invalid room availability sync interval ({Minutes}m) configured, falling back to 30m.", minutes);
            minutes = 30;
        }

        using var timer = new PeriodicTimer(TimeSpan.FromMinutes(minutes));

        while (await timer.WaitForNextTickAsync(stoppingToken))
        {
            await SyncAsync(stoppingToken);
        }
    }

    /// <summary>
    /// Synchronizes the availability of rooms based on their bookings.
    /// </summary>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    internal async Task SyncAsync(CancellationToken cancellationToken)
    {
        try
        {
            using var scope = scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<BookingDbContext>();

            var now = DateTime.Now;

            // Update rooms that should be occupied based on their bookings
            var roomsToOccupy = db.Rooms
                .Where(r => r.IsActive && r.IsAvailable)
                .Where(r => r.Bookings.Any(b =>
                    b.Status != BookingStatus.Cancelled
                    && b.StartTime <= now
                    && b.EndTime > now));

            var occupiedCount = await roomsToOccupy.ExecuteUpdateAsync(
                setters => setters.SetProperty(r => r.IsAvailable, false),
                cancellationToken);

            // Update rooms that should be freed based on their bookings
            var roomsToFree = db.Rooms
                .Where(r => r.IsActive && !r.IsAvailable)
                .Where(r => !r.Bookings.Any(b =>
                    b.Status != BookingStatus.Cancelled
                    && b.StartTime <= now
                    && b.EndTime > now));

            var freedCount = await roomsToFree.ExecuteUpdateAsync(
                setters => setters.SetProperty(r => r.IsAvailable, true),
                cancellationToken);

            if (occupiedCount > 0 || freedCount > 0)
            {
                logger.LogInformation(
                    "Room availability sync: {Occupied} occupied, {Freed} freed.",
                    occupiedCount, freedCount);
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to sync room availability.");
        }
    }
}