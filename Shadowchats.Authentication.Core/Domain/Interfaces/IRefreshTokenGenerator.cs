namespace Shadowchats.Authentication.Core.Domain.Interfaces;

internal interface IRefreshTokenGenerator
{
    string Generate();
}