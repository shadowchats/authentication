namespace Shadowchats.Authentication.Core.Domain.Interfaces;

internal interface IAccessTokenIssuer
{
    string Issue(Guid sessionId);
    bool TryParse(string token, out Guid sessionId);
}