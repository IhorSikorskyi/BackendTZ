using BackendTZ.DTOs.Requests;
using BackendTZ.DTOs.Responses;
using BackendTZ.Entities;
using BackendTZ.Exceptions;
using BackendTZ.Repositories.Interfaces;
using BackendTZ.Services.Interfaces;

namespace BackendTZ.Services.Implementations;

/// <summary>
/// Represents the service responsible for managing bookings, including creating, changing, confirming, and cancelling bookings.
/// </summary>
/// <param name="bookingRepository">The repository for managing booking data.</param>
/// <param name="serviceRepository">The repository for managing service data.</param>
/// <param name="roomRepository">The repository for managing room data.</param>
/// <param name="unitOfWork">The unit of work for managing transactions.</param>
/// <param name="pricingService">The service for calculating pricing.</param>
public class BookingManagementService(
    IBookingRepository bookingRepository,
    IServiceRepository serviceRepository,
    IRoomRepository roomRepository,
    IUnitOfWork unitOfWork,
    IPricingService pricingService) : IBookingManagementService
{
    /// <inheritdoc/>
    public async Task<BookingDetailsResponse> CreateBookingAsync
        (Guid userId, CreateBookingRequest request, CancellationToken cancellationToken)
    {
        if(!request.IsValidTimeRange())
        {
            throw new InvalidOperationException("The requested time range is invalid.");
        }

        var searchStart = request.Date.ToDateTime(request.StartTime);
        var searchEnd = request.Date.ToDateTime(request.EndTime);

        if(!await bookingRepository.IsAvailableAsync
               (request.RoomId, searchStart, searchEnd, null, cancellationToken))
        {
            throw new InvalidOperationException("The room is not available for the requested time period.");
        }

        var selectedServices = request.ServiceIds != null && request.ServiceIds.Count > 0
            ? await serviceRepository.GetByIdsAsync(request.ServiceIds, cancellationToken)
            : new List<Service>();

        if (selectedServices.Count != (request.ServiceIds?.Count ?? 0))
        {
            throw new NotFoundException("One or more selected services were not found.");
        }

        var room = await roomRepository.GetByIdAsync(request.RoomId, cancellationToken)
            ?? throw new NotFoundException("The requested room was not found.");

        var costBreakdown = pricingService.CalculateRentalCost(room, searchStart, searchEnd, selectedServices);

        var booking = new Booking
        {
            RoomId = request.RoomId,
            UserId = userId,
            StartTime = searchStart,
            EndTime = searchEnd,
            Status = BookingStatus.Pending,
            TotalCost = costBreakdown.TotalCost,
            BookingServices = selectedServices.Select(s => new BookingService
            {
                ServiceId = s.Id,
                PriceAtBooking = s.Price
            }).ToList()
        };

        await bookingRepository.AddAsync(booking, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return MapToBookingDetailsResponse(booking);
    }

    /// <inheritdoc/>
    public async Task<BookingDetailsResponse> ChangeBookingAsync
        (Guid bookingId, Guid userId, bool isAdmin, ChangeBookingRequest request, CancellationToken cancellationToken)
    {
        ValidateHasAtLeastOneField(request);

        var booking = await bookingRepository.GetByIdAsync(bookingId, cancellationToken)
                      ?? throw new NotFoundException("Booking not found.");

        EnsureUserCanModify(booking, userId, isAdmin);

        var timeOrRoomChanged = request.RoomId != null
                                || request.Date != null
                                || request.StartTime != null
                                || request.EndTime != null;

        ApplyRoomChange(booking, request);
        ApplyDateTimeChange(booking, request);

        if (timeOrRoomChanged)
        {
            await EnsureRoomAvailableAsync(booking, bookingId, cancellationToken);
        }

        var selectedServices = await ResolveServicesAsync(booking, request, cancellationToken);

        if (timeOrRoomChanged || request.ServiceIds != null)
        {
            await RecalculateCostAsync(booking, selectedServices, cancellationToken);
        }

        bookingRepository.Update(booking);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return MapToBookingDetailsResponse(booking);
    }

    /// <inheritdoc/>
    public async Task<BookingDetailsResponse> ConfirmBookingAsync
        (Guid bookingId, Guid userId, bool isAdmin, CancellationToken cancellationToken)
    {
        var booking = await bookingRepository.GetByIdAsync(bookingId, cancellationToken)
                      ?? throw new NotFoundException("Booking not found.");

        EnsureUserCanModify(booking, userId, isAdmin);
        booking.Status = BookingStatus.Confirmed;
        bookingRepository.Update(booking);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return MapToBookingDetailsResponse(booking);
    }

    /// <inheritdoc/>
    public async Task<bool> CancelBookingAsync
        (Guid bookingId, Guid userId, bool isAdmin, CancellationToken cancellationToken)
    {
        var booking = await bookingRepository.GetByIdAsync(bookingId, cancellationToken)
                      ?? throw new NotFoundException("Booking not found.");

        EnsureUserCanModify(booking, userId, isAdmin);

        booking.Status = BookingStatus.Cancelled;
        bookingRepository.Update(booking);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }

    /// <inheritdoc/>
    public async Task<BookingDetailsResponse> GetBookingDetailsAsync
        (Guid bookingId, Guid userId, bool isAdmin, CancellationToken cancellationToken)
    {
        var booking = await bookingRepository.GetByIdAsync(bookingId, cancellationToken)
                      ?? throw new NotFoundException("Booking not found.");

        EnsureUserCanModify(booking, userId, isAdmin);

        return MapToBookingDetailsResponse(booking);
    }

    /// <inheritdoc/>
    public async Task<IReadOnlyList<BookingDetailsResponse>> GetUserBookingsAsync
        (Guid userId, bool isAdmin, CancellationToken cancellationToken)
    {
        var bookings = await bookingRepository.GetUserBookingsAsync(userId, cancellationToken);

        if (!isAdmin)
        {
            bookings = bookings.Where(b => b.UserId == userId).ToList();
        }

        return bookings.Select(MapToBookingDetailsResponse).ToList();
    }

    /// <summary>
    /// Validates that the ChangeBookingRequest has at least one field to change. If none of the fields are provided, an InvalidOperationException is thrown.
    /// </summary>
    /// <param name="request">The change booking request to validate.</param>
    /// <exception cref="InvalidOperationException">Thrown if no fields are provided to change.</exception>
    private static void ValidateHasAtLeastOneField(ChangeBookingRequest request)
    {
        var hasAnyField = request.RoomId != null
            || request.Date != null
            || request.StartTime != null
            || request.EndTime != null
            || (request.ServiceIds != null && request.ServiceIds.Count > 0);

        if (!hasAnyField)
        {
            throw new InvalidOperationException("At least one field must be provided to change the booking.");
        }
    }

    /// <summary>
    /// Ensures that the user has permission to modify the booking. If the user is not the owner of the booking and is not an admin, a ForbiddenException is thrown.
    /// </summary>
    /// <param name="booking">The booking to check.</param>
    /// <param name="userId">The ID of the user attempting to modify the booking.</param>
    /// <param name="isAdmin">Indicates whether the user is an admin.</param>
    /// <exception cref="ForbiddenException">Thrown if the user does not have permission to modify the booking.</exception>
    private static void EnsureUserCanModify(Booking booking, Guid userId, bool isAdmin)
    {
        if (booking.UserId != userId && !isAdmin)
        {
            throw new ForbiddenException("You are not allowed to modify this booking.");
        }
    }
    
    /// <summary>
    /// Applies changes to the room of the booking based on the provided ChangeBookingRequest. If a new RoomId is provided, the current room is marked as available and the booking's RoomId is updated.
    /// </summary>
    /// <param name="booking">The booking to update.</param>
    /// <param name="request">The change booking request containing the new room information.</param>
    private static void ApplyRoomChange(Booking booking, ChangeBookingRequest request)
    {
        if (request.RoomId != null)
        {
            booking.Room.IsAvailable = true;
            booking.RoomId = request.RoomId.Value;
        }
    }

    /// <summary>
    /// Applies changes to the date and time of the booking based on the provided ChangeBookingRequest. Validates that the new date is not in the past and that the end time is after the start time. If valid, updates the booking's StartTime and EndTime.
    /// </summary>
    /// <param name="booking">The booking to update.</param>
    /// <param name="request">The change booking request containing the new date and time information.</param>
    /// <exception cref="ValidationException">Thrown if the new date is in the past or if the end time is not after the start time.</exception>
    private static void ApplyDateTimeChange(Booking booking, ChangeBookingRequest request)
    {
        if (request.Date == null && request.StartTime == null && request.EndTime == null)
        {
            return;
        }

        var todayCheck = DateOnly.FromDateTime(DateTime.UtcNow);
        var newDate = request.Date ?? DateOnly.FromDateTime(booking.StartTime);

        if (newDate < todayCheck)
        {
            throw new ValidationException("The booking date cannot be in the past.");
        }

        var newStartTime = request.StartTime ?? TimeOnly.FromDateTime(booking.StartTime);
        var newEndTime = request.EndTime ?? TimeOnly.FromDateTime(booking.EndTime);

        if (newEndTime <= newStartTime)
        {
            throw new ValidationException("End time must be after start time.");
        }

        booking.StartTime = newDate.ToDateTime(newStartTime);
        booking.EndTime = newDate.ToDateTime(newEndTime);
    }

    /// <summary>
    /// Ensures that the room is available for the requested time period. If the room is not available, an InvalidOperationException is thrown.
    /// </summary>
    /// <param name="booking">The booking to check for room availability.</param>
    /// <param name="bookingId">The ID of the booking.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <exception cref="InvalidOperationException">Thrown if the room is not available for the requested time period.</exception>
    private async Task EnsureRoomAvailableAsync(Booking booking, Guid bookingId, CancellationToken cancellationToken)
    {
        var isAvailable = await bookingRepository.IsAvailableAsync(
            booking.RoomId, booking.StartTime, booking.EndTime, bookingId, cancellationToken);

        if (!isAvailable)
        {
            throw new InvalidOperationException("The room is not available for the requested time period.");
        }
    }

    private static BookingDetailsResponse MapToBookingDetailsResponse(Booking booking)
    {
        var date = DateOnly.FromDateTime(booking.StartTime);
        var startTime = TimeOnly.FromDateTime(booking.StartTime);
        var endTime = TimeOnly.FromDateTime(booking.EndTime);

        var services = booking.BookingServices
            .Select(bs => new ServiceItemResponse(
                bs.Service.Id,
                bs.Service.Name,
                bs.Service.Description ?? string.Empty,
                bs.PriceAtBooking
            ))
            .ToList();

        var cost = new CostBreakdownResponse(
            booking.BaseCost,
            booking.TimeAdjustment,
            booking.ServicesCost,
            booking.TotalCost
        );

        return new BookingDetailsResponse(
            booking.Id,
            booking.RoomId,
            booking.Room.Name,
            date,
            startTime,
            endTime,
            services,
            cost
        );
    }

    /// <summary>
    /// Resolves the list of services for a booking based on the provided ChangeBookingRequest. If service IDs are provided in the request, it replaces the existing services with the new ones. If no service IDs are provided, it retains the existing services.
    /// </summary>
    /// <param name="booking">The booking for which to resolve services.</param>
    /// <param name="request">The change booking request containing the service IDs.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>A task representing the asynchronous operation, containing the list of resolved services.</returns>
    private async Task<IReadOnlyList<Service>> ResolveServicesAsync(
        Booking booking, ChangeBookingRequest request, CancellationToken cancellationToken)
    {
        if (request.ServiceIds != null)
        {
            return await ReplaceBookingServicesAsync(booking, request.ServiceIds, cancellationToken);
        }

        var existingServiceIds = booking.BookingServices
            .Select(bs => bs.ServiceId)
            .ToList();

        return existingServiceIds.Count > 0
            ? await serviceRepository.GetByIdsAsync(existingServiceIds, cancellationToken)
            : new List<Service>();
    }

    /// <summary>
    /// Replaces the existing services of a booking with a new list of services based on the provided service IDs. If any of the provided service IDs do not correspond to existing services, a NotFoundException is thrown.
    /// </summary>
    /// <param name="booking">The booking for which to replace services.</param>
    /// <param name="serviceIds">The list of service IDs to replace the existing services with.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>A task representing the asynchronous operation, containing the list of replaced services.</returns>
    /// <exception cref="NotFoundException"></exception>
    private async Task<IReadOnlyList<Service>> ReplaceBookingServicesAsync(
        Booking booking, List<Guid> serviceIds, CancellationToken cancellationToken)
    {
        var selectedServices = serviceIds.Count > 0
            ? await serviceRepository.GetByIdsAsync(serviceIds, cancellationToken)
            : new List<Service>();

        if (selectedServices.Count != serviceIds.Count)
        {
            throw new NotFoundException("One or more selected services were not found.");
        }

        booking.BookingServices.Clear();
        foreach (var service in selectedServices)
        {
            booking.BookingServices.Add(new BookingService
            {
                ServiceId = service.Id,
                PriceAtBooking = service.Price
            });
        }

        return selectedServices;
    }

    /// <summary>
    /// Recalculates the total cost of a booking based on the room, time range, and selected services. If the room is not found, a NotFoundException is thrown.
    /// </summary>
    /// <param name="booking">The booking for which to recalculate the total cost.</param>
    /// <param name="selectedServices">The list of selected services.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <exception cref="NotFoundException"></exception>
    private async Task RecalculateCostAsync(
        Booking booking, IReadOnlyList<Service> selectedServices, CancellationToken cancellationToken)
    {
        var room = await roomRepository.GetByIdAsync(booking.RoomId, cancellationToken)
            ?? throw new NotFoundException("The requested room was not found.");

        var costBreakdown = pricingService.CalculateRentalCost(
            room, booking.StartTime, booking.EndTime, selectedServices);

        booking.TotalCost = costBreakdown.TotalCost;
    }
}