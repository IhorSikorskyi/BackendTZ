using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace BackendTZ.DTOs.Requests;

/// <summary>
/// Base class for all conference room related requests.
/// </summary>
public abstract record RoomRequest;

/// <summary>
/// Request payload for creating a new conference room.
/// </summary>
/// <param name="Name">The name of the room (e.g. "Зал А").</param>
/// <param name="Capacity">The maximum capacity of the room, in people.</param>
/// <param name="BaseHourlyRate">The base rental cost per hour, in UAH.</param>
/// <param name="ServiceIds">IDs of the additional services available in this room.</param>
public record CreateRoomRequest(
    [property: Required, MaxLength(100)]
    string Name,

    [property: JsonRequired, Required, Range(1, int.MaxValue)]
    int Capacity,

    [property: JsonRequired, Required, Range(0, double.MaxValue)]
    decimal BaseHourlyRate,

    [property: Required]
    List<Guid> ServiceIds
) : RoomRequest;

/// <summary>
/// Request payload for updating an existing conference room.
/// All fields are optional — only provided fields will be updated (PATCH-like semantics).
/// </summary>
/// <param name="Name">The updated name of the room.</param>
/// <param name="Capacity">The updated capacity.</param>
/// <param name="BaseHourlyRate">The updated base rental cost per hour.</param>
/// <param name="ServiceIds">The updated list of available service IDs (replaces the existing list).</param>
public record UpdateRoomRequest(
    [property: MaxLength(100)]
    string? Name,

    [property: Range(1, int.MaxValue)]
    int? Capacity,

    [property: Range(0, double.MaxValue)]
    decimal? BaseHourlyRate,

    List<Guid>? ServiceIds
) : RoomRequest;

/// <summary>
/// Request payload for searching available rooms within a time slot and minimum capacity.
/// </summary>
/// <param name="Date">The date of the requested booking.</param>
/// <param name="StartTime">The requested start time.</param>
/// <param name="EndTime">The requested end time.</param>
/// <param name="MinCapacity">The minimum required capacity.</param>
public record RoomAvailabilityRequest(
    [property: JsonRequired, Required]
    DateOnly Date,

    [property: JsonRequired, Required]
    TimeOnly StartTime,

    [property: JsonRequired, Required]
    TimeOnly EndTime,

    [property: JsonRequired, Required, Range(1, int.MaxValue)]
    int MinCapacity
) : RoomRequest
{
    /// <summary>
    /// Validates that the end time is strictly after the start time.
    /// </summary>
    public bool IsValidTimeRange() => EndTime > StartTime;
}