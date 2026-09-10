using BackendTZ.DTOs.Requests;
using BackendTZ.DTOs.Responses;
using BackendTZ.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BackendTZ.Controllers
{
    [ApiController]
    [Route("api/rooms")]
    public class RoomsController(IRoomManagementService roomManagementService) : BaseController
    {
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

        [HttpPut("update/{roomId:guid}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(RoomDetailsResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<RoomDetailsResponse>> UpdateRoomAsync(Guid roomId, UpdateRoomRequest request, CancellationToken cancellationToken)
        {
            var response = await roomManagementService.UpdateRoomAsync(roomId, request, cancellationToken);
            return Ok(response);
        }

        [HttpDelete("delete/{roomId:guid}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(RoomDetailsResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<RoomDetailsResponse>> DeleteRoomAsync(Guid roomId, CancellationToken cancellationToken)
        {
            var response = await roomManagementService.DeleteRoomAsync(roomId, cancellationToken);
            return Ok(response);
        }

        [HttpGet("get/{roomId:guid}")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(RoomDetailsResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<RoomDetailsResponse>> GetRoomAsync(Guid roomId, CancellationToken cancellationToken)
        {
            var response = await roomManagementService.GetRoomByIdAsync(roomId, cancellationToken);
            return Ok(response);
        }

        [HttpGet("get-all")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(IEnumerable<RoomDetailsResponse>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<RoomDetailsResponse>>> GetAllRoomsAsync(CancellationToken cancellationToken)
        {
            var response = await roomManagementService.GetAllRoomsAsync(cancellationToken);
            return Ok(response);
        }

        [HttpPost("find-available")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(IEnumerable<RoomDetailsResponse>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<RoomDetailsResponse>>> FindAvailableRoomsAsync(RoomAvailabilityRequest request, CancellationToken cancellationToken)
        {
            var response = await roomManagementService.FindAvailableRoomsAsync(request, cancellationToken);
            return Ok(response);
        }
    }
}
