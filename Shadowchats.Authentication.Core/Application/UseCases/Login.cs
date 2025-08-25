using Newtonsoft.Json;
using Shadowchats.Authentication.Core.Application.Interfaces;
using Shadowchats.Authentication.Core.Domain.Aggregates;
using Shadowchats.Authentication.Core.Domain.Exceptions;
using Shadowchats.Authentication.Core.Domain.Interfaces;

namespace Shadowchats.Authentication.Core.Application.UseCases
{
    namespace Login
    {
        public class LoginCommand : ICommand<LoginResult>
        {
            [JsonProperty(Required = Required.Always, PropertyName = "login")]
            public required string Login { get; init; }
            
            [JsonProperty(Required = Required.Always, PropertyName = "password")]
            public required string Password { get; init; }
        }
        
        public class LoginResult
        {
            [JsonProperty(Required = Required.Always, PropertyName = "refreshToken")]
            public required string RefreshToken { get; init; }
            
            [JsonProperty(Required = Required.Always, PropertyName = "accessToken")]
            public required string AccessToken { get; init; }
        }
        
        internal class LoginHandler : ICommandHandler<LoginCommand, LoginResult>
        {
            public LoginHandler(IAggregateRootsRepository aggregateRootsRepository, IPasswordHasher passwordHasher, IGuidGenerator guidGenerator, IDateTimeProvider dateTimeProvider, IRefreshTokenGenerator refreshTokenGenerator, IAccessTokenIssuer accessTokenIssuer)
            {
                _aggregateRootsRepository = aggregateRootsRepository;
                _passwordHasher = passwordHasher;
                _guidGenerator = guidGenerator;
                _dateTimeProvider = dateTimeProvider;
                _refreshTokenGenerator = refreshTokenGenerator;
                _accessTokenIssuer = accessTokenIssuer;
            }

            public async Task<LoginResult> Handle(LoginCommand command)
            {
                var account = await _aggregateRootsRepository.Find<Account>(a => a.Credentials.Login == command.Login);
                if (account is null || !account.Credentials.Verify(_passwordHasher, command.Password))
                    throw new AuthenticationFailedException("Login and/or password is invalid.");

                var session = Session.Create(_guidGenerator, _dateTimeProvider, _refreshTokenGenerator, account.Guid);
                await _aggregateRootsRepository.Add(session);

                return new LoginResult
                {
                    RefreshToken = session.RefreshToken,
                    AccessToken = session.GenerateAccessToken(_accessTokenIssuer, _dateTimeProvider)
                };
            }

            private readonly IAggregateRootsRepository _aggregateRootsRepository;
            
            private readonly IPasswordHasher _passwordHasher;
            
            private readonly IGuidGenerator _guidGenerator;

            private readonly IDateTimeProvider _dateTimeProvider;

            private readonly IRefreshTokenGenerator _refreshTokenGenerator;

            private readonly IAccessTokenIssuer _accessTokenIssuer;
        }
    }
}