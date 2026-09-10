using BackendTZ.Repositories.Implementations;
using BackendTZ.Repositories.Interfaces;

namespace BackendTZ.Extensions;

public static class RepositoryServiceExtensions
{
    /// <summary>
    /// Adds repository services to the service collection.
    /// </summary>
    /// <param name="services">The service collection to which the repository services will be added.</param>
    /// <returns>The updated service collection.</returns>
    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IServiceRepository, ServiceRepository>();
        services.AddScoped<IRoomRepository, RoomRepository>();
        services.AddScoped<IBookingRepository, BookingRepository>();
        services.AddScoped<IBookingServiceRepository, BookingServiceRepository>();
        services.AddScoped<IRoomServiceRepository, RoomServiceRepository>();
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
        return services;
    }
}