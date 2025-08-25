using Shadowchats.Authentication.Infrastructure.Identity;

namespace Shadowchats.Authentication.Infrastructure.Tests.Identity;

public class SaltsManagerTests
{
    [Fact]
    public void CombineStaticAndDynamicSalts_Test()
    {
        // Arrange
        var saltsManager = new SaltsManager();
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
}