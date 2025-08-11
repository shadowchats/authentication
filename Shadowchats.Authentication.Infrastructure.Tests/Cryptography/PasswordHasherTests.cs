using Moq;
using Shadowchats.Authentication.Infrastructure.Cryptography;

namespace Shadowchats.Authentication.Infrastructure.Tests.Cryptography;

public class PasswordHasherTests
{
    [Fact]
    public void Hash_Test()
    {
        // Arrange
        var dynamicSalt = new byte[] { 17, 34 };
        var salt = new byte[] { 51, 68, 17, 34 };
        var expectedHashedPassword = 
        
        var saltsManager = new Mock<PasswordHasher.ISaltsManager>();
        saltsManager.Setup(sm => sm.GenerateDynamic()).Returns(dynamicSalt);
        saltsManager.Setup(sm => sm.CombineStaticAndDynamicSalts(dynamicSalt)).Returns(salt);

        // Act

        // Assert
    }
}
