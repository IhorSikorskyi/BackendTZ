namespace BackendTZ.DTOs.Responses;

/// <summary>
/// Base class for all booking related responses.
/// </summary>
public abstract record BookingResponse;

/// <summary>
/// Represents the cost breakdown of a booking, useful for transparency towards the client.
/// </summary>
/// <param name="BaseCost">The base cost before any discounts/surcharges, proportional to duration.</param>
/// <param name="TimeAdjustment">The discount or surcharge applied due to the time slot (negative = discount).</param>
/// <param name="ServicesCost">The total cost of the selected additional services.</param>
/// <param name="TotalCost">The final total cost of the booking.</param>
public record CostBreakdownResponse(
    decimal BaseCost,
    decimal TimeAdjustment,
    decimal ServicesCost,
    decimal TotalCost
);

/// <summary>
/// Represents a confirmed room booking.
/// </summary>
/// <param name="Id">The unique identifier of the booking.</param>
/// <param name="RoomId">The ID of the booked room.</param>
/// <param name="RoomName">The name of the booked room.</param>
/// <param name="Date">The date of the booking.</param>
/// <param name="StartTime">The start time of the booking.</param>
/// <param name="EndTime">The end time of the booking.</param>
/// <param name="Services">The additional services selected for this booking.</param>
/// <param name="Cost">The cost breakdown of this booking.</param>
public record BookingDetailsResponse(
    Guid Id,
    Guid RoomId,
    string RoomName,
    DateOnly Date,
    TimeOnly StartTime,
    TimeOnly EndTime,
    List<ServiceItemResponse> Services,
    CostBreakdownResponse Cost
) : BookingResponse;