using Newtonsoft.Json;
using Shadowchats.Authentication.Core.Application.Interfaces;

namespace Shadowchats.Authentication.Core.Application.UseCases
{
    namespace Logout
    {
        public class LogoutCommand : ICommand<LogoutResult>
        {
            [JsonProperty(Required = Required.Always, PropertyName = "refreshToken")]
            public required string RefreshToken { get; init; }
        }
        
        public class LogoutResult
        {
            [JsonProperty(Required = Required.Always, PropertyName = "message")]
            public required string Message { get; init; }
        }

        internal class LogoutHandler : ICommandHandler<LogoutCommand, LogoutResult>
        {
            public LogoutHandler(ISessionRepository sessionRepository)
            {
                _sessionRepository = sessionRepository;
            }

            public async Task<LogoutResult> Handle(LogoutCommand command)
            {
                (await _sessionRepository.GetByRefreshToken(command.RefreshToken))?.Revoke();

                return Result;
            }

            private readonly ISessionRepository _sessionRepository;

            private static readonly LogoutResult Result = new()
            {
                Message = "According to RFC 7009, the authorization server responds with an HTTP 200 OK status code " +
                          "even if the token is invalid, expired, revoked, or was never issued."
            };
        }
    }
}