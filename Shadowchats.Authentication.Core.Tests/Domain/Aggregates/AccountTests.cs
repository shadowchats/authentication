// Shadowchats — Copyright (C) 2025
// Dorovskoy Alexey Vasilievich (One290 / 0ne290) <lenya.dorovskoy@mail.ru>
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version. See the LICENSE file for details.
// For full copyright and authorship information, see the COPYRIGHT file.

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