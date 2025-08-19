// Shadowchats - Copyright (C) 2025 Доровской Алексей Васильевич
// Licensed under AGPL v3.0 - see file LICENSE

using Shadowchats.Authentication.Core.Domain.Base;

namespace Shadowchats.Authentication.Core.Tests.Domain.Base;

public class ValueObjectTests
{
    private class TestValueObject : ValueObject<TestValueObject>
    {
        public required IEnumerable<object> EqualityComponents { get; init; }

        protected override IEnumerable<object> GetEqualityComponents() => EqualityComponents;
    }

    [Theory]
    [MemberData(nameof(Equals_Data))]
    public void Equals_Test(object[] equalityComponents1, object[] equalityComponents2, bool expectedHasEquals)
    {
        // Arrange
        var testValueObject1 = new TestValueObject { EqualityComponents = equalityComponents1 };
        var testValueObject2 = new TestValueObject { EqualityComponents = equalityComponents2 };
        object? fakeTestValueObject1 = null;
        var fakeTestValueObject2 = new object();

        // Act
        var actualHasEquals1 = testValueObject1.Equals(testValueObject2);
        var actualHasEquals2 = testValueObject2.Equals(testValueObject1);

        // Assert
        Assert.Equal(expectedHasEquals, actualHasEquals1);
        Assert.Equal(expectedHasEquals, actualHasEquals2);
        Assert.False(testValueObject1.Equals(fakeTestValueObject1));
        Assert.False(testValueObject2.Equals(fakeTestValueObject1));
        Assert.False(testValueObject1.Equals(fakeTestValueObject2));
        Assert.False(testValueObject2.Equals(fakeTestValueObject2));
    }

    // ReSharper disable once InconsistentNaming
    public static readonly TheoryData<object[], object[], bool> Equals_Data = new()
    {
        { [1, "test"], [1, "test"], true },
        { [], [], true },
        { [1, "test"], ["test", 1], false },
        { [false, new object()], [1, "test"], false },
        { [1, "test"], [1, "test", 1], false }
    };

    [Fact]
    public void GetHashCode_Test()
    {
        // Arrange
        object[] equalityComponents = [1, "test"];

        var hashCode = new HashCode();
        hashCode.Add(equalityComponents[0]);
        hashCode.Add(equalityComponents[1]);
        var expectedHashCode = hashCode.ToHashCode();

        var testValueObject = new TestValueObject { EqualityComponents = equalityComponents };

        // Act
        var actualHashCode = testValueObject.GetHashCode();

        // Assert
        Assert.Equal(expectedHashCode, actualHashCode);
    }
}