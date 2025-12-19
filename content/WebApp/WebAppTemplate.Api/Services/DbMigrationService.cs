using Microsoft.EntityFrameworkCore;
using WebAppTemplate.Api.Database;

namespace WebAppTemplate.Api.Services;

public class DbMigrationService : IHostedLifecycleService
{
    private readonly ILogger<DbMigrationService> Logger;
    private readonly IServiceProvider ServiceProvider;

    public DbMigrationService(ILogger<DbMigrationService> logger, IServiceProvider serviceProvider)
    {
        Logger = logger;
        ServiceProvider = serviceProvider;
    }

    public async Task StartingAsync(CancellationToken cancellationToken)
    {
        Logger.LogTrace("Checking for pending migrations");

        await using var scope = ServiceProvider.CreateAsyncScope();
        var context = scope.ServiceProvider.GetRequiredService<DataContext>();

        var pendingMigrations = await context.Database.GetPendingMigrationsAsync(cancellationToken);
        var migrationNames = pendingMigrations.ToArray();

        if (migrationNames.Length == 0)
        {
            Logger.LogDebug("No pending migrations found");
            return;
        }
        
        Logger.LogInformation("Pending migrations: {names}", string.Join(", ", migrationNames));
        Logger.LogInformation("Migration started");

        await context.Database.MigrateAsync(cancellationToken);
        
        Logger.LogInformation("Migration complete");
    }
    
    public Task StartAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    public Task StartedAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    public Task StoppedAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    public Task StoppingAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}