using BackendTZ.DTOs.Requests;
using BackendTZ.DTOs.Responses;

namespace BackendTZ.Services.Interfaces;

/// <summary>
/// Defines the contract for managing bookings, including creating, changing, confirming, cancelling, and retrieving booking details.
/// </summary>
public interface IBookingManagementService
{
    /// <summary>
    /// Creates a new booking for a user.
    /// </summary>
    /// <param name="userId">The ID of the user creating the booking.</param>
    /// <param name="request">The booking request details.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The details of the created booking.</returns>
    Task<BookingDetailsResponse> CreateBookingAsync
        (Guid userId, CreateBookingRequest request, CancellationToken cancellationToken);

    /// <summary>
    /// Changes an existing booking for a user. Only the user who created the booking or an admin can change it.
    /// </summary>
    /// <param name="bookingId">The ID of the booking to change.</param>
    /// <param name="userId">The ID of the user requesting the change.</param>
    /// <param name="isAdmin">Indicates whether the user is an admin.</param>
    /// <param name="request">The booking change request details.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The details of the changed booking.</returns>
    Task<BookingDetailsResponse> ChangeBookingAsync
        (Guid bookingId, Guid userId, bool isAdmin, ChangeBookingRequest request, CancellationToken cancellationToken);

    /// <summary>
    /// Confirms an existing booking. Only the user who created the booking or an admin can confirm it.
    /// </summary>
    /// <param name="bookingId">The ID of the booking to confirm.</param>
    /// <param name="userId">The ID of the user requesting the confirmation.</param>
    /// <param name="isAdmin">Indicates whether the user is an admin.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The details of the confirmed booking.</returns>
    Task<BookingDetailsResponse> ConfirmBookingAsync
        (Guid bookingId, Guid userId, bool isAdmin, CancellationToken cancellationToken);

    /// <summary>
    /// Cancels an existing booking. Only the user who created the booking or an admin can cancel it.
    /// </summary>
    /// <param name="bookingId">The ID of the booking to cancel.</param>
    /// <param name="userId">The ID of the user requesting the cancellation.</param>
    /// <param name="isAdmin">Indicates whether the user is an admin.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>True if the booking was successfully cancelled; otherwise, false.</returns>
    Task<bool> CancelBookingAsync
        (Guid bookingId, Guid userId, bool isAdmin, CancellationToken cancellationToken);

    /// <summary>
    /// Gets the details of a specific booking. Only the user who created the booking or an admin can view it.
    /// </summary>
    /// <param name="bookingId">The ID of the booking to retrieve.</param>
    /// <param name="userId">The ID of the user requesting the booking details.</param>
    /// <param name="isAdmin">Indicates whether the user is an admin.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The details of the specified booking.</returns>
    Task<BookingDetailsResponse> GetBookingDetailsAsync
        (Guid bookingId, Guid userId, bool isAdmin, CancellationToken cancellationToken);

    /// <summary>
    /// Gets all bookings for a specific user. Admins can view all bookings.
    /// </summary>
    /// <param name="userId">The ID of the user whose bookings to retrieve.</param>
    /// <param name="isAdmin">Indicates whether the user is an admin.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A list of booking details for the specified user.</returns>
    Task<IReadOnlyList<BookingDetailsResponse>> GetUserBookingsAsync
        (Guid userId, bool isAdmin, CancellationToken cancellationToken);
}