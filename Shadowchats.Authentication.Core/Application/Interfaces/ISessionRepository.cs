using Shadowchats.Authentication.Core.Domain.Aggregates;

namespace Shadowchats.Authentication.Core.Application.Interfaces;

internal interface ISessionRepository
{
    Task<Session?> GetByRefreshToken(string refreshToken);
    Task Add(Session session);
}