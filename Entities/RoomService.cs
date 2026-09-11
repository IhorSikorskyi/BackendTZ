namespace BackendTZ.Entities;

/// <summary>
/// Represents a many-to-many relationship between a room and the services available in it.
/// </summary>
public class RoomService
{
    /// <summary>
    /// Gets or sets the unique identifier of the room associated with this RoomService.
    /// </summary>
    public Guid RoomId { get; set; }
    /// <summary>
    /// Gets or sets the room associated with this RoomService.
    /// </summary>
    public Room Room { get; set; } = null!;

    /// <summary>
    /// Gets or sets the unique identifier of the service associated with this RoomService.
    /// </summary>
    public Guid ServiceId { get; set; }
    /// <summary>
    /// Gets or sets the service associated with this RoomService.
    /// </summary>
    public Service Service { get; set; } = null!;
}