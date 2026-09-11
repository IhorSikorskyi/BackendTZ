using BackendTZ.DTOs.Requests;
using BackendTZ.DTOs.Responses;
using BackendTZ.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BackendTZ.Controllers;

/// <summary>
/// Controller responsible for handling booking-related operations such as creating, changing, confirming, canceling, and retrieving bookings.
/// </summary>
/// <param name="bookingManagementService">The booking management service used to perform booking operations.</param>
[Authorize]
[ApiController]
[Route("api/bookings")]
public class BookingController(IBookingManagementService bookingManagementService) : BaseController
{
    /// <summary>
    /// Creates a new booking for the current user.
    /// </summary>
    /// <param name="request">The request containing booking details.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The created booking response.</returns>
    /// <exception cref="UnauthorizedAccessException">Thrown when the user ID is not valid.</exception>
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

    /// <summary>
    /// Changes an existing booking for the current user.
    /// </summary>
    /// <param name="bookingId">The ID of the booking to change.</param>
    /// <param name="request">The request containing the updated booking details.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The updated booking response.</returns>
    /// <exception cref="UnauthorizedAccessException">Thrown when the user ID is not valid.</exception>
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

    /// <summary>
    /// Confirms an existing booking for the current user.
    /// </summary>
    /// <param name="bookingId">The ID of the booking to confirm.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The confirmed booking response.</returns>
    /// <exception cref="UnauthorizedAccessException">Thrown when the user ID is not valid.</exception>
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

    /// <summary>
    /// Cancels an existing booking for the current user.
    /// </summary>
    /// <param name="bookingId">The ID of the booking to cancel.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The canceled booking response.</returns>
    /// <exception cref="UnauthorizedAccessException">Thrown when the user ID is not valid.</exception>
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

    /// <summary>
    /// Retrieves the details of a specific booking for the current user.
    /// </summary>
    /// <param name="bookingId">The ID of the booking to retrieve details for.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The booking response containing the booking details.</returns>
    /// <exception cref="UnauthorizedAccessException">Thrown when the user ID is not valid.</exception>
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

    /// <summary>
    /// Retrieves all bookings for the current user. If the user is an admin, retrieves all bookings in the system.
    /// </summary>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A list of booking responses containing the booking details.</returns>
    /// <exception cref="UnauthorizedAccessException">Thrown when the user ID is not valid.</exception>
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