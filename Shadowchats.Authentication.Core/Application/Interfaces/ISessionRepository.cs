using Shadowchats.Authentication.Core.Domain.Aggregates;

namespace Shadowchats.Authentication.Core.Application.Interfaces;

public interface ISessionRepository
{
    Task<Session?> GetByRefreshToken(string refreshToken);
    Task Add(Session session);
}