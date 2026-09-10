using BackendTZ.Repositories.Interfaces;

namespace BackendTZ.Services.Implementations;

public class RefreshTokenCleanupService(
    ILogger<RefreshTokenCleanupService> logger,
    IServiceScopeFactory serviceScopeFactory,
    IConfiguration configuration
    ) : BackgroundService
{
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