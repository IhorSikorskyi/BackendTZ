using BackendTZ.DTOs.Requests;
using BackendTZ.DTOs.Responses;
using BackendTZ.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BackendTZ.Controllers;

/// <summary>
/// Controller responsible for handling room management operations such as creating, updating, deleting, and retrieving room details.
/// </summary>
/// <param name="roomManagementService">The service responsible for managing room operations.</param>
[ApiController]
[Route("api/rooms")]
public class RoomsController(IRoomManagementService roomManagementService) : BaseController
{
    /// <summary>
    /// Creates a new room with the provided details. Only accessible by users with the "Admin" role.
    /// </summary>
    /// <param name="request">The request containing the details of the room to be created.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The details of the newly created room.</returns>
    [HttpPost("create")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(RoomDetailsResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<RoomDetailsResponse>> CreateRoomAsync
        (CreateRoomRequest request, CancellationToken cancellationToken)
    {
        var response = await roomManagementService.CreateRoomAsync(request, cancellationToken);
        return Ok(response);
    }

    /// <summary>
    /// Updates the details of an existing room identified by its ID. Only accessible by users with the "Admin" role.
    /// </summary>
    /// <param name="roomId">The ID of the room to be updated.</param>
    /// <param name="request">The request containing the updated details of the room.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The details of the updated room.</returns>
    [HttpPut("update/{roomId:guid}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(RoomDetailsResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<RoomDetailsResponse>> UpdateRoomAsync(Guid roomId, UpdateRoomRequest request,
        CancellationToken cancellationToken)
    {
        var response = await roomManagementService.UpdateRoomAsync(roomId, request, cancellationToken);
        return Ok(response);
    }

    /// <summary>
    /// Deletes an existing room identified by its ID. Only accessible by users with the "Admin" role.
    /// </summary>
    /// <param name="roomId">The ID of the room to be deleted.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The details of the deleted room.</returns>
    [HttpDelete("delete/{roomId:guid}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(RoomDetailsResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<RoomDetailsResponse>> DeleteRoomAsync(Guid roomId,
        CancellationToken cancellationToken)
    {
        var response = await roomManagementService.DeleteRoomAsync(roomId, cancellationToken);
        return Ok(response);
    }

    /// <summary>
    /// Retrieves the details of a specific room identified by its ID. Accessible by all users, including anonymous users.
    /// </summary>
    /// <param name="roomId">The ID of the room to retrieve.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The details of the specified room.</returns>
    [HttpGet("get/{roomId:guid}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(RoomDetailsResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<RoomDetailsResponse>> GetRoomAsync(Guid roomId, CancellationToken cancellationToken)
    {
        var response = await roomManagementService.GetRoomByIdAsync(roomId, cancellationToken);
        return Ok(response);
    }

    /// <summary>
    /// Retrieves the details of all rooms. Accessible by all users, including anonymous users.
    /// </summary>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The details of all rooms.</returns>
    [HttpGet("get-all")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(IEnumerable<RoomDetailsResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<RoomDetailsResponse>>> GetAllRoomsAsync(
        CancellationToken cancellationToken)
    {
        var response = await roomManagementService.GetAllRoomsAsync(cancellationToken);
        return Ok(response);
    }

    /// <summary>
    /// Finds available rooms based on the specified criteria, such as date, time range, and minimum capacity. Accessible by all users, including anonymous users.
    /// </summary>
    /// <param name="request">The criteria for finding available rooms.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The details of the available rooms that match the specified criteria.</returns>
    [HttpPost("find-available")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(IEnumerable<RoomDetailsResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<RoomDetailsResponse>>> FindAvailableRoomsAsync(
        RoomAvailabilityRequest request, CancellationToken cancellationToken)
    {
        var response = await roomManagementService.FindAvailableRoomsAsync(request, cancellationToken);
        return Ok(response);
    }
}

