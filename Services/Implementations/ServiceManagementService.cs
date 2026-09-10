using BackendTZ.DTOs.Requests;
using BackendTZ.DTOs.Responses;
using BackendTZ.Entities;
using BackendTZ.Exceptions;
using BackendTZ.Repositories.Interfaces;
using BackendTZ.Services.Interfaces;

namespace BackendTZ.Services.Implementations;

/// <summary>
/// ServiceManagementService is responsible for managing services in the system, including creating, updating, deleting, and retrieving service information.
/// </summary>
/// <param name="serviceRepository">The repository for accessing service data.</param>
/// <param name="unitOfWork">The unit of work for managing transactions.</param>
public class ServiceManagementService(
    IServiceRepository serviceRepository,
    IUnitOfWork unitOfWork) : IServiceManagementService
{
    /// <inheritdoc/>
    public async Task<ServiceItemResponse> CreateServiceAsync(CreateServiceRequest request, CancellationToken cancellationToken)
    {
        var name = request.Name.Trim();

        var existingService = await serviceRepository.GetByNameAsync(name, cancellationToken);
        if (existingService != null)
        {
            throw new ConflictException($"A service with the name '{name}' already exists.");
        }

        var service = new Service
        {
            Id = Guid.NewGuid(),
            Name = name,
            Description = request.Description?.Trim() ?? string.Empty,
            Price = request.Price,
            IsActive = true
        };

        await serviceRepository.AddAsync(service, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return MapToResponse(service);
    }

    /// <inheritdoc/>
    public async Task<ServiceItemResponse> UpdateServiceAsync(Guid serviceId, UpdateServiceRequest request, CancellationToken cancellationToken)
    {
        if (request.Name == null && request.Description == null && request.Price == null)
        {
            throw new ValidationException("At least one property must be provided for update.");
        }
        
        var service = await serviceRepository.GetByIdAsync(serviceId, cancellationToken)
            ?? throw new NotFoundException("Service not found.");

        if (request.Name != null)
        {
            var newName = request.Name.Trim();

            if (!string.Equals(newName, service.Name, StringComparison.OrdinalIgnoreCase))
            {
                var duplicate = await serviceRepository.GetByNameAsync(newName, cancellationToken);
                if (duplicate != null && duplicate.Id != serviceId)
                {
                    throw new ConflictException($"A service with the name '{newName}' already exists.");
                }

                service.Name = newName;
            }
        }

        if (request.Description != null)
        {
            service.Description = request.Description.Trim();
        }

        if (request.Price != null)
        {
            service.Price = request.Price.Value;
        }

        serviceRepository.Update(service);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return MapToResponse(service);
    }

    /// <inheritdoc/>
    public async Task<ServiceItemResponse> DeleteServiceAsync(Guid serviceId, CancellationToken cancellationToken)
    {
        var service = await serviceRepository.GetByIdAsync(serviceId, cancellationToken)
            ?? throw new NotFoundException("Service not found.");

        var isInUse = await serviceRepository.IsUsedInActiveBookingsOrRoomsAsync(serviceId, cancellationToken);
        if (isInUse)
        {
            service.IsActive = false;
            serviceRepository.Update(service);
        }
        else
        {
            serviceRepository.Delete(service);
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return MapToResponse(service);
    }

    /// <inheritdoc/>
    public async Task<ServiceItemResponse> GetServiceAsync(Guid serviceId, CancellationToken cancellationToken)
    {
        var service = await serviceRepository.GetByIdAsync(serviceId, cancellationToken)
            ?? throw new NotFoundException("Service not found.");

        return MapToResponse(service);
    }

    /// <inheritdoc/>
    public async Task<IReadOnlyList<ServiceItemResponse>> GetAllServicesAsync(CancellationToken cancellationToken)
    {
        var services = await serviceRepository.GetAllActiveAsync(cancellationToken);

        return services.Select(MapToResponse).ToList();
    }

    private static ServiceItemResponse MapToResponse(Service service)
    {
        return new ServiceItemResponse(
            service.Id,
            service.Name,
            service.Description,
            service.Price
        );
    }
}