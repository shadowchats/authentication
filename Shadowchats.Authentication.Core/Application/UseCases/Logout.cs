// Shadowchats — Copyright (C) 2025
// Dorovskoy Alexey Vasilievich (One290 / 0ne290) <lenya.dorovskoy@mail.ru>
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version. See the LICENSE file for details.
// For full copyright and authorship information, see the COPYRIGHT file.

using Newtonsoft.Json;
using Shadowchats.Authentication.Core.Application.Interfaces;
using Shadowchats.Authentication.Core.Domain.Aggregates;

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
            public LogoutHandler(IAggregateRootsRepository aggregateRootsRepository)
            {
                _aggregateRootsRepository = aggregateRootsRepository;
            }

            public async Task<LogoutResult> Handle(LogoutCommand command)
            {
                (await _aggregateRootsRepository.Find<Session>(s => s.RefreshToken == command.RefreshToken))?.Revoke();

                return new LogoutResult
                {
                    Message =
                        "According to RFC 7009, the authorization server responds with an HTTP 200 OK status code " +
                        "even if the token is invalid, expired, revoked, or was never issued."
                };
            }

            private readonly IAggregateRootsRepository _aggregateRootsRepository;
        }
    }
}