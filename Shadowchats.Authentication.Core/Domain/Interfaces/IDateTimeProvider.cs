namespace Shadowchats.Authentication.Core.Domain.Interfaces;

public interface IDateTimeProvider
{
    DateTime UtcNow { get; }
}