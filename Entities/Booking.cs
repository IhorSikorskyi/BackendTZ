namespace BackendTZ.Entities;

/// <summary>
/// Represents a booking entity in the database.
/// </summary>
public class Booking : BaseEntity
{
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;

    public Guid RoomId { get; set; }
    public Room Room { get; set; } = null!;

    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public decimal TotalCost { get; set; }
    public BookingStatus Status { get; set; } = BookingStatus.Pending;

    public ICollection<BookingService> BookingServices { get; set; } = new List<BookingService>();
}

public enum BookingStatus
{
    Pending,
    Confirmed,
    Cancelled
}