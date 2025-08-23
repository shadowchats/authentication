// Shadowchats — Copyright (C) 2025
// Dorovskoy Alexey Vasilievich (One290 / 0ne290) <lenya.dorovskoy@mail.ru>
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version. See the LICENSE file for details.
// For full copyright and authorship information, see the COPYRIGHT file.

using Shadowchats.Authentication.Infrastructure.System;

namespace Shadowchats.Authentication.Infrastructure.Tests.System;

public class GuidGeneratorTests
{
    [Fact]
    public void Generate_Test()
    {
        // Arrange
        var guidGenerator = new GuidGenerator();
        var sampleSize = Random.Shared.Next(1_000, 100_000);
        var guids = new Guid[sampleSize];

        // Act
        for (var i = 0; i < sampleSize; i++)
            guids[i] = guidGenerator.Generate();

        // Assert
        Assert.Distinct(guids);
    }
}