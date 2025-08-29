// Shadowchats — Copyright (C) 2025
// Dorovskoy Alexey Vasilievich (One290 / 0ne290) <lenya.dorovskoy@mail.ru>
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version. See the LICENSE file for details.
// For full copyright and authorship information, see the COPYRIGHT file.

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Shadowchats.Authentication.Core.Domain.Interfaces;

namespace Shadowchats.Authentication.Infrastructure.Identity;

public class AccessTokenIssuer : IAccessTokenIssuer
{
    public AccessTokenIssuer(IOptions<JwtSettings> options, IDateTimeProvider dateTimeProvider)
    {
        _jwtSettings = options.Value;
        _dateTimeProvider = dateTimeProvider;
    }

    public string Issue(Guid accountId)
    {
        var now = _dateTimeProvider.UtcNow;
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity([
                new Claim(JwtRegisteredClaimNames.Sub, accountId.ToString())
            ]),
            Expires = now.AddMinutes(JwtSettings.TokenLifitimeInMinutes),
            IssuedAt = now,
            NotBefore = now,
            Issuer = _jwtSettings.Issuer,
            Audience = _jwtSettings.Audience,
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(_jwtSettings.SecretKey),
                SecurityAlgorithms.HmacSha256)
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    public bool TryParse(string accessToken, out Guid accountId)
    {
        accountId = Guid.Empty;

        try
        {
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = _jwtSettings.Issuer,
                ValidateAudience = true,
                ValidAudience = _jwtSettings.Audience,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(_jwtSettings.SecretKey),
                ValidAlgorithms = [SecurityAlgorithms.HmacSha256]
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(accessToken, validationParameters, out _);
            var subClaim = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            return Guid.TryParse(subClaim, out accountId);
        }
        catch
        {
            return false;
        }
    }

    private readonly JwtSettings _jwtSettings;
    private readonly IDateTimeProvider _dateTimeProvider;
}