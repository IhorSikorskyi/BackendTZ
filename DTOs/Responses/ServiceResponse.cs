namespace BackendTZ.DTOs.Responses;

/// <summary>
/// Base class for all service (додаткова послуга) related responses.
/// </summary>
public abstract record ServiceResponse;

/// <summary>
/// Represents an additional service available for a conference room.
/// </summary>
/// <param name="Id">The unique identifier of the service.</param>
/// <param name="Name">The name of the service.</param>
/// <param name="Description">The description of the service.</param>
/// <param name="Price">The price of the service in UAH.</param>
public record ServiceItemResponse(
    Guid Id,
    string Name,
    string? Description,
    decimal Price
) : ServiceResponse;