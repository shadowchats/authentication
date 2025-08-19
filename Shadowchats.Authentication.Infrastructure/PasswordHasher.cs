// Shadowchats - Copyright (C) 2025 Доровской Алексей Васильевич
// Licensed under AGPL v3.0 - see file LICENSE

using System.Security.Cryptography;
using Shadowchats.Authentication.Core.Domain.Interfaces;

namespace Shadowchats.Authentication.Infrastructure;

public class PasswordHasher : IPasswordHasher
{
    internal interface ISaltsManager
    {
        byte[] GenerateDynamic();

        byte[] CombineStaticAndDynamicSalts(IEnumerable<byte> dynamicSalt);

        int DynamicSaltSizeInBytes { get; }
    }
    
    internal class RealSaltsManager : ISaltsManager
    {
        public byte[] GenerateDynamic() => RandomNumberGenerator.GetBytes(DynamicSaltSizeInBytes);

        public byte[] CombineStaticAndDynamicSalts(IEnumerable<byte> dynamicSalt) =>
            StaticSalt.Concat(dynamicSalt).ToArray();

        public int DynamicSaltSizeInBytes { get; } = 64;
        
        private static readonly byte[] StaticSalt =
        [
            233, 135, 9, 179, 31, 107, 55, 87, 204, 145, 192, 69, 43, 164, 117, 210, 220, 182, 196, 196, 24, 82, 147, 122,
            140, 120, 44, 26, 40, 176, 234, 251, 33, 12, 91, 3, 54, 140, 22, 194, 21, 72, 111, 40, 149, 21, 117, 151, 201,
            205, 23, 69, 1, 102, 140, 250, 39, 7, 2, 166, 181, 253, 97, 230
        ]; // "6YcJsx9rN1fMkcBFK6R10ty2xMQYUpN6jHgsGiiw6vshDFsDNowWwhVIbyiVFXWXyc0XRQFmjPonBwKmtf1h5g==" in Base64
    }

    internal PasswordHasher(ISaltsManager saltsManager)
    {
        _saltsManager = saltsManager;
    }

    public string Hash(string password)
    {
        var dynamicSalt = _saltsManager.GenerateDynamic();

        return Convert.ToBase64String(
            dynamicSalt.Concat(
                HashPassword(
                    password,
                    dynamicSalt
                )
            ).ToArray()
        );
    }

    public bool Verify(string dynamicSaltAndPasswordHash, string providedPassword)
    {
        var bytesOfDynamicSaltAndPasswordHash = Convert.FromBase64String(dynamicSaltAndPasswordHash);

        var passwordHash = bytesOfDynamicSaltAndPasswordHash.Skip(_saltsManager.DynamicSaltSizeInBytes).Take(PasswordHashLength);
        var providedPasswordHash = HashPassword(providedPassword, bytesOfDynamicSaltAndPasswordHash.Take(_saltsManager.DynamicSaltSizeInBytes));

        return passwordHash.SequenceEqual(providedPasswordHash);
    }

    private byte[] HashPassword(string password, IEnumerable<byte> dynamicSalt)
    {
        return Rfc2898DeriveBytes.Pbkdf2(password, _saltsManager.CombineStaticAndDynamicSalts(dynamicSalt), IterationsNumber, HashingAlgorithmName, PasswordHashLength);
    }

    private readonly ISaltsManager _saltsManager;

    private const int IterationsNumber = 100_000;

    private static readonly HashAlgorithmName HashingAlgorithmName = HashAlgorithmName.SHA512;

    private const int PasswordHashLength = 64;
}
