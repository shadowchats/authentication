namespace Shadowchats.Authentication.Core.Domain.Interfaces;

public interface IRefreshTokenGenerator
{
    string Generate();
}