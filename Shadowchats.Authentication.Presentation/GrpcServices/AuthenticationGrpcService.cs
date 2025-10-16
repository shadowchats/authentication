// Shadowchats — Copyright (C) 2025
// Dorovskoy Alexey Vasilievich (One290 / 0ne290) <lenya.dorovskoy@mail.ru>
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version. See the LICENSE file for details.
// For full copyright and authorship information, see the COPYRIGHT file.

using Grpc.Core;
using Shadowchats.Authentication.Core.Application.Base;
using Shadowchats.Authentication.Core.Application.Interfaces;
using Shadowchats.Authentication.Core.Application.UseCases.GenerateAccessToken;
using Shadowchats.Authentication.Core.Application.UseCases.Login;
using Shadowchats.Authentication.Core.Application.UseCases.Logout;
using Shadowchats.Authentication.Core.Application.UseCases.LogoutAll;
using Shadowchats.Authentication.Core.Application.UseCases.RegisterAccount;
using Shadowchats.Authentication.Presentation.Grpc;

namespace Shadowchats.Authentication.Presentation.GrpcServices;

public class AuthenticationGrpcService(IBus bus) : AuthenticationService.AuthenticationServiceBase
{
    public override async Task<RegisterAccountResponse> RegisterAccount(RegisterAccountRequest request, ServerCallContext context)
    {
        var command = new RegisterAccountCommand
        {
            Login = request.Login,
            Password = request.Password
        };

        await bus.Execute<RegisterAccountCommand, NoResult>(command);

        return new RegisterAccountResponse
        {
            Message = "Account registered."
        };
    }

    public override async Task<LoginResponse> Login(LoginRequest request, ServerCallContext context)
    {
        var command = new LoginCommand
        {
            Login = request.Login,
            Password = request.Password
        };

        var result = await bus.Execute<LoginCommand, LoginResult>(command);

        return new LoginResponse
        {
            RefreshToken = result.RefreshToken,
            AccessToken = result.AccessToken
        };
    }

    public override async Task<GenerateAccessTokenResponse> GenerateAccessToken(GenerateAccessTokenRequest request, ServerCallContext context)
    {
        var command = new GenerateAccessTokenQuery
        {
            RefreshToken = request.RefreshToken
        };

        var result = await bus.Execute<GenerateAccessTokenQuery, GenerateAccessTokenResult>(command);

        return new GenerateAccessTokenResponse
        {
            AccessToken = result.AccessToken
        };
    }

    public override async Task<LogoutResponse> Logout(LogoutRequest request, ServerCallContext context)
    {
        var command = new LogoutCommand
        {
            RefreshToken = request.RefreshToken
        };

        await bus.Execute<LogoutCommand, NoResult>(command);

        return new LogoutResponse
        {
            Message = "According to RFC 7009, the authorization server responds with an HTTP 200 OK status code " +
                      "even if the token is invalid, expired, revoked, or was never issued."
        };
    }

    public override async Task<LogoutAllResponse> LogoutAll(LogoutAllRequest request, ServerCallContext context)
    {
        var command = new LogoutAllCommand
        {
            Login = request.Login,
            Password = request.Password
        };

        await bus.Execute<LogoutAllCommand, NoResult>(command);

        return new LogoutAllResponse
        {
            Message = "All active sessions revoked."
        };
    }
}
