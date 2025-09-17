// Shadowchats — Copyright (C) 2025
// Dorovskoy Alexey Vasilievich (One290 / 0ne290) <lenya.dorovskoy@mail.ru>
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version. See the LICENSE file for details.
// For full copyright and authorship information, see the COPYRIGHT file.

using Shadowchats.Authentication.Core.Application.Base;
using Shadowchats.Authentication.Core.Application.Interfaces;
using Shadowchats.Authentication.Core.Domain.Aggregates;

namespace Shadowchats.Authentication.Core.Application.UseCases
{
    namespace Logout
    {
        public record LogoutCommand : ICommand<NoResult>
        {
            public required string RefreshToken { get; init; }
        }

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
    }
}