// Shadowchats — Copyright (C) 2025
// Dorovskoy Alexey Vasilievich (One290 / 0ne290) <lenya.dorovskoy@mail.ru>
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version. See the LICENSE file for details.
// For full copyright and authorship information, see the COPYRIGHT file.

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