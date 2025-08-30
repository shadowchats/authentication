using OpenTelemetry.Trace;
using Serilog;
using Shadowchats.Authentication.Core.Domain.Exceptions;
using Shadowchats.Authentication.Infrastructure.Persistence;
using Shadowchats.Authentication.Presentation.CompositionRoot.Extensions;
using Shadowchats.Authentication.Presentation.GrpcInterceptors;
using Shadowchats.Authentication.Presentation.GrpcServices;

namespace Shadowchats.Authentication.Presentation.CompositionRoot;

public static class CustomApplicationBuilder
{
    public static WebApplication Build()
    {
        var builder = WebApplication.CreateBuilder();

        builder.Services.AddGrpc(options => { options.Interceptors.Add<ExceptionHandlingGrpcInterceptor>(); });
        builder.Services
            .AddApplication()
            .AddInfrastructure(builder.Configuration)
            .AddCustomLogging()
            .AddOpenTelemetry().WithTracing(tracerProviderBuilder =>
            {
                tracerProviderBuilder.AddAspNetCoreInstrumentation(options => { options.RecordException = true; })
                    .AddGrpcClientInstrumentation().AddHttpClientInstrumentation();
            });

        builder.Host.UseSerilog();

        var app = builder.Build();

        using var scope = app.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AuthenticationDbContext>();
        if (!db.Database.CanConnect())
            throw new BugException("Database unavailable.");

        var lifetime = app.Services.GetRequiredService<IHostApplicationLifetime>();
        lifetime.ApplicationStopped.Register(() => { Log.CloseAndFlush(); });

        app.UseSerilogRequestLogging();

        app.MapGrpcService<AuthenticationGrpcService>();

        return app;
    }
}