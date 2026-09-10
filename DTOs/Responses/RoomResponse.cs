namespace BackendTZ.DTOs.Responses;

/// <summary>
/// Base class for all conference room related responses.
/// </summary>
public abstract record RoomResponse;

/// <summary>
/// Represents public-facing information about a conference room.
/// </summary>
/// <param name="Id">The unique identifier of the room.</param>
/// <param name="Name">The name of the room.</param>
/// <param name="Capacity">The maximum capacity, in people.</param>
/// <param name="BaseHourlyRate">The base rental cost per hour, in UAH.</param>
/// <param name="Services">The additional services available in this room.</param>
public record RoomDetailsResponse(
    Guid Id,
    string Name,
    int Capacity,
    decimal BaseHourlyRate,
    bool IsAvailable,
    List<ServiceItemResponse> Services
) : RoomResponse;