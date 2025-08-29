using OpenTelemetry.Trace;
using Serilog;
using Shadowchats.Authentication.Presentation.CompositionRoot.Extensions;
using Shadowchats.Authentication.Presentation.GrpcInterceptors;
using Shadowchats.Authentication.Presentation.GrpcServices;

namespace Shadowchats.Authentication.Presentation.CompositionRoot;

public static class CustomApplicationBuilder
{
    public static WebApplication Build()
    {
        var builder = WebApplication.CreateBuilder();

        builder.Services.AddGrpc(options =>
        {
            options.Interceptors.Add<ExceptionHandlingGrpcInterceptor>();
        });
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
        
        var lifetime = app.Services.GetRequiredService<IHostApplicationLifetime>();
        lifetime.ApplicationStopped.Register(() =>
        {
            Log.CloseAndFlush();
        });

        app.UseSerilogRequestLogging();

        app.MapGrpcService<AuthenticationGrpcService>();

        return app;
    }
}