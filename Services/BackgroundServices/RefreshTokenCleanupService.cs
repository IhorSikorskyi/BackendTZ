using BackendTZ.Repositories.Interfaces;

namespace BackendTZ.Services.BackgroundServices;

/// <summary>
/// Represents a background service that periodically cleans up old refresh tokens from the database.
/// </summary>
/// <param name="logger">The logger used to log information and errors.</param>
/// <param name="serviceScopeFactory">The factory used to create service scopes.</param>
/// <param name="configuration">The application configuration.</param>
public class RefreshTokenCleanupService(
    ILogger<RefreshTokenCleanupService> logger,
    IServiceScopeFactory serviceScopeFactory,
    IConfiguration configuration
    ) : BackgroundService
{
    /// <summary>
    /// Executes the background service to clean up old refresh tokens from the database.
    /// </summary>
    /// <param name="stoppingToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("RefreshTokenCleanupService is starting.");

        await RunCleanupAsync(stoppingToken);

        var hours = configuration.GetValue("AppSettings:IntervalHoursForCleanup", defaultValue: 24);
        if (hours <= 0)
        {
            logger.LogWarning(
                "Invalid cleanup interval ({Hours}h) configured, falling back to 24h.", hours);
            hours = 24;
        }
        using var timer = new PeriodicTimer(TimeSpan.FromHours(hours));

        while (await timer.WaitForNextTickAsync(stoppingToken))
        {
            await RunCleanupAsync(stoppingToken);
        }
    }

    /// <summary>
    /// Runs the cleanup operation to remove old refresh tokens from the database.
    /// </summary>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    internal async Task RunCleanupAsync(CancellationToken cancellationToken)
    {
        try
        {
            await using var scope = serviceScopeFactory.CreateAsyncScope();
            var repo = scope.ServiceProvider.GetRequiredService<IRefreshTokenRepository>();
            var unit = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

            await repo.RemoveOldTokensAsync(cancellationToken);
            await unit.SaveChangesAsync(cancellationToken);
            logger.LogInformation("Old refresh tokens removed.");
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            // Expected during shutdown — rethrow without re-logging, let the host handle it.
            throw;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to clean refresh tokens.");
        }
    }
}