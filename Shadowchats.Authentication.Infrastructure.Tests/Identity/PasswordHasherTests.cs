// Shadowchats — Copyright (C) 2025
// Dorovskoy Alexey Vasilievich (One290 / 0ne290) <lenya.dorovskoy@mail.ru>
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version. See the LICENSE file for details.
// For full copyright and authorship information, see the COPYRIGHT file.

using Moq;
using Shadowchats.Authentication.Infrastructure.Identity;

namespace Shadowchats.Authentication.Infrastructure.Tests.Identity;

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
        
        var saltsManager = new Mock<ISaltsManager>();
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
        
        var saltsManager = new Mock<ISaltsManager>();
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
        
        var saltsManager = new Mock<ISaltsManager>();
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
}
