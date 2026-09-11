using BackendTZ.Entities;

namespace BackendTZ.Services.Interfaces;

/// <summary>
/// Defines the contract for a pricing service that calculates the total rental cost for a booking, including time-based adjustments and additional services.
/// </summary>
public interface IPricingService
{
    /// <summary>
    /// Calculates the total rental cost for a booking, including the time-based
    /// discount/surcharge on the room's base rate and the cost of selected services.
    /// </summary>
    /// <param name="room">The room being booked, including its base hourly rate.</param>
    /// <param name="startTime">The start date and time of the booking.</param>
    /// <param name="endTime">The end date and time of the booking. Must be after <paramref name="startTime"/>.</param>
    /// <param name="selectedServices">The additional services selected for this booking (e.g. projector, Wi-Fi).</param>
    /// <returns>A breakdown of the calculated cost, including the base cost, time-based adjustment, services cost, and total.</returns>
    /// <exception cref="ArgumentException">Thrown when <paramref name="endTime"/> is not after <paramref name="startTime"/>.</exception>
    CostBreakdown CalculateRentalCost(
        Room room,
        DateTime startTime,
        DateTime endTime,
        IReadOnlyList<Service> selectedServices);

}

/// <summary>
/// Represents a breakdown of a booking's calculated rental cost.
/// </summary>
/// <param name="BaseCost">The base cost of the room for the full duration, before any time-based discount or surcharge.</param>
/// <param name="TimeAdjustment">The net discount (negative) or surcharge (positive) applied due to the time-of-day pricing rules.</param>
/// <param name="ServicesCost">The total cost of the selected additional services.</param>
/// <param name="TotalCost">The final total cost of the booking (BaseCost + TimeAdjustment + ServicesCost).</param>
public record CostBreakdown(decimal BaseCost, decimal TimeAdjustment, decimal ServicesCost, decimal TotalCost);