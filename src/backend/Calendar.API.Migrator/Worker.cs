using System.Diagnostics;
using Microsoft.EntityFrameworkCore;

namespace Calendar.API.Migrator;

public class Worker<TContext> : BackgroundService
where TContext : DbContext
{
    private readonly IHostEnvironment _hostEnvironment;
    private readonly IHostApplicationLifetime _hostApplicationLifetime;
    private readonly IDbContextFactory<TContext> _contextFactory;
    private readonly Func<TContext, CancellationToken, Task> _seeder;
    private readonly ActivitySource _activitySource;
    
    public Worker(
        IHostEnvironment hostEnvironment,
        IHostApplicationLifetime hostApplicationLifetime,
        IDbContextFactory<TContext> contextFactory, 
        Func<TContext,CancellationToken, Task> seeder)
    {
        _hostEnvironment = hostEnvironment;
        _hostApplicationLifetime = hostApplicationLifetime;
        _contextFactory = contextFactory;
        _seeder = seeder;
        _activitySource = new ActivitySource(_hostEnvironment.ApplicationName);
    }
    
    public Worker(
        IHostEnvironment hostEnvironment,
        IHostApplicationLifetime hostApplicationLifetime,
        IDbContextFactory<TContext> contextFactory, 
        IDbSeeder<TContext>? seeder = null) : this(
        hostEnvironment,
        hostApplicationLifetime,
        contextFactory, 
        seeder != null ? 
            async (context, ct) => await seeder.SeedAsync(context, ct) 
            : (_, _) => Task.CompletedTask)
    {
    }
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await using var context = await _contextFactory.CreateDbContextAsync(stoppingToken);
            
        using var migrationActivity = _activitySource.StartActivity(_hostEnvironment.ApplicationName, ActivityKind.Client);

        try
        {
            var strategy = context.Database.CreateExecutionStrategy();
                
            await strategy.ExecuteAsync( async () =>
            {
                await context.Database.MigrateAsync(cancellationToken: stoppingToken);
                    
                await _seeder.Invoke(context, stoppingToken);
            });
        }
        catch (Exception ex)
        {
            migrationActivity?.AddException(ex);
            throw;
        }
        
        _hostApplicationLifetime.StopApplication();
    }
}