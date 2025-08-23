// Shadowchats — Copyright (C) 2025
// Dorovskoy Alexey Vasilievich (One290 / 0ne290) <lenya.dorovskoy@mail.ru>
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version. See the LICENSE file for details.
// For full copyright and authorship information, see the COPYRIGHT file.

using Moq;
using Shadowchats.Authentication.Infrastructure.Domain;

namespace Shadowchats.Authentication.Infrastructure.Tests;

public class PasswordHasherTests
{
    [Fact]
    public void Hash_Test()
    {
        // Arrange
        var dynamicSalt = "dynamic salt"u8.ToArray();
        var salt = "static + dynamic salts"u8.ToArray();
        const string password = "TeSt1=";
        const string expectedPasswordHash =
            "ZHluYW1pYyBzYWx0cqmIOh5nzkHUJXiTFjPerBmNfHNp73HD1p4LGU77UXFjNptJ9en9b362qpF1VQuDowOQ/1JjecNCJo+GnVhGrw=="; // Base64(dynamicSalt + PBKDF2("SHA512", password, salt, 100_000, 64))
        
        var saltsManager = new Mock<PasswordHasher.ISaltsManager>();
        saltsManager.Setup(sm => sm.GenerateDynamic()).Returns(dynamicSalt);
        saltsManager.Setup(sm => sm.CombineStaticAndDynamicSalts(dynamicSalt)).Returns(salt);

        var passwordHasher = new PasswordHasher(saltsManager.Object);

        // Act
        var actualPasswordHash = passwordHasher.Hash(password);

        // Assert
        Assert.Equal(expectedPasswordHash, actualPasswordHash);
        
        saltsManager.Verify(sm => sm.GenerateDynamic(), Times.Once);
        saltsManager.Verify(sm => sm.CombineStaticAndDynamicSalts(dynamicSalt), Times.Once);
    }
    
    [Fact]
    public void Verify_True_Test()
    {
        // Arrange
        var dynamicSalt = "dynamic salt"u8.ToArray();
        var salt = "static + dynamic salts"u8.ToArray();
        const string password = "TeSt1=";
        const string passwordHash =
            "ZHluYW1pYyBzYWx0cqmIOh5nzkHUJXiTFjPerBmNfHNp73HD1p4LGU77UXFjNptJ9en9b362qpF1VQuDowOQ/1JjecNCJo+GnVhGrw=="; // Base64(dynamicSalt + PBKDF2("SHA512", password, salt, 100_000, 64))
        
        var saltsManager = new Mock<PasswordHasher.ISaltsManager>();
        saltsManager.Setup(sm => sm.DynamicSaltSizeInBytes).Returns(dynamicSalt.Length);
        saltsManager.Setup(sm => sm.CombineStaticAndDynamicSalts(dynamicSalt)).Returns(salt);

        var passwordHasher = new PasswordHasher(saltsManager.Object);

        // Act
        var actualPasswordVerified = passwordHasher.Verify(passwordHash, password);

        // Assert
        Assert.True(actualPasswordVerified);
        
        saltsManager.Verify(sm => sm.DynamicSaltSizeInBytes, Times.Exactly(2));
        saltsManager.Verify(sm => sm.CombineStaticAndDynamicSalts(dynamicSalt), Times.Once);
    }
    
    [Fact]
    public void Verify_False_Test()
    {
        // Arrange
        var dynamicSalt = "dynamic salt"u8.ToArray();
        var salt = "static + dynamic salts"u8.ToArray();
        const string password = "TeSt2=";
        const string passwordHash =
            "ZHluYW1pYyBzYWx0cqmIOh5nzkHUJXiTFjPerBmNfHNp73HD1p4LGU77UXFjNptJ9en9b362qpF1VQuDowOQ/1JjecNCJo+GnVhGrw=="; // Base64(dynamicSalt + PBKDF2("SHA512", password, salt, 100_000, 64))
        
        var saltsManager = new Mock<PasswordHasher.ISaltsManager>();
        saltsManager.Setup(sm => sm.DynamicSaltSizeInBytes).Returns(dynamicSalt.Length);
        saltsManager.Setup(sm => sm.CombineStaticAndDynamicSalts(dynamicSalt)).Returns(salt);

        var passwordHasher = new PasswordHasher(saltsManager.Object);

        // Act
        var actualPasswordVerified = passwordHasher.Verify(passwordHash, password);

        // Assert
        Assert.False(actualPasswordVerified);
        
        saltsManager.Verify(sm => sm.DynamicSaltSizeInBytes, Times.Exactly(2));
        saltsManager.Verify(sm => sm.CombineStaticAndDynamicSalts(dynamicSalt), Times.Once);
    }
    
    [Fact]
    public void RealSaltsManager_CombineStaticAndDynamicSalts_Test()
    {
        // Arrange
        var saltsManager = new PasswordHasher.RealSaltsManager();
        var dynamicSalt = "dynamic salt"u8.ToArray();
        var staticSalt =
            Convert.FromBase64String(
                "6YcJsx9rN1fMkcBFK6R10ty2xMQYUpN6jHgsGiiw6vshDFsDNowWwhVIbyiVFXWXyc0XRQFmjPonBwKmtf1h5g==");
        var expectedSalt = staticSalt.Concat(dynamicSalt).ToArray();
        
        // Act
        var actualSalt = saltsManager.CombineStaticAndDynamicSalts(dynamicSalt);

        // Assert
        Assert.Equal(expectedSalt, actualSalt);
    }
    
    [Fact]
    public void RealSaltsManager_GenerateDynamic_Test()
    {
        // Arrange
        var saltsManager = new PasswordHasher.RealSaltsManager();
        var sampleSize = Random.Shared.Next(1_000, 100_000);
        var dynamicSalts = new byte[sampleSize][];

        // Act
        for (var i = 0; i < sampleSize; i++)
            dynamicSalts[i] = saltsManager.GenerateDynamic();

        // Assert
        Assert.Distinct(dynamicSalts.Select(Convert.ToBase64String));
        Assert.All(dynamicSalts, s => Assert.Equal(s.Length, saltsManager.DynamicSaltSizeInBytes));
    }
}
