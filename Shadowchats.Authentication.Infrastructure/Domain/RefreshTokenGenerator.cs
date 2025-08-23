using System.Security.Cryptography;
using Shadowchats.Authentication.Core.Domain.Interfaces;

namespace Shadowchats.Authentication.Infrastructure.Domain;

public class RefreshTokenGenerator : IRefreshTokenGenerator
{
    public string Generate() => Convert.ToBase64String(RandomNumberGenerator.GetBytes(RefreshTokenSizeInBytes));

    private const int RefreshTokenSizeInBytes = 32;
}