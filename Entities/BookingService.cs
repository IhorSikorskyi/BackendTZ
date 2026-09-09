namespace BackendTZ.Entities;

/// <summary>
/// Represents a many-to-many relationship between a booking and its selected services,
/// including the price of each service at the time of booking.
/// </summary>
public class BookingService
{
    public Guid BookingId { get; set; }
    public Booking Booking { get; set; } = null!;

    public Guid ServiceId { get; set; }
    public Service Service { get; set; } = null!;

    public decimal PriceAtBooking { get; set; }
}