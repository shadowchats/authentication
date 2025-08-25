using Newtonsoft.Json;
using Shadowchats.Authentication.Core.Application.Interfaces;
using Shadowchats.Authentication.Core.Domain.Aggregates;
using Shadowchats.Authentication.Core.Domain.Exceptions;
using Shadowchats.Authentication.Core.Domain.Interfaces;

namespace Shadowchats.Authentication.Core.Application.UseCases
{
    namespace GenerateAccessTokenToken
    {
        public class GenerateAccessTokenCommand : ICommand<GenerateAccessTokenResult>
        {
            [JsonProperty(Required = Required.Always, PropertyName = "refreshToken")]
            public required string RefreshToken { get; init; }
        }
        
        public class GenerateAccessTokenResult
        {
            [JsonProperty(Required = Required.Always, PropertyName = "accessToken")]
            public required string AccessToken { get; init; }
        }
        
        internal class GenerateAccessTokenHandler : ICommandHandler<GenerateAccessTokenCommand, GenerateAccessTokenResult>
        {
            public GenerateAccessTokenHandler(IAggregateRootsRepository aggregateRootsRepository, IDateTimeProvider dateTimeProvider, IAccessTokenIssuer accessTokenIssuer)
            {
                _aggregateRootsRepository = aggregateRootsRepository;
                _dateTimeProvider = dateTimeProvider;
                _accessTokenIssuer = accessTokenIssuer;
            }

            public async Task<GenerateAccessTokenResult> Handle(GenerateAccessTokenCommand command)
            {
                var session =
                    await _aggregateRootsRepository.Find<Session>(s => s.RefreshToken == command.RefreshToken);
                if (session is null)
                    throw new InvariantViolationException("Refresh token is invalid.");

                return new GenerateAccessTokenResult
                {
                    AccessToken = session.GenerateAccessToken(_accessTokenIssuer, _dateTimeProvider)
                };
            }

            private readonly IAggregateRootsRepository _aggregateRootsRepository;

            private readonly IDateTimeProvider _dateTimeProvider;
            
            private readonly IAccessTokenIssuer _accessTokenIssuer;
        }
    }
}