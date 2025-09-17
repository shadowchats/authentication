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
using Shadowchats.Authentication.Core.Domain.Interfaces;

namespace Shadowchats.Authentication.Core.Application.Jobs
{
    namespace RevokeExpiredSessions
    {
        public record RevokeExpiredSessionsJob : ICommand<NoResult>;

        public class RevokeExpiredSessionsHandler : IMessageHandler<RevokeExpiredSessionsJob, NoResult>
        {
            public RevokeExpiredSessionsHandler(IAggregateRootRepository<Session> sessionRepository,
                IPersistenceContext persistenceContext, IDateTimeProvider dateTimeProvider)
            {
                _sessionRepository = sessionRepository;
                _persistenceContext = persistenceContext;
                _dateTimeProvider = dateTimeProvider;
            }

            public async Task<NoResult> Handle(RevokeExpiredSessionsJob _)
            {
                var sessions =
                    await _sessionRepository.FindAll(s => s.IsActive && s.ExpiresAt < _dateTimeProvider.UtcNow);
                if (sessions.Count > 0)
                {
                    foreach (var session in sessions)
                        session.Revoke();

                    await _persistenceContext.SaveChanges();
                }

                return NoResult.Value;
            }

            private readonly IAggregateRootRepository<Session> _sessionRepository;
            
            private readonly IPersistenceContext _persistenceContext;

            private readonly IDateTimeProvider _dateTimeProvider;
        }
    }
}