using System.ComponentModel.DataAnnotations;

namespace BackendTZ.DTOs.Requests;

/// <summary>
/// Base class for all report/analytics related requests.
/// </summary>
public abstract record ReportRequest;

/// <summary>
/// Request payload for querying report data over a period, optionally scoped to a single room.
/// </summary>
/// <param name="From">The start date of the reporting period (inclusive).</param>
/// <param name="To">The end date of the reporting period (inclusive).</param>
/// <param name="RoomId">Optional room ID to scope the report to a single room.</param>
public record ReportPeriodRequest(
    [property: Required]
    DateOnly From,

    [property: Required]
    DateOnly To,

    Guid? RoomId
) : ReportRequest
{
    public bool IsValidPeriod() => To >= From;
}