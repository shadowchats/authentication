// Shadowchats — Copyright (C) 2025
// Dorovskoy Alexey Vasilievich (One290 / 0ne290) <lenya.dorovskoy@mail.ru>
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version. See the LICENSE file for details.
// For full copyright and authorship information, see the COPYRIGHT file.

using Shadowchats.Authentication.Core.Application.Interfaces;
using Shadowchats.Authentication.Core.Domain.Aggregates;
using Shadowchats.Authentication.Core.Domain.Exceptions;
using Shadowchats.Authentication.Core.Domain.Interfaces;

namespace Shadowchats.Authentication.Core.Application.UseCases
{
    namespace LogoutAll
    {
        public class LogoutAllCommand : ICommand<LogoutAllResult>
        {
            public required string Login { get; init; }
            
            public required string Password { get; init; }
        }
        
        public class LogoutAllResult
        {
            public required string Message { get; init; }
        }

        public class LogoutAllHandler(
            IAggregateRootRepository<Account> accountRepository,
            IAggregateRootRepository<Session> sessionRepository,
            IPasswordHasher passwordHasher
        ) : ICommandHandler<LogoutAllCommand, LogoutAllResult>
        {
            public async Task<LogoutAllResult> Handle(LogoutAllCommand command)
            {
                var account = await accountRepository.Find(a => a.Credentials.Login == command.Login);
                if (account is null || !account.Credentials.VerifyPassword(passwordHasher, command.Password))
                    throw new AuthenticationFailedException("Login and/or password is invalid.");

                foreach (var session in await sessionRepository.FindAll(s =>
                             s.AccountId == account.Guid && s.IsActive))
                    session.Revoke();

                return new LogoutAllResult
                {
                    Message = "All active sessions revoked."
                };
            }
        }
    }
}