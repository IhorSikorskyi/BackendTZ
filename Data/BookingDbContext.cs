using BackendTZ.Entities;
using Microsoft.EntityFrameworkCore;

namespace BackendTZ.Data;

/// <summary>
/// Represents the database context for the booking system, providing access to the database and managing entity configurations.
/// </summary>
/// <param name="options">The options to be used by a DbContext.</param>
public class BookingDbContext(DbContextOptions<BookingDbContext> options) : DbContext(options)
{
    /// <summary>
    /// Gets the DbSet of User entities in the database.
    /// </summary>
    public DbSet<User> Users => Set<User>();

    /// <summary>
    /// Gets the DbSet of Room entities in the database.
    /// </summary>
    public DbSet<Room> Rooms => Set<Room>();

    /// <summary>
    /// Gets the DbSet of Service entities in the database.
    /// </summary>
    public DbSet<Service> Services => Set<Service>();

    /// <summary>
    /// Gets the DbSet of RoomService entities in the database.
    /// </summary>
    public DbSet<RoomService> RoomServices => Set<RoomService>();

    /// <summary>
    /// Gets the DbSet of Booking entities in the database.
    /// </summary>
    public DbSet<Booking> Bookings => Set<Booking>();

    /// <summary>
    /// Gets the DbSet of BookingService entities in the database.
    /// </summary>
    public DbSet<BookingService> BookingServices => Set<BookingService>();

    /// <summary>
    /// Gets the DbSet of RefreshToken entities in the database.
    /// </summary>
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

    /// <summary>
    /// Configures the model by applying entity configurations from the assembly containing the BookingDbContext class.
    /// </summary>
    /// <param name="modelBuilder">The builder used to construct the model for the context.</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(BookingDbContext).Assembly);
    }

    /// <summary>
    /// Saves changes made in the context to the database, automatically setting the CreatedAt property for newly added entities.
    /// </summary>
    /// <returns>The number of state entries written to the database.</returns>
    public override int SaveChanges()
    {
        var now = DateTime.Now;

        foreach (var entry in ChangeTracker.Entries<BaseEntity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedAt = now;
                    break;
            }
        }

        return base.SaveChanges();

    }
}