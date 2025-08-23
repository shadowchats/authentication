using Shadowchats.Authentication.Core.Domain.Interfaces;

namespace Shadowchats.Authentication.Infrastructure.System;

internal class DateTimeProvider : IDateTimeProvider
{
    public DateTime UtcNow => DateTime.UtcNow;
}