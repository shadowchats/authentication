namespace Shadowchats.Authentication.Core.Domain.Interfaces;

internal interface IDateTimeProvider
{
    DateTime UtcNow { get; }
}