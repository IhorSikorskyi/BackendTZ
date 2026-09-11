using BackendTZ.Entities;
using BackendTZ.Services.Implementations;
using BackendTZ.Services.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace BackendTZ.Extensions;

/// <summary>
/// Provides extension methods for adding application services to the service collection.
/// </summary>
public static class ApplicationServiceExtensions
{
    /// <summary>
    /// Adds application services to the service collection.
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IBookingManagementService, BookingManagementService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
        services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
        services.AddScoped<IRefreshTokenService, RefreshTokenService>();
        services.AddScoped<IRoomManagementService, RoomManagementService>();
        services.AddScoped<IServiceManagementService, ServiceManagementService>();
        services.AddScoped<IPricingService, PricingService>();
        services.AddScoped<IReportService, ReportService>();
        return services;
    }
}