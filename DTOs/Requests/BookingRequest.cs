using System.ComponentModel.DataAnnotations;

namespace BackendTZ.DTOs.Requests;

/// <summary>
/// Base class for all booking related requests.
/// </summary>
public abstract record BookingRequest;

/// <summary>
/// Request payload for creating a new room booking.
/// </summary>
/// <param name="RoomId">The ID of the room to book.</param>
/// <param name="Date">The date of the booking.</param>
/// <param name="StartTime">The start time of the booking.</param>
/// <param name="EndTime">The end time of the booking.</param>
/// <param name="ServiceIds">IDs of the additional services selected for this booking.</param>
public record CreateBookingRequest(
    [property: Required]
    Guid RoomId,

    [property: Required]
    DateOnly Date,

    [property: Required]
    TimeOnly StartTime,

    [property: Required]
    TimeOnly EndTime,

    List<Guid>? ServiceIds
) : BookingRequest
{
    public bool IsValidTimeRange() => EndTime > StartTime;
}