using Serilog;
using Serilog.Configuration;
using Shadowchats.Authentication.Infrastructure.Logging.Enrichers;

namespace Shadowchats.Authentication.Infrastructure.Extensions;

internal static class LoggerEnrichmentConfigurationExtensions
{
    internal static LoggerConfiguration WithExceptionDetails(this LoggerEnrichmentConfiguration enrich) => enrich.With<ExceptionDetailsEnricher>();
}