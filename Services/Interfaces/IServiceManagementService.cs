using BackendTZ.DTOs.Requests;
using BackendTZ.DTOs.Responses;

namespace BackendTZ.Services.Interfaces;

/// <summary>
/// Interface for managing services in the system, providing methods for creating, updating, deleting, and retrieving service information.
/// </summary>
public interface IServiceManagementService
{
    /// <summary>
    /// Creates a new service in the system based on the provided request data.
    /// </summary>
    /// <param name="request">The request object containing the details of the service to be created.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>The created service item response.</returns>
    Task<ServiceItemResponse> CreateServiceAsync
        (CreateServiceRequest request, CancellationToken cancellationToken);

    /// <summary>
    /// Updates an existing service in the system based on the provided request data.
    /// </summary>
    /// <param name="serviceId">The ID of the service to be updated.</param>
    /// <param name="request">The request object containing the updated details of the service.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>The updated service item response.</returns>
    Task<ServiceItemResponse> UpdateServiceAsync 
        (Guid serviceId, UpdateServiceRequest request, CancellationToken cancellationToken);

    /// <summary>
    /// Deletes a service from the system based on the provided service ID.
    /// </summary>
    /// <param name="serviceId">The ID of the service to be deleted.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>The deleted service item response.</returns>
    Task<ServiceItemResponse> DeleteServiceAsync 
        (Guid serviceId, CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves a service from the system based on the provided service ID.
    /// </summary>
    /// <param name="serviceId">The ID of the service to be retrieved.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>The retrieved service item response.</returns>
    Task<ServiceItemResponse> GetServiceAsync 
        (Guid serviceId, CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves all services from the system.
    /// </summary>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A list of all service item responses.</returns>
    Task<IReadOnlyList<ServiceItemResponse>> GetAllServicesAsync
        (CancellationToken cancellationToken);
}