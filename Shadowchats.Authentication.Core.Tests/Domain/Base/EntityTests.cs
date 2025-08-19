// Shadowchats - Copyright (C) 2025 Доровской Алексей Васильевич
// Licensed under AGPL v3.0 - see file LICENSE

using Shadowchats.Authentication.Core.Domain.Base;
using Shadowchats.Authentication.Core.Domain.Exceptions;

namespace Shadowchats.Authentication.Core.Tests.Domain.Base;

public class EntityTests
{
    private class TestEntity(Guid guid) : Entity<TestEntity>(guid);
    
    private class TestDomainEvent : IDomainEvent;

    [Fact]
    public void Constructor_OK_Test()
    {
        // Arrange
        var expectedGuid = Guid.Parse("8043f748-208a-4659-83e6-332344e87466");

        // Act
        var testEntity = new TestEntity(expectedGuid);

        // Assert
        Assert.Equal(expectedGuid, testEntity.Guid);
        Assert.Empty(testEntity.DomainEvents);
    }
    
    [Fact]
    public void Constructor_InvariantViolationException_Test()
    {
        // Arrange
        var guid = Guid.Empty;
        const string expectedExceptionMessage = "Guid is empty.";

        // Act
        var exception = Assert.Throws<InvariantViolationException>(() => new TestEntity(guid));

        // Assert
        Assert.Equal(expectedExceptionMessage, exception.Message);
    }
    
    [Fact]
    public void AddDomainEvent_Test()
    {
        // Arrange
        var guid = Guid.Parse("8043f748-208a-4659-83e6-332344e87466");
        var testEntity = new TestEntity(guid);
        TestDomainEvent[] expectedDomainEvents = [new(), new()];

        // Act
        testEntity.AddDomainEvent(expectedDomainEvents[0]);
        testEntity.AddDomainEvent(expectedDomainEvents[1]);

        // Assert
        Assert.True(expectedDomainEvents.SequenceEqual(testEntity.DomainEvents));
    }
    
    [Fact]
    public void RemoveDomainEvent_Test()
    {
        // Arrange
        var guid = Guid.Parse("8043f748-208a-4659-83e6-332344e87466");
        var testEntity = new TestEntity(guid);
        TestDomainEvent[] domainEvents = [new(), new()];
        TestDomainEvent[] expectedDomainEvents = [domainEvents[0]];

        // Act
        testEntity.AddDomainEvent(domainEvents[0]);
        testEntity.AddDomainEvent(domainEvents[1]);
        testEntity.RemoveDomainEvent(domainEvents[1]);

        // Assert
        Assert.True(expectedDomainEvents.SequenceEqual(testEntity.DomainEvents));
    }
    
    [Fact]
    public void ClearDomainEvents_Test()
    {
        // Arrange
        var guid = Guid.Parse("8043f748-208a-4659-83e6-332344e87466");
        var testEntity = new TestEntity(guid);
        TestDomainEvent[] expectedDomainEvents = [new(), new()];

        // Act
        testEntity.AddDomainEvent(expectedDomainEvents[0]);
        testEntity.AddDomainEvent(expectedDomainEvents[1]);
        testEntity.ClearDomainEvents();

        // Assert
        Assert.Empty(testEntity.DomainEvents);
    }
    
    [Theory]
    [InlineData("8043f748-208a-4659-83e6-332344e87466", "8043f748-208a-4659-83e6-332344e87466", true)]
    [InlineData("8043f748-208a-4659-83e6-332344e87466", "16faf2be-bd56-49cb-ad42-14f87e21fdb2", false)]
    public void Equals_Test(string serializedGuid1, string serializedGuid2, bool expectedHasEquals)
    {
        // Arrange
        var guid1 = Guid.Parse(serializedGuid1);
        var guid2 = Guid.Parse(serializedGuid2);
        
        var testEntity1 = new TestEntity(guid1);
        var testEntity2 = new TestEntity(guid2);
        object? fakeTestEntity1 = null;
        var fakeTestEntity2 = new object();

        // Act
        var actualHasEquals1 = testEntity1.Equals(testEntity2);
        var actualHasEquals2 = testEntity2.Equals(testEntity1);

        // Assert
        Assert.Equal(expectedHasEquals, actualHasEquals1);
        Assert.Equal(expectedHasEquals, actualHasEquals2);
        Assert.False(testEntity1.Equals(fakeTestEntity1));
        Assert.False(testEntity2.Equals(fakeTestEntity1));
        Assert.False(testEntity1.Equals(fakeTestEntity2));
        Assert.False(testEntity2.Equals(fakeTestEntity2));
    }
    
    [Fact]
    public void GetHashCode_Test()
    {
        // Arrange
        var guid = Guid.Parse("8043f748-208a-4659-83e6-332344e87466");
        var expectedHashCode = guid.GetHashCode();
        var testEntity = new TestEntity(guid);

        // Act
        var actualHashCode = testEntity.GetHashCode();

        // Assert
        Assert.Equal(expectedHashCode, actualHashCode);
    }
}