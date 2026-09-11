using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

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
/// <param name="NumberOfGuests">The number of guests for the booking.</param>
/// <param name="ServiceIds">IDs of the additional services selected for this booking.</param>
public record CreateBookingRequest(
    [property: JsonRequired, Required] Guid RoomId,

    [property: JsonRequired, Required] DateOnly Date,

    [property: JsonRequired, Required] TimeOnly StartTime,

    [property: JsonRequired, Required] TimeOnly EndTime,
    [property: JsonRequired, Required] int NumberOfGuests,

    List<Guid>? ServiceIds
) : BookingRequest

{
    /// <summary>
    /// Checks if the time range specified by StartTime and EndTime is valid (i.e., EndTime is after StartTime).
    /// </summary>
    /// <returns>True if the time range is valid; otherwise, false.</returns>
    public bool IsValidTimeRange() => EndTime > StartTime;
}

/// <summary>
/// Request payload for changing an existing room booking.
/// </summary>
/// <param name="RoomId">The new ID of the room to book.</param>
/// <param name="Date">The new date of the booking.</param>
/// <param name="StartTime">The new start time of the booking.</param>
/// <param name="EndTime">The new end time of the booking.</param>
/// <param name="NumberOfGuests">The new number of guests for the booking.</param>
/// <param name="ServiceIds">The new list of additional service IDs for the booking.</param>
public record ChangeBookingRequest(
    Guid? RoomId,
    DateOnly? Date,
    TimeOnly? StartTime,
    TimeOnly? EndTime,
    int? NumberOfGuests,
    List<Guid>? ServiceIds
) : BookingRequest
{
    /// <summary>
    /// Checks if the time range specified by StartTime and EndTime is valid (i.e., EndTime is after StartTime).
    /// </summary>
    /// <returns>True if the time range is valid; otherwise, false.</returns>
    public bool IsValidTimeRange() => EndTime > StartTime;
}