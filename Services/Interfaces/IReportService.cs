using BackendTZ.DTOs.Requests;
using BackendTZ.DTOs.Responses;

namespace BackendTZ.Services.Interfaces;

/// <summary>
/// Defines the contract for a report service that provides various reporting functionalities such as revenue reports, room utilization reports, popular rooms and services reports, booking statistics, and pricing period reports.
/// </summary>
public interface IReportService
{
    /// <summary>
    /// Gets the revenue report based on the provided request parameters.
    /// </summary>
    /// <param name="request">The request parameters for the revenue report.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The revenue report response.</returns>
    Task<RevenueReportResponse> GetRevenueReportAsync
        (RevenueReportRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the room utilization report based on the provided request parameters.
    /// </summary>
    /// <param name="request">The request parameters for the room utilization report.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The room utilization report response.</returns>
    Task<RoomUtilizationReportResponse> GetRoomUtilizationReportAsync
        (RoomUtilizationReportRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the popular rooms report based on the provided request parameters.
    /// </summary>
    /// <param name="request">The request parameters for the popular rooms report.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The popular rooms report response.</returns>
    Task<PopularRoomsReportResponse> GetPopularRoomsReportAsync
        (PopularRoomsReportRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the popular services report based on the provided request parameters.
    /// </summary>
    /// <param name="request">The request parameters for the popular services report.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The popular services report response.</returns>
    Task<PopularServicesReportResponse> GetPopularServicesReportAsync
        (PopularServicesReportRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the booking statistics report based on the provided request parameters.
    /// </summary>
    /// <param name="request">The request parameters for the booking statistics report.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The booking statistics report response.</returns>
    Task<BookingStatisticsResponse> GetBookingStatisticsReportAsync
        (BookingStatisticsRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the pricing period report based on the provided request parameters.
    /// </summary>
    /// <param name="request">The request parameters for the pricing period report.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The pricing period report response.</returns>
    Task<PricingPeriodReportResponse> GetPricingPeriodReportAsync
        (PricingPeriodReportRequest request, CancellationToken cancellationToken = default);
}