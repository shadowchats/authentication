namespace Shadowchats.Authentication.Core.Domain.Interfaces;

internal interface IAccessTokenIssuer
{
    string Issue(Guid accountId);
    bool TryParse(string token, out Guid accountId);
}