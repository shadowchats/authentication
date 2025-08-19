// Shadowchats - Copyright (C) 2025 Доровской Алексей Васильевич
// Licensed under AGPL v3.0 - see file LICENSE

namespace Shadowchats.Authentication.Infrastructure.Tests;

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