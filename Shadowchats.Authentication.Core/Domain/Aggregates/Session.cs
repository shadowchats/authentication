using Shadowchats.Authentication.Core.Domain.Base;
using Shadowchats.Authentication.Core.Domain.Exceptions;
using Shadowchats.Authentication.Core.Domain.Interfaces;

namespace Shadowchats.Authentication.Core.Domain.Aggregates;

internal class Session : AggregateRoot<Session>
{
    public static Session Create(IGuidGenerator guidGenerator, IDateTimeProvider dateTimeProvider, IRefreshTokenGenerator refreshTokenGenerator,
        Guid accountId) => new(guidGenerator.Generate(), accountId, dateTimeProvider.UtcNow.AddDays(LifetimeInDays),
        refreshTokenGenerator.Generate(), true);

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
        if (!IsActive || dateTimeProvider.UtcNow > ExpiresAt)
            throw new InvariantViolationException("Refresh token is invalid.");
        
        return accessTokenIssuer.Issue(Guid);
    }
    
    public Guid AccountId { get; }
    public DateTime ExpiresAt { get; }
    public string RefreshToken { get; }
    public bool IsActive { get; private set; }

    private const int LifetimeInDays = 30;
}