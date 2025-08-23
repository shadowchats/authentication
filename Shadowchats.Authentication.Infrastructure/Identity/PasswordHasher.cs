// Shadowchats — Copyright (C) 2025
// Dorovskoy Alexey Vasilievich (One290 / 0ne290) <lenya.dorovskoy@mail.ru>
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version. See the LICENSE file for details.
// For full copyright and authorship information, see the COPYRIGHT file.

using System.Security.Cryptography;
using Shadowchats.Authentication.Core.Domain.Interfaces;

namespace Shadowchats.Authentication.Infrastructure.Identity;

public class PasswordHasher : IPasswordHasher
{
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
