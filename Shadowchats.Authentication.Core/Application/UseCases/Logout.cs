// Shadowchats — Copyright (C) 2025
// Dorovskoy Alexey Vasilievich (One290 / 0ne290) <lenya.dorovskoy@mail.ru>
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version. See the LICENSE file for details.
// For full copyright and authorship information, see the COPYRIGHT file.

using Shadowchats.Authentication.Core.Application.Interfaces;
using Shadowchats.Authentication.Core.Domain.Aggregates;

namespace Shadowchats.Authentication.Core.Application.UseCases
{
    namespace Logout
    {
        public class LogoutCommand : ICommand<LogoutResult>
        {
            public required string RefreshToken { get; init; }
        }
        
        public class LogoutResult
        {
            public required string Message { get; init; }
        }

        public class LogoutHandler(IAggregateRootRepository<Session> sessionRepository, IPersistenceContext persistenceContext)
            : ICommandHandler<LogoutCommand, LogoutResult>
        {
            public async Task<LogoutResult> Handle(LogoutCommand command)
            {
                var session = await sessionRepository.Find(s => s.RefreshToken == command.RefreshToken);
                if (session is not null)
                {
                    session.Revoke();
                    await persistenceContext.SaveChanges();
                }

                return new LogoutResult
                {
                    Message =
                        "According to RFC 7009, the authorization server responds with an HTTP 200 OK status code " +
                        "even if the token is invalid, expired, revoked, or was never issued."
                };
            }
        }
    }
}