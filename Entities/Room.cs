namespace BackendTZ.Entities;

/// <summary>
/// Represents a room entity in the database.
/// </summary>
public class Room : BaseEntity
{
    public required string Name { get; set; }
    public required int Capacity { get; set; }
    public required decimal BaseHourlyRate { get; set; }
    public bool IsAvailable { get; set; } = true;
    public bool IsActive { get; set; } = true;

    public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    public ICollection<RoomService> RoomServices { get; set; } = new List<RoomService>();
}