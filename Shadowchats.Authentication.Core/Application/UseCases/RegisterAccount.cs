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
using Shadowchats.Authentication.Core.Domain.ValueObjects;

namespace Shadowchats.Authentication.Core.Application.UseCases
{
    namespace RegisterAccount
    {
        public class RegisterAccountCommand : ICommand<RegisterAccountResult>
        {
            [JsonProperty(Required = Required.Always, PropertyName = "login")]
            public required string Login { get; init; }
            
            [JsonProperty(Required = Required.Always, PropertyName = "password")]
            public required string Password { get; init; }
        }
        
        public class RegisterAccountResult
        {
            [JsonProperty(Required = Required.Always, PropertyName = "message")]
            public required string Message { get; init; }
        }
        
        internal class RegisterAccountHandler : ICommandHandler<RegisterAccountCommand, RegisterAccountResult>
        {
            public RegisterAccountHandler(IAggregateRootsRepository aggregateRootsRepository, IPasswordHasher passwordHasher, IGuidGenerator guidGenerator)
            {
                _aggregateRootsRepository = aggregateRootsRepository;
                _passwordHasher = passwordHasher;
                _guidGenerator = guidGenerator;
            }

            public async Task<RegisterAccountResult> Handle(RegisterAccountCommand command)
            {
                if (await _aggregateRootsRepository.Exists<Account>(a => a.Credentials.Login == command.Login))
                    throw new InvariantViolationException("Login and/or password is invalid.");
                
                await _aggregateRootsRepository.Add(Account.Create(_guidGenerator,
                    Credentials.Create(_passwordHasher, command.Login, command.Password)));

                return new RegisterAccountResult
                {
                    Message = "Account registered."
                };
            }

            private readonly IAggregateRootsRepository _aggregateRootsRepository;
            
            private readonly IPasswordHasher _passwordHasher;
            
            private readonly IGuidGenerator _guidGenerator;
        }
    }
}