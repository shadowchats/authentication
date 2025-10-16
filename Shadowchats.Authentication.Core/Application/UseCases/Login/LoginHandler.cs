using Shadowchats.Authentication.Core.Application.Interfaces;
using Shadowchats.Authentication.Core.Domain.Aggregates;
using Shadowchats.Authentication.Core.Domain.Exceptions;
using Shadowchats.Authentication.Core.Domain.Interfaces;

namespace Shadowchats.Authentication.Core.Application.UseCases.Login;

public class LoginHandler : IMessageHandler<LoginCommand, LoginResult>
{
    public LoginHandler(IAggregateRootRepository<Account> accountRepository,
        IAggregateRootRepository<Session> sessionRepository, IPersistenceContext persistenceContext,
        IPasswordHasher passwordHasher, IGuidGenerator guidGenerator, IDateTimeProvider dateTimeProvider,
        IRefreshTokenGenerator refreshTokenGenerator, IAccessTokenIssuer accessTokenIssuer)
    {
        _accountRepository = accountRepository;
        _sessionRepository = sessionRepository;
        _persistenceContext = persistenceContext;
        _passwordHasher = passwordHasher;
        _guidGenerator = guidGenerator;
        _dateTimeProvider = dateTimeProvider;
        _refreshTokenGenerator = refreshTokenGenerator;
        _accessTokenIssuer = accessTokenIssuer;
    }

    public async Task<LoginResult> Handle(LoginCommand command)
    {
        var account = await _accountRepository.Find(a => a.Credentials.Login == command.Login);
        if (account is null || !account.Credentials.VerifyPassword(_passwordHasher, command.Password))
            throw new AuthenticationFailedException("Login and/or password is invalid.");

        var session = Session.Create(_guidGenerator, _dateTimeProvider, _refreshTokenGenerator, account.Guid);
        await _sessionRepository.Add(session);
        await _persistenceContext.SaveChanges();

        return new LoginResult
        {
            RefreshToken = session.RefreshToken,
            AccessToken = session.GenerateAccessToken(_accessTokenIssuer, _dateTimeProvider)
        };
    }

    private readonly IAggregateRootRepository<Account> _accountRepository;

    private readonly IAggregateRootRepository<Session> _sessionRepository;

    private readonly IPersistenceContext _persistenceContext;

    private readonly IPasswordHasher _passwordHasher;

    private readonly IGuidGenerator _guidGenerator;

    private readonly IDateTimeProvider _dateTimeProvider;

    private readonly IRefreshTokenGenerator _refreshTokenGenerator;

    private readonly IAccessTokenIssuer _accessTokenIssuer;
}