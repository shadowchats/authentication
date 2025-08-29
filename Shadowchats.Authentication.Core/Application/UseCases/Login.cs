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
        
        public class LoginHandler : ICommandHandler<LoginCommand, LoginResult>
        {
            public LoginHandler(IAggregateRootsRepository aggregateRootsRepository, IPasswordHasher passwordHasher, IGuidGenerator guidGenerator, IDateTimeProvider dateTimeProvider, IRefreshTokenGenerator refreshTokenGenerator, IAccessTokenIssuer accessTokenIssuer)
            {
                _aggregateRootsRepository = aggregateRootsRepository;
                _passwordHasher = passwordHasher;
                _guidGenerator = guidGenerator;
                _dateTimeProvider = dateTimeProvider;
                _refreshTokenGenerator = refreshTokenGenerator;
                _accessTokenIssuer = accessTokenIssuer;
            }

            public async Task<LoginResult> Handle(LoginCommand command)
            {
                var account = await _aggregateRootsRepository.Find<Account>(a => a.Credentials.Login == command.Login);
                if (account is null || !account.Credentials.Verify(_passwordHasher, command.Password))
                    throw new AuthenticationFailedException("Login and/or password is invalid.");

                var session = Session.Create(_guidGenerator, _dateTimeProvider, _refreshTokenGenerator, account.Guid);
                await _aggregateRootsRepository.Add(session);

                return new LoginResult
                {
                    RefreshToken = session.RefreshToken,
                    AccessToken = session.GenerateAccessToken(_accessTokenIssuer, _dateTimeProvider)
                };
            }

            private readonly IAggregateRootsRepository _aggregateRootsRepository;
            
            private readonly IPasswordHasher _passwordHasher;
            
            private readonly IGuidGenerator _guidGenerator;

            private readonly IDateTimeProvider _dateTimeProvider;

            private readonly IRefreshTokenGenerator _refreshTokenGenerator;

            private readonly IAccessTokenIssuer _accessTokenIssuer;
        }
    }
}