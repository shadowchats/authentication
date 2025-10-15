using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Shadowchats.Authentication.Infrastructure.Persistence.AuthenticationDbContext;

namespace Shadowchats.Authentication.Presentation.CompositionRoot;

public class ReadyHealthCheck : IHealthCheck
{
    public ReadyHealthCheck(
        ReadWrite dbRw,
        ReadOnly dbRo,
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
        // Checking relational databases
        try
        {
            await _dbRw.Accounts.AnyAsync(cancellationToken);
            _logger.LogInformation("ReadWrite database is reachable and operational.");

            // Проверяем RO
            await _dbRo.Accounts.AnyAsync(cancellationToken);
            _logger.LogInformation("ReadOnly database replica is reachable and operational.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Database health check failed (RW or RO).");
            return HealthCheckResult.Unhealthy(
                "Critical database dependency failure (RW or RO).",
                ex
            );
        }

        return HealthCheckResult.Healthy(
            "All critical dependencies are reachable and operational."
        );
    }
    
    private readonly ReadWrite _dbRw;
    private readonly ReadOnly _dbRo;
    private readonly ILogger<ReadyHealthCheck> _logger;
}
