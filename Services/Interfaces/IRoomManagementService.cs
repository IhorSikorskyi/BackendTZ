using BackendTZ.DTOs.Requests;
using BackendTZ.DTOs.Responses;

namespace BackendTZ.Services.Interfaces;

public interface IRoomManagementService
{
    /// <summary>
    /// Creates a new room with the specified details.
    /// </summary>
    /// <param name="request">The details of the room to create.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>The details of the created room.</returns>
    Task<RoomDetailsResponse> CreateRoomAsync
        (CreateRoomRequest request, CancellationToken cancellationToken);

    /// <summary>
    /// Updates the details of an existing room identified by its ID.
    /// </summary>
    /// <param name="roomId">The ID of the room to update.</param>
    /// <param name="request">The updated details of the room.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>The details of the updated room.</returns>
    Task<RoomDetailsResponse> UpdateRoomAsync
        (Guid roomId, UpdateRoomRequest request, CancellationToken cancellationToken);

    /// <summary>
    /// Deletes an existing room identified by its ID.
    /// </summary>
    /// <param name="roomId">The ID of the room to delete.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>The details of the deleted room.</returns>
    Task<RoomDetailsResponse> DeleteRoomAsync
        (Guid roomId, CancellationToken cancellationToken);
    
    /// <summary>
    /// Retrieves the details of a room identified by its ID.
    /// </summary>
    /// <param name="roomId">The ID of the room to retrieve.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>The details of the retrieved room.</returns>
    Task<RoomDetailsResponse> GetRoomByIdAsync
        (Guid roomId, CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves the details of all rooms.
    /// </summary>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A list of details of all rooms.</returns>
    Task<IReadOnlyList<RoomDetailsResponse>> GetAllRoomsAsync
        (CancellationToken cancellationToken);

    /// <summary>
    /// Finds available rooms based on the specified availability request.
    /// </summary>
    /// <param name="request">The availability request containing search criteria.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A list of details of available rooms.</returns>
    Task<IReadOnlyList<RoomDetailsResponse>> FindAvailableRoomsAsync
        (RoomAvailabilityRequest request, CancellationToken cancellationToken);
}