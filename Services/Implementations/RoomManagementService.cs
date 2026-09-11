using BackendTZ.DTOs.Requests;
using BackendTZ.DTOs.Responses;
using BackendTZ.Entities;
using BackendTZ.Exceptions;
using BackendTZ.Repositories.Interfaces;
using BackendTZ.Services.Interfaces;

namespace BackendTZ.Services.Implementations;

/// <summary>
/// Represents the implementation of the IRoomService interface for managing room-related operations.
/// </summary>
/// <param name="roomRepository">The repository used to access room data.</param>
/// <param name="unitOfWork">The unit of work used to manage transactions.</param>
public class RoomManagementService(
    IRoomRepository roomRepository,
    IUnitOfWork unitOfWork
    ) : IRoomManagementService
{
    /// <inheritdoc/>
    public async Task<RoomDetailsResponse> CreateRoomAsync
        (CreateRoomRequest request, CancellationToken cancellationToken)
    {
        var name = request.Name.Trim();

        var existRoom = await roomRepository.GetByNameAsync(name, cancellationToken);
        if(existRoom != null)
        {
            throw new InvalidOperationException($"A room with the name '{name}' already exists.");
        }

        var room = new Room
        {
            Name = name,
            Capacity = request.Capacity,
            BaseHourlyRate = request.BaseHourlyRate,
            RoomServices = request.ServiceIds.Select(id => new RoomService { ServiceId = id }).ToList()
        };

        await roomRepository.AddAsync(room, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return MapToResponse(room);
    }

    /// <inheritdoc/>
    public async Task<RoomDetailsResponse> UpdateRoomAsync
        (Guid roomId, UpdateRoomRequest request, CancellationToken cancellationToken)
    {
        if (request.Name == null && request.Capacity == null && request.BaseHourlyRate == null && (request.ServiceIds == null || !request.ServiceIds.Any()))
        {
            throw new ValidationException("At least one property must be provided for update.");
        }

        var room = await roomRepository.GetByIdAsync(roomId, cancellationToken)
            ?? throw new InvalidOperationException($"Room with ID '{roomId}' does not exist.");

        if (request.Name != null)
        {
            var newName = request.Name.Trim();

            if (!string.Equals(newName, room.Name, StringComparison.OrdinalIgnoreCase))
            {
                var duplicate = await roomRepository.GetByNameAsync(newName, cancellationToken);
                if (duplicate != null && duplicate.Id != roomId)
                {
                    throw new ConflictException($"A room with the name '{newName}' already exists.");
                }

                room.Name = newName;
            }
        }

        if (request.Capacity != null)
        {
            room.Capacity = request.Capacity.Value;
        }

        if (request.BaseHourlyRate != null)
        {
            room.BaseHourlyRate = request.BaseHourlyRate.Value;
        }

        if (request.ServiceIds != null)
        {
            room.RoomServices.Clear();
            foreach (var serviceId in request.ServiceIds)
            {
                room.RoomServices.Add(new RoomService { ServiceId = serviceId });
            }
        }

        roomRepository.Update(room);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return MapToResponse(room);
    }

    /// <inheritdoc/>
    public async Task<RoomDetailsResponse> DeleteRoomAsync
        (Guid roomId, CancellationToken cancellationToken)
    {
        var room = await roomRepository.GetByIdAsync(roomId, cancellationToken)
                   ?? throw new InvalidOperationException($"Room with ID '{roomId}' does not exist.");

        var hasActiveBookings = await roomRepository.HasActiveBookingsAsync(roomId, cancellationToken);

        if (hasActiveBookings)
        {
            room.IsActive = false;
            roomRepository.Update(room);
        }
        else
        {
            roomRepository.Delete(room);
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return MapToResponse(room);
    }

    /// <inheritdoc/>
    public async Task<RoomDetailsResponse> GetRoomByIdAsync
        (Guid roomId, CancellationToken cancellationToken)
    {
        var room = await roomRepository.GetByIdAsync(roomId, cancellationToken)
                   ?? throw new InvalidOperationException($"Room with ID '{roomId}' does not exist.");
        
        return MapToResponse(room);
    }

    /// <inheritdoc/>
    public async Task<IReadOnlyList<RoomDetailsResponse>> GetAllRoomsAsync
        (CancellationToken cancellationToken)
    {
        var rooms = await roomRepository.GetAllAsync(cancellationToken);

        return rooms.Select(MapToResponse).ToList();
    }

    /// <inheritdoc/>
    public async Task<IReadOnlyList<RoomDetailsResponse>> FindAvailableRoomsAsync
        (RoomAvailabilityRequest request, CancellationToken cancellationToken)
    {
        if (!request.IsValidTimeRange())
        {
            throw new ValidationException("End time must be after start time.");
        }

        var searchStart = request.Date.ToDateTime(request.StartTime);
        var searchEnd = request.Date.ToDateTime(request.EndTime);

        var availableRooms = await roomRepository.FindAvailableAsync(
            searchStart, searchEnd, request.MinCapacity, cancellationToken);

        return availableRooms.Select(MapToResponse).ToList();
    }

    private static RoomDetailsResponse MapToResponse(Room room)
    {
        var services = room.RoomServices
            .Select(rs => new ServiceItemResponse(
                rs.Service.Id,
                rs.Service.Name,
                rs.Service.Description ?? string.Empty,
                rs.Service.Price
            ))
            .ToList();

        return new RoomDetailsResponse(
            room.Id,
            room.Name,
            room.Capacity,
            room.BaseHourlyRate,
            room.IsAvailable,
            services
        );
    }
}