using BackendTZ.DTOs.Requests;
using BackendTZ.DTOs.Responses;
using BackendTZ.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BackendTZ.Controllers;

[Authorize]
[ApiController]
[Route("api/bookings")]
public class BookingController(IBookingManagementService bookingManagementService) : BaseController
{
    [HttpPost("create")]
    [ProducesResponseType(typeof(BookingResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<BookingResponse>> CreateBooking(CreateBookingRequest request, CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId()
                            ?? throw new UnauthorizedAccessException("User ID is not valid.");
        var response = await bookingManagementService.CreateBookingAsync(userId, request, cancellationToken);
        return Ok(response);
    }

    [HttpPut("change/{bookingId:guid}")]
    [ProducesResponseType(typeof(BookingResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<BookingResponse>> ChangeBooking(Guid bookingId, ChangeBookingRequest request, CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId()
                            ?? throw new UnauthorizedAccessException("User ID is not valid.");
        var isAdmin = User.IsInRole("Admin");
        var response = await bookingManagementService
            .ChangeBookingAsync(bookingId, userId, isAdmin, request, cancellationToken);
        return Ok(response);
    }

    [HttpPost("confirm/{bookingId:guid}")]
    [ProducesResponseType(typeof(BookingResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<BookingResponse>> ConfirmBooking(Guid bookingId, CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId()
                            ?? throw new UnauthorizedAccessException("User ID is not valid.");
        var isAdmin = User.IsInRole("Admin");
        var response = await bookingManagementService.ConfirmBookingAsync(bookingId, userId, isAdmin, cancellationToken);
        return Ok(response);
    }

    [HttpDelete("cancel/{bookingId:guid}")]
    [ProducesResponseType(typeof(BookingResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<BookingResponse>> CancelBooking(Guid bookingId, CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId()
                            ?? throw new UnauthorizedAccessException("User ID is not valid.");
        var isAdmin = User.IsInRole("Admin");
        var response = await bookingManagementService.CancelBookingAsync(bookingId, userId, isAdmin, cancellationToken);
        return Ok(response);
    }

    [HttpGet("get-details/{bookingId:guid}")]
    [ProducesResponseType(typeof(BookingResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<BookingResponse>> GetBookingDetail(Guid bookingId, CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId()
                            ?? throw new UnauthorizedAccessException("User ID is not valid.");
        var isAdmin = User.IsInRole("Admin");
        var response = await bookingManagementService.GetBookingDetailsAsync(bookingId, userId, isAdmin, cancellationToken);
        return Ok(response);
    }

    [HttpGet("get-all")]
    [ProducesResponseType(typeof(IEnumerable<BookingResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<IEnumerable<BookingResponse>>> GetAllBookings(CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId()
                            ?? throw new UnauthorizedAccessException("User ID is not valid.");
        var isAdmin = User.IsInRole("Admin");
        var response = await bookingManagementService.GetUserBookingsAsync(userId, isAdmin, cancellationToken);
        return Ok(response);
    }
}