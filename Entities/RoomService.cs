namespace BackendTZ.Entities;

/// <summary>
/// Represents a many-to-many relationship between a room and the services available in it.
/// </summary>
public class RoomService
{
    public Guid RoomId { get; set; }
    public Room Room { get; set; } = null!;

    public Guid ServiceId { get; set; }
    public Service Service { get; set; } = null!;
}