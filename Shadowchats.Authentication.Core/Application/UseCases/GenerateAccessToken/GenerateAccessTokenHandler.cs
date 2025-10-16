using Shadowchats.Authentication.Core.Application.Interfaces;
using Shadowchats.Authentication.Core.Domain.Aggregates;
using Shadowchats.Authentication.Core.Domain.Exceptions;
using Shadowchats.Authentication.Core.Domain.Interfaces;

namespace Shadowchats.Authentication.Core.Application.UseCases.GenerateAccessToken;

public class GenerateAccessTokenHandler : IMessageHandler<GenerateAccessTokenQuery, GenerateAccessTokenResult>
{
    public GenerateAccessTokenHandler(IAggregateRootRepository<Session> sessionRepository,
        IDateTimeProvider dateTimeProvider, IAccessTokenIssuer accessTokenIssuer)
    {
        _sessionRepository = sessionRepository;
        _dateTimeProvider = dateTimeProvider;
        _accessTokenIssuer = accessTokenIssuer;
    }

    public async Task<GenerateAccessTokenResult> Handle(GenerateAccessTokenQuery query)
    {
        var session = await _sessionRepository.Find(s => s.RefreshToken == query.RefreshToken);
        if (session is null)
            throw new InvariantViolationException("Refresh token is invalid.");

        return new GenerateAccessTokenResult
        {
            AccessToken = session.GenerateAccessToken(_accessTokenIssuer, _dateTimeProvider)
        };
    }

    private readonly IAggregateRootRepository<Session> _sessionRepository;

    private readonly IDateTimeProvider _dateTimeProvider;

    private readonly IAccessTokenIssuer _accessTokenIssuer;
}