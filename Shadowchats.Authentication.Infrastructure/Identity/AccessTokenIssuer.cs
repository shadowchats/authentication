using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using Shadowchats.Authentication.Core.Domain.Interfaces;

namespace Shadowchats.Authentication.Infrastructure.Identity;

internal class AccessTokenIssuer : IAccessTokenIssuer
{
    public AccessTokenIssuer(JwtSettings jwtSettings, IDateTimeProvider dateTimeProvider)
    {
        _jwtSettings = jwtSettings;
        _dateTimeProvider = dateTimeProvider;
    }

    public string Issue(Guid accountId)
    {
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Jti, accountId.ToString())
        };
        var now = _dateTimeProvider.UtcNow;
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = now.AddMinutes(JwtSettings.TokenLifitimeInMinutes),
            IssuedAt = now,
            NotBefore = now,
            Issuer = _jwtSettings.Issuer,
            Audience = _jwtSettings.Audience,
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(_jwtSettings.SecretKey),
                SecurityAlgorithms.HmacSha256Signature)
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
                ValidAlgorithms = [SecurityAlgorithms.HmacSha256Signature]
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(accessToken, validationParameters, out _);
            var jtiClaim = principal.FindFirst(JwtRegisteredClaimNames.Jti)?.Value;

            return Guid.TryParse(jtiClaim, out accountId);
        }
        catch
        {
            return false;
        }
    }

    private readonly JwtSettings _jwtSettings;
    private readonly IDateTimeProvider _dateTimeProvider;
}