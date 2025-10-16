using Shadowchats.Authentication.Core.Application.Base;
using Shadowchats.Authentication.Core.Application.Interfaces;
using Shadowchats.Authentication.Core.Domain.Aggregates;
using Shadowchats.Authentication.Core.Domain.Interfaces;

namespace Shadowchats.Authentication.Core.Application.Jobs.RevokeExpiredSessions;

public class RevokeExpiredSessionsHandler : IMessageHandler<RevokeExpiredSessionsJob, NoResult>
{
    public RevokeExpiredSessionsHandler(IAggregateRootRepository<Session> sessionRepository,
        IPersistenceContext persistenceContext, IDateTimeProvider dateTimeProvider)
    {
        _sessionRepository = sessionRepository;
        _persistenceContext = persistenceContext;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<NoResult> Handle(RevokeExpiredSessionsJob _)
    {
        var sessions =
            await _sessionRepository.FindAll(s => s.IsActive && s.ExpiresAt < _dateTimeProvider.UtcNow);
        if (sessions.Count > 0)
        {
            foreach (var session in sessions)
                session.Revoke();

            await _persistenceContext.SaveChanges();
        }

        return NoResult.Value;
    }

    private readonly IAggregateRootRepository<Session> _sessionRepository;

    private readonly IPersistenceContext _persistenceContext;

    private readonly IDateTimeProvider _dateTimeProvider;
}