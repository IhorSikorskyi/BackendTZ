using BackendTZ.Entities;
using BackendTZ.Services.Interfaces;

namespace BackendTZ.Services.Implementations;

/// <summary>
/// Calculates rental cost based on time-of-day rules and selected additional services.
/// This service is pure business logic: it has no dependency on the database or any I/O,
/// which makes it trivially unit-testable.
/// </summary>
public class PricingService : IPricingService
{
    private const decimal EveningDiscountRate = 0.20m;   // 18:00–23:00 -> -20%
    private const decimal MorningDiscountRate = 0.10m;   // 06:00–09:00 -> -10%
    private const decimal PeakSurchargeRate = 0.15m;     // 12:00–14:00 -> +15%

    private static readonly TimeOnly MorningStart = new(6, 0);
    private static readonly TimeOnly StandardStart = new(9, 0);
    private static readonly TimeOnly PeakStart = new(12, 0);
    private static readonly TimeOnly PeakEnd = new(14, 0);
    private static readonly TimeOnly EveningStart = new(18, 0);
    private static readonly TimeOnly EveningEnd = new(23, 0);

    /// <inheritdoc/>
    public CostBreakdown CalculateRentalCost(
            Room room, 
            DateTime startTime, 
            DateTime endTime, 
            IReadOnlyList<Service> selectedServices)
    {
        if (endTime <= startTime)
        {
            throw new ArgumentException("End time must be after start time.", nameof(endTime));
        }

        var baseCost = CalculateTimeSlicedBaseCost(room.BaseHourlyRate, startTime, endTime);
        var servicesCost = selectedServices.Sum(s => s.Price);

        return new CostBreakdown(
            BaseCost: room.BaseHourlyRate * (decimal)(endTime - startTime).TotalHours,
            TimeAdjustment: baseCost - room.BaseHourlyRate * (decimal)(endTime - startTime).TotalHours,
            ServicesCost: servicesCost,
            TotalCost: baseCost + servicesCost);
    }

    /// <summary>
    /// Splits the booking interval into hourly slices and applies the correct
    /// discount/surcharge rate to each slice, based on which time-of-day band it falls into.
    /// This correctly handles bookings that span multiple pricing bands (e.g. 08:00-15:00).
    /// </summary>
    /// <param name="hourlyRate">The base hourly rate of the room.</param>
    /// <param name="startTime">The start time of the booking.</param>
    /// <param name="endTime">The end time of the booking.</param>
    /// <returns>The total cost after applying time-of-day adjustments.</returns>
    private static decimal CalculateTimeSlicedBaseCost(decimal hourlyRate, DateTime startTime, DateTime endTime)
    {
        decimal total = 0m;
        var cursor = startTime;

        while (cursor < endTime)
        {
            var sliceEnd = cursor.AddHours(1) > endTime ? endTime : cursor.AddHours(1);
            var sliceHours = (decimal)(sliceEnd - cursor).TotalHours;
            var rateMultiplier = GetRateMultiplier(TimeOnly.FromDateTime(cursor));

            total += hourlyRate * sliceHours * rateMultiplier;
            cursor = sliceEnd;
        }

        return total;
    }

    /// <summary>
    /// Returns the appropriate multiplier for the base hourly rate based on the time of day.
    /// </summary>
    /// <param name="timeOfDay">The time of day for which to get the rate multiplier.</param>
    /// <returns>The rate multiplier to apply to the base hourly rate.</returns>
    private static decimal GetRateMultiplier(TimeOnly timeOfDay)
    {
        if (timeOfDay >= PeakStart && timeOfDay < PeakEnd)
        {
            return 1 + PeakSurchargeRate;
        }

        if (timeOfDay >= EveningStart && timeOfDay < EveningEnd)
        {
            return 1 - EveningDiscountRate;
        }

        if (timeOfDay >= MorningStart && timeOfDay < StandardStart)
        {
            return 1 - MorningDiscountRate;
        }

        return 1m;
    }
}