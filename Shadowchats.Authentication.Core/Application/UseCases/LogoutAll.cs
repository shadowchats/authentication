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
using Shadowchats.Authentication.Core.Domain.Exceptions;
using Shadowchats.Authentication.Core.Domain.Interfaces;

namespace Shadowchats.Authentication.Core.Application.UseCases
{
    namespace LogoutAll
    {
        public class LogoutAllCommand : ICommand<LogoutAllResult>
        {
            [JsonProperty(Required = Required.Always, PropertyName = "login")]
            public required string Login { get; init; }
            
            [JsonProperty(Required = Required.Always, PropertyName = "password")]
            public required string Password { get; init; }
        }
        
        public class LogoutAllResult
        {
            [JsonProperty(Required = Required.Always, PropertyName = "message")]
            public required string Message { get; init; }
        }
        
        internal class LogoutAllHandler : ICommandHandler<LogoutAllCommand, LogoutAllResult>
        {
            public LogoutAllHandler(IAggregateRootsRepository aggregateRootsRepository, IPasswordHasher passwordHasher)
            {
                _aggregateRootsRepository = aggregateRootsRepository;
                _passwordHasher = passwordHasher;
            }

            public async Task<LogoutAllResult> Handle(LogoutAllCommand command)
            {
                var account = await _aggregateRootsRepository.Find<Account>(a => a.Credentials.Login == command.Login);
                if (account is null || !account.Credentials.Verify(_passwordHasher, command.Password))
                    throw new AuthenticationFailedException("Login and/or password is invalid.");

                foreach (var session in await _aggregateRootsRepository.FindAll<Session>(s => s.AccountId == account.Guid && s.IsActive))
                    session.Revoke();

                return new LogoutAllResult
                {
                    Message = "All active sessions revoked."
                };
            }

            private readonly IAggregateRootsRepository _aggregateRootsRepository;

            private readonly IPasswordHasher _passwordHasher;
        }
    }
}