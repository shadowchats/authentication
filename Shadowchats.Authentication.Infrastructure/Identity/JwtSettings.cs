using Shadowchats.Authentication.Core.Domain.Exceptions;

namespace Shadowchats.Authentication.Infrastructure.Identity;

internal class JwtSettings
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