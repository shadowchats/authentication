using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace Shadowchats.Authentication.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        Log.Logger = new LoggerConfiguration()
            .Enrich.FromLogContext()
            .Enrich.WithExceptionDetails()
            .MinimumLevel.Information()
            .WriteTo.Async(c => c.Console())
            .CreateLogger();

        services.AddSerilog();
        
        services.AddHostedService<InfrastructureCleanupService>();

        return services;
    }
}