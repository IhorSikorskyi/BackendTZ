using BackendTZ.Entities;
using Microsoft.EntityFrameworkCore;

namespace BackendTZ.Data.Seed;

/// <summary>
/// Represents a data seeder that populates the database with initial data for services and rooms.
/// </summary>
/// <param name="dbContext">The database context used to access the database.</param>
/// <param name="logger">The logger used to log information during the seeding process.</param>
public class DataSeeder(BookingDbContext dbContext, ILogger<DataSeeder> logger) : IDataSeeder
{
    /// <inheritdoc/>
    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        await dbContext.Database.MigrateAsync(cancellationToken);

        var services = await SeedServicesAsync(cancellationToken);
        await SeedRoomsAsync(services, cancellationToken);

        await dbContext.SaveChangesAsync(cancellationToken);
        logger.LogInformation("Database seeding completed.");
    }

    /// <summary>
    /// Seeds the database with initial services if they do not already exist.
    /// </summary>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous seeding operation. The task result contains a dictionary of service names and their corresponding Service objects.</returns>
    private async Task<Dictionary<string, Service>> SeedServicesAsync(CancellationToken cancellationToken)
    {
        if (await dbContext.Services.AnyAsync(cancellationToken))
        {
            return await dbContext.Services.ToDictionaryAsync(s => s.Name, cancellationToken);
        }

        var services = new List<Service>
        {
            new() { Name = "Проєктор", Price = 500m, Description = "Проєктор для презентацій" },
            new() { Name = "Wi-Fi", Price = 300m, Description = "Бездротовий доступ до інтернету" },
            new() { Name = "Звук", Price = 700m, Description = "Звукове обладнання" },
        };

        dbContext.Services.AddRange(services);
        logger.LogInformation("Seeded {Count} services.", services.Count);

        return services.ToDictionary(s => s.Name);
    }

    /// <summary>
    /// Seeds the database with initial rooms if they do not already exist, associating them with the provided services.
    /// </summary>
    /// <param name="services">A dictionary of service names and their corresponding Service objects.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous seeding operation.</returns>
    private async Task SeedRoomsAsync(Dictionary<string, Service> services, CancellationToken cancellationToken)
    {
        if (await dbContext.Rooms.AnyAsync(cancellationToken))
        {
            return;
        }

        var rooms = new List<Room>
        {
            new()
            {
                Name = "Зал А",
                Capacity = 50,
                BaseHourlyRate = 2000m,
                RoomServices = CreateRoomServices(services, "Проєктор", "Wi-Fi"),
            },
            new()
            {
                Name = "Зал B",
                Capacity = 100,
                BaseHourlyRate = 3500m,
                RoomServices = CreateRoomServices(services, "Проєктор", "Wi-Fi", "Звук"),
            },
            new()
            {
                Name = "Зал C",
                Capacity = 30,
                BaseHourlyRate = 1500m,
                RoomServices = CreateRoomServices(services, "Wi-Fi"),
            },
        };

        dbContext.Rooms.AddRange(rooms);
        logger.LogInformation("Seeded {Count} rooms.", rooms.Count);
    }

    /// <summary>
    /// Creates a list of RoomService objects based on the provided service names and the corresponding Service objects from the dictionary.
    /// </summary>
    /// <param name="services">A dictionary of service names and their corresponding Service objects.</param>
    /// <param name="serviceNames">An array of service names to be associated with the room.</param>
    /// <returns>A list of RoomService objects.</returns>
    private static List<RoomService> CreateRoomServices(
        Dictionary<string, Service> services, params string[] serviceNames) =>
        serviceNames
            .Select(name => new RoomService { Service = services[name] })
            .ToList();
}