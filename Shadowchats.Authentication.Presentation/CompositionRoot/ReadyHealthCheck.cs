using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Shadowchats.Authentication.Infrastructure.Persistence;

namespace Shadowchats.Authentication.Presentation.CompositionRoot;

public class ReadyHealthCheck : IHealthCheck
{
    public ReadyHealthCheck(
        AuthenticationDbContext.ReadWrite dbRw,
        AuthenticationDbContext.ReadOnly dbRo,
        ILogger<ReadyHealthCheck> logger)
    {
        _dbRw = dbRw;
        _dbRo = dbRo;
        _logger = logger;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            await _dbRw.Accounts.AnyAsync(cancellationToken);
            _logger.LogInformation("ReadWrite database is reachable and operational.");

            // Проверяем RO
            await _dbRo.Accounts.AnyAsync(cancellationToken);
            _logger.LogInformation("ReadOnly database replica is reachable and operational.");

            return HealthCheckResult.Healthy(
                "All critical database dependencies (RW and RO) are reachable and operational."
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Database health check failed (RW or RO).");
            return HealthCheckResult.Unhealthy(
                "Critical database dependency failure (RW or RO).",
                ex
            );
        }
    }
    
    private readonly AuthenticationDbContext.ReadWrite _dbRw;
    private readonly AuthenticationDbContext.ReadOnly _dbRo;
    private readonly ILogger<ReadyHealthCheck> _logger;
}