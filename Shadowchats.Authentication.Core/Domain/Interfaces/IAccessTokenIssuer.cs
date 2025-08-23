namespace Shadowchats.Authentication.Core.Domain.Interfaces;

public interface IAccessTokenIssuer
{
    string Issue(Guid sessionId);
    bool TryParse(string token, out Guid sessionId);
}