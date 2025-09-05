// Shadowchats — Copyright (C) 2025
// Dorovskoy Alexey Vasilievich (One290 / 0ne290) <lenya.dorovskoy@mail.ru>
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version. See the LICENSE file for details.
// For full copyright and authorship information, see the COPYRIGHT file.

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Shadowchats.Authentication.Core.Application.Base;
using Shadowchats.Authentication.Core.Application.Interfaces;
using Shadowchats.Authentication.Core.Application.Jobs.RevokeExpiredSessions;

namespace Shadowchats.Authentication.Infrastructure.Scheduling;

public class RevokeExpiredSessionsScheduler(
    IServiceScopeFactory scopeFactory,
    ILogger<RevokeExpiredSessionsScheduler> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                logger.LogInformation("Job {JobName} started", JobName);

                using var scope = scopeFactory.CreateScope();
                var bus = scope.ServiceProvider.GetRequiredService<IBus>();
                await bus.Execute<RevokeExpiredSessionsJob, NoResult>(Job);

                logger.LogInformation("Job {JobName} finished successfully", JobName);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Job {JobName} failed", JobName);
            }
            try
            {
                await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
            }
            catch (TaskCanceledException)
            {
                // Graceful shutdown
            }
        }
    }

    private const string JobName = nameof(RevokeExpiredSessionsJob);

    private static readonly RevokeExpiredSessionsJob Job = new();
}