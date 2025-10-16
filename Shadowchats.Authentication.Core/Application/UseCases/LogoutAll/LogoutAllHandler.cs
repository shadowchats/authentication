using Shadowchats.Authentication.Core.Application.Base;
using Shadowchats.Authentication.Core.Application.Interfaces;
using Shadowchats.Authentication.Core.Domain.Aggregates;
using Shadowchats.Authentication.Core.Domain.Exceptions;
using Shadowchats.Authentication.Core.Domain.Interfaces;

namespace Shadowchats.Authentication.Core.Application.UseCases.LogoutAll;

public class LogoutAllHandler : IMessageHandler<LogoutAllCommand, NoResult>
{
    public LogoutAllHandler(IAggregateRootRepository<Account> accountRepository,
        IAggregateRootRepository<Session> sessionRepository, IPersistenceContext persistenceContext,
        IPasswordHasher passwordHasher)
    {
        _accountRepository = accountRepository;
        _sessionRepository = sessionRepository;
        _persistenceContext = persistenceContext;
        _passwordHasher = passwordHasher;
    }

    public async Task<NoResult> Handle(LogoutAllCommand command)
    {
        var account = await _accountRepository.Find(a => a.Credentials.Login == command.Login);
        if (account is null || !account.Credentials.VerifyPassword(_passwordHasher, command.Password))
            throw new AuthenticationFailedException("Login and/or password is invalid.");

        var sessions = await _sessionRepository.FindAll(s => s.AccountId == account.Guid && s.IsActive);
        if (sessions.Count > 0)
        {
            foreach (var session in sessions)
                session.Revoke();

            await _persistenceContext.SaveChanges();
        }

        return NoResult.Value;
    }

    private readonly IAggregateRootRepository<Account> _accountRepository;

    private readonly IAggregateRootRepository<Session> _sessionRepository;

    private readonly IPersistenceContext _persistenceContext;

    private readonly IPasswordHasher _passwordHasher;
}