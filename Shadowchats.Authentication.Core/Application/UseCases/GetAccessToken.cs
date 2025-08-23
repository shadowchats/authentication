using Newtonsoft.Json;
using Shadowchats.Authentication.Core.Application.Interfaces;
using Shadowchats.Authentication.Core.Domain.Exceptions;
using Shadowchats.Authentication.Core.Domain.Interfaces;

namespace Shadowchats.Authentication.Core.Application.UseCases
{
    namespace GetAccessToken
    {
        public class GetAccessTokenCommand : ICommand<GetAccessTokenResult>
        {
            [JsonProperty(Required = Required.Always, PropertyName = "refreshToken")]
            public required string RefreshToken { get; init; }
        }
        
        public class GetAccessTokenResult
        {
            [JsonProperty(Required = Required.Always, PropertyName = "accessToken")]
            public required string AccessToken { get; init; }
        }
        
        internal class GetAccessTokenHandler : ICommandHandler<GetAccessTokenCommand, GetAccessTokenResult>
        {
            public GetAccessTokenHandler(ISessionRepository sessionRepository, IDateTimeProvider dateTimeProvider, IAccessTokenIssuer accessTokenIssuer)
            {
                _sessionRepository = sessionRepository;
                _dateTimeProvider = dateTimeProvider;
                _accessTokenIssuer = accessTokenIssuer;
            }

            public async Task<GetAccessTokenResult> Handle(GetAccessTokenCommand command)
            {
                var session = await _sessionRepository.GetByRefreshToken(command.RefreshToken);
                if (session is null)
                    throw new InvariantViolationException("Refresh token is invalid.");

                return new GetAccessTokenResult
                {
                    AccessToken = session.GenerateAccessToken(_accessTokenIssuer, _dateTimeProvider)
                };
            }

            private readonly ISessionRepository _sessionRepository;

            private readonly IDateTimeProvider _dateTimeProvider;
            
            private readonly IAccessTokenIssuer _accessTokenIssuer;
        }
    }
}