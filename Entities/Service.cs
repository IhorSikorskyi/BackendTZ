namespace BackendTZ.Entities;

/// <summary>
/// Represents a service entity in the database.
/// </summary>
public class Service : BaseEntity
{
    /// <summary>
    /// Gets or sets the name of the service.
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// Gets or sets the description of the service.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the price of the service.
    /// </summary>
    public required decimal Price { get; set; }
    /// <summary>
    /// Gets or sets a value indicating whether the service is active.
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Gets or sets the collection of room services associated with the service.
    /// </summary>
    public ICollection<RoomService> RoomServices { get; set; } = new List<RoomService>();
    /// <summary>
    /// Gets or sets the collection of booking services associated with the service.
    /// </summary>
    public ICollection<BookingService> BookingServices { get; set; } = new List<BookingService>();
}