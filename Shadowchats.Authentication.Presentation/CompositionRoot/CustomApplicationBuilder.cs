// Shadowchats — Copyright (C) 2025
// Dorovskoy Alexey Vasilievich (One290 / 0ne290) <lenya.dorovskoy@mail.ru>
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version. See the LICENSE file for details.
// For full copyright and authorship information, see the COPYRIGHT file.

using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using OpenTelemetry.Trace;
using Serilog;
using Shadowchats.Authentication.Infrastructure.Persistence.AuthenticationDbContext;
using Shadowchats.Authentication.Presentation.CompositionRoot.Extensions;
using Shadowchats.Authentication.Presentation.GrpcInterceptors;
using Shadowchats.Authentication.Presentation.GrpcServices;

namespace Shadowchats.Authentication.Presentation.CompositionRoot;

public class CustomApplicationBuilder
{
    public static async Task<WebApplication?> Build(string[] args)
    {
        var builder = WebApplication.CreateBuilder();

        builder.WebHost.UseSetting("AllowedHosts", "*");
        builder.WebHost.ConfigureKestrel(options =>
        {
            options.ListenAnyIP(5000, listenOptions => { listenOptions.Protocols = HttpProtocols.Http1AndHttp2; });
        });

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

        builder.Services.AddHealthChecks()
            .AddCheck("health", () => HealthCheckResult.Healthy())
            .AddCheck<ReadyHealthCheck>("ready");

        var app = builder.Build();

        var lifetime = app.Services.GetRequiredService<IHostApplicationLifetime>();
        lifetime.ApplicationStopped.Register(Log.CloseAndFlush);

        if (args.Contains("migrate"))
        {
            using var scope = app.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AuthenticationDbContextReadWrite>();
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<CustomApplicationBuilder>>();
            await WaitForDatabaseAsync(db, logger);
            await db.Database.MigrateAsync();
            logger.LogInformation("Migrations applied successfully.");
            return null;
        }

        app.UseSerilogRequestLogging();

        app.UseGrpcWeb();

        app.MapHealthChecks("/health", new HealthCheckOptions
        {
            Predicate = check => check.Name == "health"
        });
        app.MapHealthChecks("/ready", new HealthCheckOptions
        {
            Predicate = check => check.Name == "ready"
        });

        app.MapGrpcService<AuthenticationGrpcService>().EnableGrpcWeb();

        return app;
    }

    private static async Task WaitForDatabaseAsync(AuthenticationDbContextReadWrite db,
        ILogger<CustomApplicationBuilder> logger)
    {
        const int maxRetries = 10;
        var delay = TimeSpan.FromSeconds(5);
        for (var i = 0; i < maxRetries; i++)
        {
            try
            {
                await db.Database.OpenConnectionAsync();
                await db.Database.CloseConnectionAsync();
                return;
            }
            catch
            {
                logger.LogCritical("Database not ready, retry {I}/{MaxRetries}", i + 1, maxRetries);
                await Task.Delay(delay);
            }
        }

        throw new Exception("Database is unreachable after multiple retries.");
    }
}