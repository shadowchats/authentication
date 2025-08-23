using Serilog.Core;
using Serilog.Events;
using Shadowchats.Authentication.Infrastructure.Extensions;

namespace Shadowchats.Authentication.Infrastructure.Logging.Enrichers;

internal class ExceptionDetailsEnricher : ILogEventEnricher
{
    public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
    {
        if (logEvent.Exception == null)
            return;

        logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("ExceptionLocation",
            logEvent.Exception.GetLocation()));
        logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("ExceptionType",
            logEvent.Exception.GetType().Name));
        logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("ExceptionMessage",
            logEvent.Exception.Message));
    }
}