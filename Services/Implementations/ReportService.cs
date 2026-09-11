using BackendTZ.DTOs.Requests;
using BackendTZ.DTOs.Responses;
using BackendTZ.Entities;
using BackendTZ.Repositories.Interfaces;
using BackendTZ.Services.Interfaces;
using System.Globalization;
using BookingStatus = BackendTZ.Entities.BookingStatus;

namespace BackendTZ.Services.Implementations;

/// <summary>
/// Represents the implementation of the IReportService interface for generating various reports related to bookings, rooms, and services.
/// </summary>
/// <param name="bookingRepository">The repository for accessing booking data.</param>
/// <param name="roomRepository">The repository for accessing room data.</param>
/// <param name="serviceRepository">The repository for accessing service data.</param>
public class ReportService(
        IBookingRepository bookingRepository, 
        IRoomRepository roomRepository, 
        IServiceRepository serviceRepository) : IReportService
{
    /// <inheritdoc/>
    public async Task<RevenueReportResponse> GetRevenueReportAsync
        (RevenueReportRequest request, CancellationToken cancellationToken = default)
    {
        var (start, endExclusive) = ToDateTimeRange(request.DateFrom, request.DateTo);

        var bookings = (await bookingRepository.GetBookingsForPeriodAsync(
                start, endExclusive, request.RoomId, cancellationToken))
            .Where(CountsTowardMetrics)
            .ToList();

        var totalRevenue = bookings.Sum(b => b.TotalCost);
        var baseRentalRevenue = bookings.Sum(b => b.BaseCost + b.TimeAdjustment);
        var servicesRevenue = bookings.Sum(b => b.ServicesCost);

        var breakdown = bookings
            .GroupBy(b => GetPeriodLabel(b.StartTime, request.GroupBy))
            .OrderBy(g => g.Key)
            .Select(g => new RevenueByPeriodResponse(
                Period: g.Key,
                Revenue: g.Sum(b => b.TotalCost),
                BookingsCount: g.Count()))
            .ToList();

        var byRoom = bookings
            .GroupBy(b => b.Room)
            .Where(g => request.IncludeInactiveRooms || g.Key.IsActive)
            .Select(g => new RevenueByRoomResponse(
                RoomId: g.Key.Id,
                RoomName: g.Key.Name,
                Revenue: g.Sum(b => b.TotalCost),
                IsActive: g.Key.IsActive))
            .OrderByDescending(r => r.Revenue)
            .ToList();

        return new RevenueReportResponse(
            request.DateFrom,
            request.DateTo,
            totalRevenue,
            baseRentalRevenue,
            servicesRevenue,
            breakdown,
            byRoom);
    }

    /// <inheritdoc/>
    public async Task<RoomUtilizationReportResponse> GetRoomUtilizationReportAsync
        (RoomUtilizationReportRequest request, CancellationToken cancellationToken = default)
    {
        var (start, endExclusive) = ToDateTimeRange(request.DateFrom, request.DateTo);
        var totalHoursInPeriod = (decimal)(endExclusive - start).TotalHours;

        var rooms = await roomRepository.GetRoomsAsync(
            request.IncludeInactiveRooms, request.RoomId, cancellationToken);

        var bookings = (await bookingRepository.GetBookingsForPeriodAsync(
                start, endExclusive, request.RoomId, cancellationToken))
            .Where(CountsTowardMetrics)
            .ToList();

        var bookedHoursByRoom = bookings
            .GroupBy(b => b.RoomId)
            .ToDictionary(g => g.Key, g => (decimal)g.Sum(b => (b.EndTime - b.StartTime).TotalHours));

        var roomRows = rooms.Select(room =>
        {
            var availableHours = room.IsAvailable ? totalHoursInPeriod : 0m;
            var bookedHours = bookedHoursByRoom.GetValueOrDefault(room.Id, 0m);
            var utilizationPercent = availableHours > 0
                ? Math.Round(bookedHours / availableHours * 100m, 2)
                : 0m;

            return new RoomUtilizationResponse(
                RoomId: room.Id,
                RoomName: room.Name,
                AvailableHours: availableHours,
                BookedHours: bookedHours,
                UtilizationPercent: utilizationPercent,
                IsActive: room.IsActive);
        })
        .OrderByDescending(r => r.UtilizationPercent)
        .ToList();

        return new RoomUtilizationReportResponse(request.DateFrom, request.DateTo, roomRows);
    }

    /// <inheritdoc/>
    public async Task<PopularRoomsReportResponse> GetPopularRoomsReportAsync
        (PopularRoomsReportRequest request, CancellationToken cancellationToken = default)
    {
        var (start, endExclusive) = ToDateTimeRange(request.DateFrom, request.DateTo);

        var bookings = (await bookingRepository.GetBookingsForPeriodAsync(
                start, endExclusive, roomId: null, cancellationToken))
            .Where(CountsTowardMetrics)
            .ToList();

        var groupedByRoom = bookings.GroupBy(b => b.Room).ToList();

        var byBookingsCount = groupedByRoom
            .Select(g => new RoomByBookingsCountResponse(g.Key.Id, g.Key.Name, g.Count()))
            .OrderByDescending(r => r.BookingsCount)
            .Take(request.TopN)
            .ToList();

        var byRevenue = groupedByRoom
            .Select(g => new RoomByRevenueResponse(g.Key.Id, g.Key.Name, g.Sum(b => b.TotalCost)))
            .OrderByDescending(r => r.Revenue)
            .Take(request.TopN)
            .ToList();

        return new PopularRoomsReportResponse(request.DateFrom, request.DateTo, byBookingsCount, byRevenue);
    }

    /// <inheritdoc/>
    public async Task<PopularServicesReportResponse> GetPopularServicesReportAsync
        (PopularServicesReportRequest request, CancellationToken cancellationToken = default)
    {
        var (start, endExclusive) = ToDateTimeRange(request.DateFrom, request.DateTo);

        var bookings = (await bookingRepository.GetBookingsForPeriodAsync(
                start, endExclusive, roomId: null, cancellationToken))
            .Where(CountsTowardMetrics)
            .ToList();

        var bookingServices = bookings.SelectMany(b => b.BookingServices).ToList();

        var serviceIds = bookingServices.Select(bs => bs.ServiceId).Distinct().ToList();
        var services = await serviceRepository.GetByIdsAsync(serviceIds, cancellationToken);
        var servicesById = services.ToDictionary(s => s.Id);

        var usage = bookingServices
            .GroupBy(bs => bs.ServiceId)
            .Where(g => servicesById.ContainsKey(g.Key))
            .Select(g =>
            {
                var service = servicesById[g.Key];
                return new ServiceUsageResponse(
                    ServiceId: service.Id,
                    ServiceName: service.Name,
                    TimesOrdered: g.Count(),
                    TotalRevenue: g.Sum(bs => bs.PriceAtBooking),
                    IsActive: service.IsActive);
            })
            .OrderByDescending(s => s.TimesOrdered)
            .ToList();

        return new PopularServicesReportResponse(request.DateFrom, request.DateTo, usage);
    }

    /// <inheritdoc/>
    public async Task<BookingStatisticsResponse> GetBookingStatisticsReportAsync
        (BookingStatisticsRequest request, CancellationToken cancellationToken = default)
    {
        var (start, endExclusive) = ToDateTimeRange(request.DateFrom, request.DateTo);

        var bookings = await bookingRepository.GetBookingsForPeriodAsync(
            start, endExclusive, roomId: null, cancellationToken);

        var totalBookings = bookings.Count;
        var cancelledBookings = bookings.Count(b => b.Status == BookingStatus.Cancelled);
        var cancellationRate = totalBookings > 0
            ? Math.Round((decimal)cancelledBookings / totalBookings * 100m, 2)
            : 0m;

        var averageDurationHours = totalBookings > 0
            ? Math.Round((decimal)bookings.Average(b => (b.EndTime - b.StartTime).TotalHours), 2)
            : 0m;

        var averageCheckAmount = totalBookings > 0
            ? Math.Round(bookings.Average(b => b.TotalCost), 2)
            : 0m;

        return new BookingStatisticsResponse(
            request.DateFrom,
            request.DateTo,
            totalBookings,
            cancelledBookings,
            cancellationRate,
            averageDurationHours,
            averageCheckAmount);
    }

    /// <inheritdoc/>
    public async Task<PricingPeriodReportResponse> GetPricingPeriodReportAsync
        (PricingPeriodReportRequest request, CancellationToken cancellationToken = default)
    {
        var (start, endExclusive) = ToDateTimeRange(request.DateFrom, request.DateTo);

        var bookings = (await bookingRepository.GetBookingsForPeriodAsync(
                start, endExclusive, roomId: null, cancellationToken))
            .Where(CountsTowardMetrics)
            .ToList();

        var grouped = bookings
            .GroupBy(b => GetPricingPeriodType(b.StartTime))
            .ToDictionary(g => g.Key, g => g.ToList());

        var periods = Enum.GetValues<PricingPeriodType>()
            .Select(periodType =>
            {
                grouped.TryGetValue(periodType, out var periodBookings);
                periodBookings ??= new List<Booking>();

                return new PricingPeriodBreakdownResponse(
                    PeriodType: periodType,
                    BookingsCount: periodBookings.Count,
                    Revenue: periodBookings.Sum(b => b.TotalCost));
            })
            .ToList();

        return new PricingPeriodReportResponse(request.DateFrom, request.DateTo, periods);
    }

    /// <summary>
    /// Converts an inclusive [DateFrom, DateTo] DateOnly range into the
    /// [periodStartInclusive, periodEndExclusive) DateTime range expected
    /// by IBookingRepository.GetBookingsForPeriodAsync.
    /// </summary>
    /// <param name="from">The start date of the range.</param>
    /// <param name="to">The end date of the range.</param>
    /// <returns>A tuple containing the start and endExclusive DateTime values.</returns>
    private static (DateTime start, DateTime endExclusive) ToDateTimeRange(DateOnly from, DateOnly to) =>
        (from.ToDateTime(TimeOnly.MinValue), to.AddDays(1).ToDateTime(TimeOnly.MinValue));

    /// <summary>
    /// Business rule: a cancelled booking never counts toward revenue,
    /// popularity, or room-usage metrics. Adjust here if that's wrong.
    /// </summary>
    /// <param name="booking">The booking to evaluate.</param>
    /// <returns>True if the booking counts toward metrics; otherwise, false.</returns>
    private static bool CountsTowardMetrics(Booking booking) =>
        booking.Status != BookingStatus.Cancelled;

    /// <summary>
    /// Buckets a start time into a period label for the requested granularity.
    /// </summary>
    /// <param name="startTime">The start time of the booking.</param>
    /// <param name="groupBy">The granularity to group by.</param>
    /// <returns>The period label.</returns>
    private static string GetPeriodLabel(DateTime startTime, ReportGroupBy groupBy) => groupBy switch
    {
        ReportGroupBy.Day => startTime.ToString("yyyy-MM-dd"),
        ReportGroupBy.Week => $"{startTime.Year}-W{ISOWeek.GetWeekOfYear(startTime):D2}",
        ReportGroupBy.Month => startTime.ToString("yyyy-MM"),
        _ => startTime.ToString("yyyy-MM-dd")
    };

    /// <summary>
    /// PLACEHOLDER pricing-period boundaries — the schema has no column that
    /// records which pricing period a booking fell into, so this is derived
    /// from the start hour. Replace with the real business rule if one
    /// already exists elsewhere (e.g. a PricingService).
    /// </summary>
    /// <param name="startTime">The start time of the booking.</param>
    /// <returns>The pricing period type.</returns>
    private static PricingPeriodType GetPricingPeriodType(DateTime startTime) => startTime.Hour switch
    {
        >= 6 and < 12 => PricingPeriodType.Morning,
        >= 12 and < 17 => PricingPeriodType.Standard,
        >= 17 and < 20 => PricingPeriodType.Peak,
        _ => PricingPeriodType.Evening // 20:00–23:59 and 00:00–05:59
    };

}