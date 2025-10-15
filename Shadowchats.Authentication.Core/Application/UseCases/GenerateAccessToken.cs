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

namespace Shadowchats.Authentication.Core.Application.UseCases.GenerateAccessToken;

public record Command : IQuery<Result>
{
    public required string RefreshToken { get; init; }
}

public record Result
{
    public required string AccessToken { get; init; }
}

public class Handler : IMessageHandler<Command, Result>
{
    public Handler(IAggregateRootRepository<Session> sessionRepository,
        IDateTimeProvider dateTimeProvider, IAccessTokenIssuer accessTokenIssuer)
    {
        _sessionRepository = sessionRepository;
        _dateTimeProvider = dateTimeProvider;
        _accessTokenIssuer = accessTokenIssuer;
    }

    public async Task<Result> Handle(Command command)
    {
        var session = await _sessionRepository.Find(s => s.RefreshToken == command.RefreshToken);
        if (session is null)
            throw new InvariantViolationException("Refresh token is invalid.");

        return new Result
        {
            AccessToken = session.GenerateAccessToken(_accessTokenIssuer, _dateTimeProvider)
        };
    }

    private readonly IAggregateRootRepository<Session> _sessionRepository;

    private readonly IDateTimeProvider _dateTimeProvider;

    private readonly IAccessTokenIssuer _accessTokenIssuer;
}