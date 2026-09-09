namespace BackendTZ.Entities;

/// <summary>
/// Represents a service entity in the database.
/// </summary>
public class Service : BaseEntity
{
    public required string Name { get; set; }
    public string? Description { get; set; }
    public required decimal Price { get; set; }

    public ICollection<RoomService> RoomServices { get; set; } = new List<RoomService>();
    public ICollection<BookingService> BookingServices { get; set; } = new List<BookingService>();
}