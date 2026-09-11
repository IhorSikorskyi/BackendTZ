using BackendTZ.DTOs.Requests;
using BackendTZ.DTOs.Responses;
using BackendTZ.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BackendTZ.Controllers;

/// <summary>
/// Controller responsible for handling report-related operations such as generating revenue reports, room utilization reports, popular rooms and services reports, booking statistics, and pricing period reports.
/// </summary>
/// <param name="reportService">The report service used to generate various reports.</param>
[Authorize(Roles = "Admin")]
[ApiController]
[Route("api/reports")]
public class ReportController(IReportService reportService) : BaseController
{
    /// <summary>
    /// Generates a revenue report based on the provided request parameters.
    /// </summary>
    /// <param name="request">The request containing the parameters for generating the revenue report.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The revenue report response containing the generated report data.</returns>
    [HttpPost("revenue")]
    [ProducesResponseType(typeof(RevenueReportResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<RevenueReportResponse>> GetRevenueReport
        (RevenueReportRequest request, CancellationToken cancellationToken)
    {
        var response = await reportService.GetRevenueReportAsync(request, cancellationToken);
        return Ok(response);
    }

    /// <summary>
    /// Generates a room utilization report based on the provided request parameters.
    /// </summary>
    /// <param name="request">The request containing the parameters for generating the room utilization report.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The room utilization report response containing the generated report data.</returns>
    [HttpPost("room-utilization")]
    [ProducesResponseType(typeof(RoomUtilizationReportResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<RoomUtilizationReportResponse>> GetRoomUtilizationReport
        (RoomUtilizationReportRequest request, CancellationToken cancellationToken)
    {
        var response = await reportService.GetRoomUtilizationReportAsync(request, cancellationToken);
        return Ok(response);
    }

    /// <summary>
    /// Generates a popular rooms report based on the provided request parameters.
    /// </summary>
    /// <param name="request">The request containing the parameters for generating the popular rooms report.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The popular rooms report response containing the generated report data.</returns>
    [HttpPost("popular-rooms")]
    [ProducesResponseType(typeof(PopularRoomsReportResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PopularRoomsReportResponse>> GetPopularRoomsReport
        (PopularRoomsReportRequest request, CancellationToken cancellationToken)
    {
        var response = await reportService.GetPopularRoomsReportAsync(request, cancellationToken);
        return Ok(response);
    }

    /// <summary>
    /// Generates a popular services report based on the provided request parameters.
    /// </summary>
    /// <param name="request">The request containing the parameters for generating the popular services report.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The popular services report response containing the generated report data.</returns>
    [HttpPost("popular-services")]
    [ProducesResponseType(typeof(PopularServicesReportResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PopularServicesReportResponse>> GetPopularServicesReport
        (PopularServicesReportRequest request, CancellationToken cancellationToken)
    {
        var response = await reportService.GetPopularServicesReportAsync(request, cancellationToken);
        return Ok(response);
    }

    /// <summary>
    /// Generates a booking statistics report based on the provided request parameters.
    /// </summary>
    /// <param name="request">The request containing the parameters for generating the booking statistics report.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The booking statistics report response containing the generated report data.</returns>
    [HttpPost("booking-statistics")]
    [ProducesResponseType(typeof(BookingStatisticsResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<BookingStatisticsResponse>> GetBookingStatisticsReport
        (BookingStatisticsRequest request, CancellationToken cancellationToken)
    {
        var response = await reportService.GetBookingStatisticsReportAsync(request, cancellationToken);
        return Ok(response);
    }

    /// <summary>
    /// Generates a pricing period report based on the provided request parameters.
    /// </summary>
    /// <param name="request">The request containing the parameters for generating the pricing period report.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The pricing period report response containing the generated report data.</returns>
    [HttpPost("pricing-period")]
    [ProducesResponseType(typeof(PricingPeriodReportResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PricingPeriodReportResponse>> GetPricingPeriodReport
        (PricingPeriodReportRequest request, CancellationToken cancellationToken)
    {
        var response = await reportService.GetPricingPeriodReportAsync(request, cancellationToken);
        return Ok(response);
    }
}