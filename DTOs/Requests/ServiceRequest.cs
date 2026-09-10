using System.ComponentModel.DataAnnotations;

namespace BackendTZ.DTOs.Requests;

/// <summary>
/// Base class for all service (додаткова послуга) related requests.
/// </summary>
public abstract record ServiceRequest;

/// <summary>
/// Request payload for creating a new additional service (e.g. projector, Wi-Fi).
/// </summary>
/// <param name="Name">The name of the service.</param>
/// <param name="Description">The description of the service.</param>
/// <param name="Price">The price of the service in UAH.</param>
public record CreateServiceRequest(
    [property: Required, MaxLength(100)]
    string Name,

    [property: MaxLength(1000)]
    string? Description,

    [property: Required, Range(0, double.MaxValue)]
    decimal Price
) : ServiceRequest;

/// <summary>
/// Request payload for updating an existing additional service.
/// </summary>
/// <param name="Name">The updated name of the service.</param>
/// <param name="Price">The updated price of the service in UAH.</param>
public record UpdateServiceRequest(
    [property: MaxLength(100)]
    string? Name,

    [property: MaxLength(1000)]
    string? Description,

    [property: Range(0, double.MaxValue)]
    decimal? Price
) : ServiceRequest;