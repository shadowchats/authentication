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
    namespace Login
    {
        public class LoginCommand : ICommand<LoginResult>
        {
            public required string Login { get; init; }
            
            public required string Password { get; init; }
        }
        
        public class LoginResult
        {
            public required string RefreshToken { get; init; }
            
            public required string AccessToken { get; init; }
        }
        
        public class LoginHandler(
            IAggregateRootRepository<Account> accountRepository,
            IAggregateRootRepository<Session> sessionRepository,
            IPasswordHasher passwordHasher,
            IGuidGenerator guidGenerator,
            IDateTimeProvider dateTimeProvider,
            IRefreshTokenGenerator refreshTokenGenerator,
            IAccessTokenIssuer accessTokenIssuer)
            : ICommandHandler<LoginCommand, LoginResult>
        {
            public async Task<LoginResult> Handle(LoginCommand command)
            {
                var account = await accountRepository.Find(a => a.Credentials.Login == command.Login);
                if (account is null || !account.Credentials.VerifyPassword(passwordHasher, command.Password))
                    throw new AuthenticationFailedException("Login and/or password is invalid.");

                var session = Session.Create(guidGenerator, dateTimeProvider, refreshTokenGenerator, account.Guid);
                await sessionRepository.Add(session);

                return new LoginResult
                {
                    RefreshToken = session.RefreshToken,
                    AccessToken = session.GenerateAccessToken(accessTokenIssuer, dateTimeProvider)
                };
            }
        }
    }
}