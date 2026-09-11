using BackendTZ.DTOs.Responses;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace BackendTZ.DTOs.Requests;

/// <summary>
/// Base class for all report related requests.
/// </summary>
public abstract record ReportRequest(
    [property: JsonRequired, Required] DateOnly DateFrom,
    [property: JsonRequired, Required] DateOnly DateTo
)
{
    /// <summary>
    /// Checks if the date range specified in the request is valid (i.e., DateTo is not earlier than DateFrom).
    /// </summary>
    /// <returns>True if the date range is valid; otherwise, false.</returns>
    public bool IsValidDateRange() => DateTo >= DateFrom;
}

/// <summary>
/// Request payload for the revenue report, optionally grouped by period and/or filtered by room.
/// </summary>
/// <param name="DateFrom">Start of the reporting period (inclusive).</param>
/// <param name="DateTo">End of the reporting period (inclusive).</param>
/// <param name="GroupBy">The granularity used to bucket revenue over time.</param>
/// <param name="RoomId">Optional room filter; when null, all rooms are included.</param>
/// <param name="IncludeInactiveRooms">Whether to include rooms that were later deactivated/deleted.</param>
public record RevenueReportRequest(
    DateOnly DateFrom,
    DateOnly DateTo,
    [property: JsonRequired]
    ReportGroupBy GroupBy,
    Guid? RoomId,
    bool IncludeInactiveRooms = true
) : ReportRequest(DateFrom, DateTo);

/// <summary>
/// Request payload for the room utilization report.
/// </summary>
/// <param name="DateFrom">Start of the reporting period (inclusive).</param>
/// <param name="DateTo">End of the reporting period (inclusive).</param>
/// <param name="RoomId">Optional room filter; when null, all rooms are included.</param>
/// <param name="IncludeInactiveRooms">Whether to include rooms that were later deactivated/deleted.</param>
public record RoomUtilizationReportRequest(
    DateOnly DateFrom,
    DateOnly DateTo,
    Guid? RoomId,
    bool IncludeInactiveRooms = false
) : ReportRequest(DateFrom, DateTo);

/// <summary>
/// Request payload for the popular rooms report.
/// </summary>
/// <param name="DateFrom">Start of the reporting period (inclusive).</param>
/// <param name="DateTo">End of the reporting period (inclusive).</param>
/// <param name="TopN">Maximum number of rooms to include in each ranking.</param>
public record PopularRoomsReportRequest(
    DateOnly DateFrom,
    DateOnly DateTo,
    [property: Range(1, 100)] int TopN
) : ReportRequest(DateFrom, DateTo);

/// <summary>
/// Request payload for the popular services report.
/// </summary>
/// <param name="DateFrom">Start of the reporting period (inclusive).</param>
/// <param name="DateTo">End of the reporting period (inclusive).</param>
public record PopularServicesReportRequest(
    DateOnly DateFrom,
    DateOnly DateTo
) : ReportRequest(DateFrom, DateTo);

/// <summary>
/// Request payload for aggregate booking statistics.
/// </summary>
/// <param name="DateFrom">Start of the reporting period (inclusive).</param>
/// <param name="DateTo">End of the reporting period (inclusive).</param>
public record BookingStatisticsRequest(
    DateOnly DateFrom,
    DateOnly DateTo
) : ReportRequest(DateFrom, DateTo);

/// <summary>
/// Request payload for the pricing period breakdown report.
/// </summary>
/// <param name="DateFrom">Start of the reporting period (inclusive).</param>
/// <param name="DateTo">End of the reporting period (inclusive).</param>
public record PricingPeriodReportRequest(
    DateOnly DateFrom,
    DateOnly DateTo
) : ReportRequest(DateFrom, DateTo);

/// <summary>
/// Request payload for the top clients report.
/// </summary>
/// <param name="DateFrom">Start of the reporting period (inclusive).</param>
/// <param name="DateTo">End of the reporting period (inclusive).</param>
/// <param name="TopN">Maximum number of clients to include in the ranking.</param>
public record TopClientsReportRequest(
    DateOnly DateFrom,
    DateOnly DateTo,
    [property: Range(1, 100)] int TopN
) : ReportRequest(DateFrom, DateTo);