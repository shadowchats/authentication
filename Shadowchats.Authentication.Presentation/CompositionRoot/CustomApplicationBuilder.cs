// Shadowchats — Copyright (C) 2025
// Dorovskoy Alexey Vasilievich (One290 / 0ne290) <lenya.dorovskoy@mail.ru>
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version. See the LICENSE file for details.
// For full copyright and authorship information, see the COPYRIGHT file.

using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using OpenTelemetry.Trace;
using Serilog;
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
        
        builder.WebHost.UseSetting("AllowedHosts", "*");
        builder.WebHost.ConfigureKestrel(options =>
        {
            options.ListenAnyIP(5000, listenOptions =>
            {
                listenOptions.Protocols = HttpProtocols.Http1AndHttp2;
            });
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
        
        var app = builder.Build();

        using var scope = app.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AuthenticationDbContext>();
        db.Database.OpenConnection();

        var lifetime = app.Services.GetRequiredService<IHostApplicationLifetime>();
        lifetime.ApplicationStopped.Register(Log.CloseAndFlush);

        app.UseSerilogRequestLogging();

        app.MapGrpcService<AuthenticationGrpcService>();

        return app;
    }
}