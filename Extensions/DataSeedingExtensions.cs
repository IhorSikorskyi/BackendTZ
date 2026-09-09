using BackendTZ.Data.Seed;

namespace BackendTZ.Extensions;

/// <summary>
/// Extension methods for registering and running database seeding.
/// </summary>
public static class DataSeedingExtensions
{
    /// <summary>
    /// Registers the data seeder in the DI container.
    /// </summary>
    public static IServiceCollection AddDataSeeding(this IServiceCollection services)
    {
        services.AddScoped<IDataSeeder, DataSeeder>();
        return services;
    }

    /// <summary>
    /// Runs the data seeder against the application's service provider.
    /// Intended to be called once during startup, in Development environments.
    /// </summary>
    public static async Task SeedDatabaseAsync(this IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var seeder = scope.ServiceProvider.GetRequiredService<IDataSeeder>();
        await seeder.SeedAsync();
    }
}