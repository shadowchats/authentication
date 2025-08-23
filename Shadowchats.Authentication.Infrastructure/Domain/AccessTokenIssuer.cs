using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using Shadowchats.Authentication.Core.Domain.Exceptions;
using Shadowchats.Authentication.Core.Domain.Interfaces;

namespace Shadowchats.Authentication.Infrastructure.Domain;

public class AccessTokenIssuer : IAccessTokenIssuer
{
    public class JwtSettings
    {
        public required byte[] SecretKey
        {
            get => _secretKey;
            init
            {
                if (value.Length < 32)
                    throw new BugException("JWT SecretKey must be at least 32 bytes.");

                _secretKey = value;
            }
        }
        private readonly byte[] _secretKey = null!;
        
        public required string Issuer
        {
            get => _issuer;
            init
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new BugException("JWT Issuer is required.");

                _issuer = value;
            }
        }
        private readonly string _issuer = null!;
        
        public required string Audience
        {
            get => _audience;
            init
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new BugException("JWT Audience is required.");

                _audience = value;
            }
        }
        private readonly string _audience = null!;
        
        public const int TokenLifitimeInMinutes = 15;
    }

    public AccessTokenIssuer(JwtSettings jwtSettings, IDateTimeProvider dateTimeProvider)
    {
        _jwtSettings = jwtSettings;
        _dateTimeProvider = dateTimeProvider;
    }

    public string Issue(Guid sessionId)
    {
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Jti, sessionId.ToString())
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

    public bool TryParse(string accessToken, out Guid sessionId)
    {
        sessionId = Guid.Empty;

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

            return Guid.TryParse(jtiClaim, out sessionId);
        }
        catch
        {
            return false;
        }
    }

    private readonly JwtSettings _jwtSettings;
    private readonly IDateTimeProvider _dateTimeProvider;
}