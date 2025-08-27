// Shadowchats — Copyright (C) 2025
// Dorovskoy Alexey Vasilievich (One290 / 0ne290) <lenya.dorovskoy@mail.ru>
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version. See the LICENSE file for details.
// For full copyright and authorship information, see the COPYRIGHT file.

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