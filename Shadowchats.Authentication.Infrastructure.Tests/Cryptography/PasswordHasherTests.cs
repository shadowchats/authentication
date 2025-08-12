using Moq;
using Shadowchats.Authentication.Infrastructure.Cryptography;

namespace Shadowchats.Authentication.Infrastructure.Tests.Cryptography;

public class PasswordHasherTests
{
    [Fact]
    public void Hash_Test()
    {
        // Arrange
        var dynamicSalt = new byte[]
        {
            0x64, 0x79, 0x6e, 0x61, 0x6d, 0x69, 0x63, 0x20, 0x73, 0x61, 0x6c, 0x74
        }; // "dynamic salt" in UTF-8
        var salt = new byte[]
        {
            0x73, 0x74, 0x61, 0x74, 0x69, 0x63, 0x20, 0x2b, 0x20, 0x64, 0x79, 0x6e, 0x61, 0x6d, 0x69, 0x63, 0x20, 0x73,
            0x61, 0x6c, 0x74, 0x73
        }; // "static + dynamic salts" in UTF-8
        const string password = "TeSt1=";
        const string expectedHashedPassword =
            "ZHluYW1pYyBzYWx0cqmIOh5nzkHUJXiTFjPerBmNfHNp73HD1p4LGU77UXFjNptJ9en9b362qpF1VQuDowOQ/1JjecNCJo+GnVhGrw=="; // Base64(dynamicSalt + PBKDF2("SHA512", password, salt, 100_000, 64))
        
        var saltsManager = new Mock<PasswordHasher.ISaltsManager>();
        saltsManager.Setup(sm => sm.GenerateDynamic()).Returns(dynamicSalt);
        saltsManager.Setup(sm => sm.CombineStaticAndDynamicSalts(dynamicSalt)).Returns(salt);

        var passwordHasher = new PasswordHasher(saltsManager.Object);

        // Act
        var actualHashedPassword = passwordHasher.Hash(password);

        // Assert
        Assert.Equal(expectedHashedPassword, actualHashedPassword);
        
        saltsManager.Verify(sm => sm.GenerateDynamic(), Times.Once);
        saltsManager.Verify(sm => sm.CombineStaticAndDynamicSalts(dynamicSalt), Times.Once);
    }
    
    [Fact]
    public void Verify_False_Test()
    {
        throw new NotImplementedException();
        
        // Arrange
        var dynamicSalt = new byte[]
        {
            0x64, 0x79, 0x6e, 0x61, 0x6d, 0x69, 0x63, 0x20, 0x73, 0x61, 0x6c, 0x74
        }; // "dynamic salt" in UTF-8
        var salt = new byte[]
        {
            0x73, 0x74, 0x61, 0x74, 0x69, 0x63, 0x20, 0x2b, 0x20, 0x64, 0x79, 0x6e, 0x61, 0x6d, 0x69, 0x63, 0x20, 0x73,
            0x61, 0x6c, 0x74, 0x73
        }; // "static + dynamic salts" in UTF-8
        const string password = "TeSt1=";
        const string expectedHashedPassword =
            "ZHluYW1pYyBzYWx0cqmIOh5nzkHUJXiTFjPerBmNfHNp73HD1p4LGU77UXFjNptJ9en9b362qpF1VQuDowOQ/1JjecNCJo+GnVhGrw=="; // Base64(dynamicSalt + PBKDF2("SHA512", password, salt, 100_000, 64))
        
        var saltsManager = new Mock<PasswordHasher.ISaltsManager>();
        saltsManager.Setup(sm => sm.GenerateDynamic()).Returns(dynamicSalt);
        saltsManager.Setup(sm => sm.CombineStaticAndDynamicSalts(dynamicSalt)).Returns(salt);

        var passwordHasher = new PasswordHasher(saltsManager.Object);

        // Act
        var actualHashedPassword = passwordHasher.Hash(password);

        // Assert
        Assert.Equal(expectedHashedPassword, actualHashedPassword);
        
        saltsManager.Verify(sm => sm.GenerateDynamic(), Times.Once);
        saltsManager.Verify(sm => sm.CombineStaticAndDynamicSalts(dynamicSalt), Times.Once);
    }
    
    [Fact]
    public void Verify_True_Test()
    {
        throw new NotImplementedException();
        
        // Arrange
        var dynamicSalt = new byte[]
        {
            0x64, 0x79, 0x6e, 0x61, 0x6d, 0x69, 0x63, 0x20, 0x73, 0x61, 0x6c, 0x74
        }; // "dynamic salt" in UTF-8
        var salt = new byte[]
        {
            0x73, 0x74, 0x61, 0x74, 0x69, 0x63, 0x20, 0x2b, 0x20, 0x64, 0x79, 0x6e, 0x61, 0x6d, 0x69, 0x63, 0x20, 0x73,
            0x61, 0x6c, 0x74, 0x73
        }; // "static + dynamic salts" in UTF-8
        const string password = "TeSt1=";
        const string expectedHashedPassword =
            "ZHluYW1pYyBzYWx0cqmIOh5nzkHUJXiTFjPerBmNfHNp73HD1p4LGU77UXFjNptJ9en9b362qpF1VQuDowOQ/1JjecNCJo+GnVhGrw=="; // Base64(dynamicSalt + PBKDF2("SHA512", password, salt, 100_000, 64))
        
        var saltsManager = new Mock<PasswordHasher.ISaltsManager>();
        saltsManager.Setup(sm => sm.GenerateDynamic()).Returns(dynamicSalt);
        saltsManager.Setup(sm => sm.CombineStaticAndDynamicSalts(dynamicSalt)).Returns(salt);

        var passwordHasher = new PasswordHasher(saltsManager.Object);

        // Act
        var actualHashedPassword = passwordHasher.Hash(password);

        // Assert
        Assert.Equal(expectedHashedPassword, actualHashedPassword);
        
        saltsManager.Verify(sm => sm.GenerateDynamic(), Times.Once);
        saltsManager.Verify(sm => sm.CombineStaticAndDynamicSalts(dynamicSalt), Times.Once);
    }
    
    [Fact]
    public void RealSaltsManager_CombineStaticAndDynamicSalts_Test()
    {
        throw new NotImplementedException();
        
        // Arrange
        var dynamicSalt = new byte[]
        {
            0x64, 0x79, 0x6e, 0x61, 0x6d, 0x69, 0x63, 0x20, 0x73, 0x61, 0x6c, 0x74
        }; // "dynamic salt" in UTF-8
        var salt = new byte[]
        {
            0x73, 0x74, 0x61, 0x74, 0x69, 0x63, 0x20, 0x2b, 0x20, 0x64, 0x79, 0x6e, 0x61, 0x6d, 0x69, 0x63, 0x20, 0x73,
            0x61, 0x6c, 0x74, 0x73
        }; // "static + dynamic salts" in UTF-8
        const string password = "TeSt1=";
        const string expectedHashedPassword =
            "ZHluYW1pYyBzYWx0cqmIOh5nzkHUJXiTFjPerBmNfHNp73HD1p4LGU77UXFjNptJ9en9b362qpF1VQuDowOQ/1JjecNCJo+GnVhGrw=="; // Base64(dynamicSalt + PBKDF2("SHA512", password, salt, 100_000, 64))
        
        var saltsManager = new Mock<PasswordHasher.ISaltsManager>();
        saltsManager.Setup(sm => sm.GenerateDynamic()).Returns(dynamicSalt);
        saltsManager.Setup(sm => sm.CombineStaticAndDynamicSalts(dynamicSalt)).Returns(salt);

        var passwordHasher = new PasswordHasher(saltsManager.Object);

        // Act
        var actualHashedPassword = passwordHasher.Hash(password);

        // Assert
        Assert.Equal(expectedHashedPassword, actualHashedPassword);
        
        saltsManager.Verify(sm => sm.GenerateDynamic(), Times.Once);
        saltsManager.Verify(sm => sm.CombineStaticAndDynamicSalts(dynamicSalt), Times.Once);
    }
    
    [Fact]
    public void RealSaltsManager_GenerateDynamic_Test()
    {
        throw new NotImplementedException();
        
        // Arrange
        var dynamicSalt = new byte[]
        {
            0x64, 0x79, 0x6e, 0x61, 0x6d, 0x69, 0x63, 0x20, 0x73, 0x61, 0x6c, 0x74
        }; // "dynamic salt" in UTF-8
        var salt = new byte[]
        {
            0x73, 0x74, 0x61, 0x74, 0x69, 0x63, 0x20, 0x2b, 0x20, 0x64, 0x79, 0x6e, 0x61, 0x6d, 0x69, 0x63, 0x20, 0x73,
            0x61, 0x6c, 0x74, 0x73
        }; // "static + dynamic salts" in UTF-8
        const string password = "TeSt1=";
        const string expectedHashedPassword =
            "ZHluYW1pYyBzYWx0cqmIOh5nzkHUJXiTFjPerBmNfHNp73HD1p4LGU77UXFjNptJ9en9b362qpF1VQuDowOQ/1JjecNCJo+GnVhGrw=="; // Base64(dynamicSalt + PBKDF2("SHA512", password, salt, 100_000, 64))
        
        var saltsManager = new Mock<PasswordHasher.ISaltsManager>();
        saltsManager.Setup(sm => sm.GenerateDynamic()).Returns(dynamicSalt);
        saltsManager.Setup(sm => sm.CombineStaticAndDynamicSalts(dynamicSalt)).Returns(salt);

        var passwordHasher = new PasswordHasher(saltsManager.Object);

        // Act
        var actualHashedPassword = passwordHasher.Hash(password);

        // Assert
        Assert.Equal(expectedHashedPassword, actualHashedPassword);
        
        saltsManager.Verify(sm => sm.GenerateDynamic(), Times.Once);
        saltsManager.Verify(sm => sm.CombineStaticAndDynamicSalts(dynamicSalt), Times.Once);
    }
}
