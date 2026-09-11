namespace BackendTZ.Entities;

/// <summary>
/// Represents a many-to-many relationship between a booking and its selected services,
/// including the price of each service at the time of booking.
/// </summary>
public class BookingService
{
    /// <summary>
    /// Gets or sets the ID of the booking associated with this service.
    /// </summary>
    public Guid BookingId { get; set; }
    /// <summary>
    /// Gets or sets the booking associated with this service.
    /// </summary>
    public Booking Booking { get; set; } = null!;

    /// <summary>
    /// Gets or sets the ID of the service associated with this booking.
    /// </summary>
    public Guid ServiceId { get; set; }
    /// <summary>
    /// Gets or sets the service associated with this booking.
    /// </summary>
    public Service Service { get; set; } = null!;

    /// <summary>
    /// Gets or sets the price of the service at the time of booking.
    /// </summary>
    public decimal PriceAtBooking { get; set; }
}