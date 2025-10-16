using Shadowchats.Authentication.Core.Application.Base;
using Shadowchats.Authentication.Core.Application.Interfaces;
using Shadowchats.Authentication.Core.Domain.Aggregates;

namespace Shadowchats.Authentication.Core.Application.UseCases.Logout;

public class LogoutHandler : IMessageHandler<LogoutCommand, NoResult>
{
    public LogoutHandler(IAggregateRootRepository<Session> sessionRepository, IPersistenceContext persistenceContext)
    {
        _sessionRepository = sessionRepository;
        _persistenceContext = persistenceContext;
    }

    public async Task<NoResult> Handle(LogoutCommand command)
    {
        var session = await _sessionRepository.Find(s => s.RefreshToken == command.RefreshToken);
        if (session is not null)
        {
            session.Revoke();
            await _persistenceContext.SaveChanges();
        }

        return NoResult.Value;
    }

    private readonly IAggregateRootRepository<Session> _sessionRepository;

    private readonly IPersistenceContext _persistenceContext;
}