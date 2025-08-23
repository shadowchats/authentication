using Microsoft.Extensions.Hosting;
using Serilog;

namespace Shadowchats.Authentication.Infrastructure;

internal class InfrastructureCleanupService : IHostedService
{
    public InfrastructureCleanupService(IHostApplicationLifetime lifetime)
    {
        _lifetime = lifetime;
    }

    public Task StartAsync(CancellationToken _)
    {
        _lifetime.ApplicationStopping.Register(OnStopping);
        return Task.CompletedTask;
    }

    private static void OnStopping()
    {
        Log.CloseAndFlush();
    }

    public Task StopAsync(CancellationToken _)
    {
        return Task.CompletedTask;
    }
    
    private readonly IHostApplicationLifetime _lifetime;
}