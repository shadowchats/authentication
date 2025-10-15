// Shadowchats — Copyright (C) 2025
// Dorovskoy Alexey Vasilievich (One290 / 0ne290) <lenya.dorovskoy@mail.ru>
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version. See the LICENSE file for details.
// For full copyright and authorship information, see the COPYRIGHT file.

using JetBrains.Annotations;
using Shadowchats.Authentication.Core.Domain.Base;
using Shadowchats.Authentication.Core.Domain.Exceptions;
using Shadowchats.Authentication.Core.Domain.Interfaces;

namespace Shadowchats.Authentication.Core.Domain.Aggregates;

public class Session : AggregateRoot<Session>
{
    public static Session Create(IGuidGenerator guidGenerator, IDateTimeProvider dateTimeProvider, IRefreshTokenGenerator refreshTokenGenerator,
        Guid accountId) => new(guidGenerator.Generate(), accountId, dateTimeProvider.UtcNow.AddDays(LifetimeInDays),
        refreshTokenGenerator.Generate(), true);
    
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    [UsedImplicitly]
    private Session() { } 
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

    private Session(Guid guid, Guid accountId, DateTime expiresAt, string refreshToken, bool isActive) : base(guid)
    {
        AccountId = accountId;
        ExpiresAt = expiresAt;
        RefreshToken = refreshToken;
        IsActive = isActive;
    }
    
    public void Revoke()
    {
        if (!IsActive)
            throw new InvariantViolationException("Session is already revoked.");
        
        IsActive = false;
    }
    
    public string GenerateAccessToken(IAccessTokenIssuer accessTokenIssuer, IDateTimeProvider dateTimeProvider)
    {
        if (!IsActive || ExpiresAt < dateTimeProvider.UtcNow)
            throw new InvariantViolationException("Refresh token is invalid.");
        
        return accessTokenIssuer.Issue(AccountId);
    }
    
    public Guid AccountId { get; private set; }
    public DateTime ExpiresAt { get; private set; }
    public string RefreshToken { get; private set; }
    public bool IsActive { get; private set; }

    private const int LifetimeInDays = 30;
}