namespace BackendTZ.Entities;

/// <summary>
/// Represents a booking entity in the database.
/// </summary>
public class Booking : BaseEntity
{
    /// <summary>
    /// Gets or sets the unique identifier of the user associated with the booking.
    /// </summary>
    public Guid UserId { get; set; }
    /// <summary>
    /// Gets or sets the user associated with the booking.
    /// </summary>
    public User User { get; set; } = null!;

    /// <summary>
    /// Gets or sets the unique identifier of the room associated with the booking.
    /// </summary>
    public Guid RoomId { get; set; }
    /// <summary>
    /// Gets or sets the room associated with the booking.
    /// </summary>
    public Room Room { get; set; } = null!;

    /// <summary>
    /// Gets or sets the start time of the booking.
    /// </summary>
    public DateTime StartTime { get; set; }

    /// <summary>
    /// Gets or sets the end time of the booking.
    /// </summary>
    public DateTime EndTime { get; set; }

    /// <summary>
    /// Gets or sets the base cost of the booking before any adjustments or additional services.
    /// </summary>
    public decimal BaseCost { get; set; }

    /// <summary>
    /// Gets or sets the time adjustment for the booking, which can be a discount or surcharge based on the time slot.
    /// </summary>
    public decimal TimeAdjustment { get; set; }

    /// <summary>
    /// Gets or sets the total cost of the additional services selected for the booking.
    /// </summary>
    public decimal ServicesCost { get; set; }

    /// <summary>
    /// Gets or sets the total cost of the booking, including base cost, time adjustments, and additional services.
    /// </summary>
    public decimal TotalCost { get; set; }

    /// <summary>
    /// Gets or sets the status of the booking, which can be Pending, Confirmed, or Cancelled.
    /// </summary>
    public BookingStatus Status { get; set; } = BookingStatus.Pending;

    /// <summary>
    /// Gets or sets the collection of booking services associated with this booking.
    /// </summary>
    public ICollection<BookingService> BookingServices { get; set; } = new List<BookingService>();
}

/// <summary>
/// Represents the status of a booking, indicating whether it is pending, confirmed, or cancelled.
/// </summary>
public enum BookingStatus
{
    /// <summary>
    /// Indicates that the booking is pending and has not yet been confirmed.
    /// </summary>
    Pending,
    /// <summary>
    /// Indicates that the booking has been confirmed and is valid.
    /// </summary>
    Confirmed,
    /// <summary>
    /// Indicates that the booking has been cancelled and is no longer valid.
    /// </summary>
    Cancelled
}