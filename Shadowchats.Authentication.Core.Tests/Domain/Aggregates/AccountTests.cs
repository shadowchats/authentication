// Shadowchats - Copyright (C) 2025 Доровской Алексей Васильевич
// Licensed under AGPL v3.0 - see file LICENSE

using Moq;
using Shadowchats.Authentication.Core.Domain.Aggregates;
using Shadowchats.Authentication.Core.Domain.Interfaces;
using Shadowchats.Authentication.Core.Domain.ValueObjects;

namespace Shadowchats.Authentication.Core.Tests.Domain.Aggregates;

public class AccountTests
{
    [Fact]
    public void Create_Test()
    {
        // Arrange
        var expectedGuid = Guid.Parse("4d0dd4bf-bc64-4df8-b037-cfbad9337d0a");
        var exceptedCredentials = Credentials.Create(Mock.Of<IPasswordHasher>(), "test", "***** tesT1 *****");
        
        var guidGenerator = new Mock<IGuidGenerator>();
        guidGenerator.Setup(gg => gg.Generate()).Returns(expectedGuid);

        // Act
        var account = Account.Create(guidGenerator.Object, exceptedCredentials);

        // Assert
        Assert.Equal(expectedGuid, account.Guid);
        Assert.Equal(exceptedCredentials, account.Credentials);
        
        guidGenerator.Verify(gg => gg.Generate(), Times.Once);
    }
}