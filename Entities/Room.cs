namespace BackendTZ.Entities;

/// <summary>
/// Represents a room entity in the database.
/// </summary>
public class Room : BaseEntity
{
    /// <summary>
    /// Gets or sets the name of the room.
    /// </summary>
    public required string Name { get; set; }
    /// <summary>
    /// Gets or sets the capacity of the room.
    /// </summary>
    public required int Capacity { get; set; }
    /// <summary>
    /// Gets or sets the base hourly rate of the room.
    /// </summary>
    public required decimal BaseHourlyRate { get; set; }
    /// <summary>
    /// Gets or sets a value indicating whether the room is available.
    /// </summary>
    public bool IsAvailable { get; set; } = true;
    /// <summary>
    /// Gets or sets a value indicating whether the room is active.
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Gets or sets the collection of bookings associated with the room.
    /// </summary>
    public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    /// <summary>
    /// Gets or sets the collection of room services associated with the room.
    /// </summary>
    public ICollection<RoomService> RoomServices { get; set; } = new List<RoomService>();
}