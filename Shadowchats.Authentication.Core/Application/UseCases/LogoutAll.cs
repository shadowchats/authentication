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
using Shadowchats.Authentication.Core.Domain.Exceptions;
using Shadowchats.Authentication.Core.Domain.Interfaces;

namespace Shadowchats.Authentication.Core.Application.UseCases
{
    namespace LogoutAll
    {
        public record LogoutAllCommand : ICommand<NoResult>
        {
            public required string Login { get; init; }
            
            public required string Password { get; init; }
        }

        public class LogoutAllHandler : IMessageHandler<LogoutAllCommand, NoResult>
        {
            public LogoutAllHandler(IAggregateRootRepository<Account> accountRepository,
                IAggregateRootRepository<Session> sessionRepository, IPersistenceContext persistenceContext,
                IPasswordHasher passwordHasher)
            {
                _accountRepository = accountRepository;
                _sessionRepository = sessionRepository;
                _persistenceContext = persistenceContext;
                _passwordHasher = passwordHasher;
            }

            public async Task<NoResult> Handle(LogoutAllCommand command)
            {
                var account = await _accountRepository.Find(a => a.Credentials.Login == command.Login);
                if (account is null || !account.Credentials.VerifyPassword(_passwordHasher, command.Password))
                    throw new AuthenticationFailedException("Login and/or password is invalid.");

                var sessions = await _sessionRepository.FindAll(s => s.AccountId == account.Guid && s.IsActive);
                if (sessions.Count > 0)
                {
                    foreach (var session in sessions)
                        session.Revoke();

                    await _persistenceContext.SaveChanges();
                }
                
                return NoResult.Value;
            }
            
            private readonly IAggregateRootRepository<Account> _accountRepository;

            private readonly IAggregateRootRepository<Session> _sessionRepository;
            
            private readonly IPersistenceContext _persistenceContext;

            private readonly IPasswordHasher _passwordHasher;
        }
    }
}