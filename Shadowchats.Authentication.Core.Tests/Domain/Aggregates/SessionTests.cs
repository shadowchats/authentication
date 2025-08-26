using Moq;
using Shadowchats.Authentication.Core.Domain.Aggregates;
using Shadowchats.Authentication.Core.Domain.Exceptions;
using Shadowchats.Authentication.Core.Domain.Interfaces;

namespace Shadowchats.Authentication.Core.Tests.Domain.Aggregates;

public class SessionTests
{
    [Fact]
    public void Create_Test()
    {
        // Arrange
        var guid = Guid.NewGuid();
        var accountId = Guid.NewGuid();
        const string refreshToken = "refresh123";
        var now = DateTime.UtcNow;

        var guidGeneratorMock = new Mock<IGuidGenerator>();
        guidGeneratorMock.Setup(x => x.Generate()).Returns(guid);

        var dateTimeProviderMock = new Mock<IDateTimeProvider>();
        dateTimeProviderMock.Setup(x => x.UtcNow).Returns(now);

        var refreshTokenGeneratorMock = new Mock<IRefreshTokenGenerator>();
        refreshTokenGeneratorMock.Setup(x => x.Generate()).Returns(refreshToken);

        // Act
        var session = Session.Create(
            guidGeneratorMock.Object,
            dateTimeProviderMock.Object,
            refreshTokenGeneratorMock.Object,
            accountId);

        // Assert
        Assert.Equal(guid, session.Guid);
        Assert.Equal(accountId, session.AccountId);
        Assert.Equal(refreshToken, session.RefreshToken);
        Assert.True(session.IsActive);
        Assert.Equal(now.AddDays(30), session.ExpiresAt);
    }

    [Fact]
    public void Revoke_Test()
    {
        // Arrange
        var session = CreateSession();

        // Act
        session.Revoke();

        // Assert
        Assert.False(session.IsActive);
    }

    [Fact]
    public void Revoke_InvariantViolationException_Test()
    {
        // Arrange
        var session = CreateSession();
        session.Revoke();

        // Act & Assert
        Assert.Throws<InvariantViolationException>(() => session.Revoke());
    }

    [Fact]
    public void GenerateAccessToken_Test()
    {
        // Arrange
        var accountId = Guid.NewGuid();
        var session = CreateSession(accountId);

        var dateTimeProviderMock = new Mock<IDateTimeProvider>();
        dateTimeProviderMock.Setup(x => x.UtcNow).Returns(session.ExpiresAt.AddDays(-1));

        const string expectedToken = "access-token-123";
        var accessTokenIssuerMock = new Mock<IAccessTokenIssuer>();
        accessTokenIssuerMock.Setup(x => x.Issue(accountId)).Returns(expectedToken);

        // Act
        var token = session.GenerateAccessToken(accessTokenIssuerMock.Object, dateTimeProviderMock.Object);

        // Assert
        Assert.Equal(expectedToken, token);
    }

    [Fact]
    public void GenerateAccessToken_InvariantViolationException_SessionIsInactive_Test()
    {
        // Arrange
        var session = CreateSession();
        session.Revoke();

        var dateTimeProviderMock = new Mock<IDateTimeProvider>();
        dateTimeProviderMock.Setup(x => x.UtcNow).Returns(session.ExpiresAt.AddDays(-1));

        var accessTokenIssuerMock = new Mock<IAccessTokenIssuer>();

        // Act & Assert
        Assert.Throws<InvariantViolationException>(() =>
            session.GenerateAccessToken(accessTokenIssuerMock.Object, dateTimeProviderMock.Object));
    }

    [Fact]
    public void GenerateAccessToken_InvariantViolationException_SessionIsExpired_Test()
    {
        // Arrange
        var session = CreateSession();

        var dateTimeProviderMock = new Mock<IDateTimeProvider>();
        dateTimeProviderMock.Setup(x => x.UtcNow).Returns(session.ExpiresAt.AddDays(1));

        var accessTokenIssuerMock = new Mock<IAccessTokenIssuer>();

        // Act & Assert
        Assert.Throws<InvariantViolationException>(() =>
            session.GenerateAccessToken(accessTokenIssuerMock.Object, dateTimeProviderMock.Object));
    }

    private static Session CreateSession(Guid? accountId = null)
    {
        var guidGenerator = Mock.Of<IGuidGenerator>(g => g.Generate() == Guid.NewGuid());
        var dateTimeProvider = Mock.Of<IDateTimeProvider>(d => d.UtcNow == DateTime.UtcNow);
        var refreshTokenGenerator = Mock.Of<IRefreshTokenGenerator>(r => r.Generate() == "refresh-token");

        return Session.Create(
            guidGenerator,
            dateTimeProvider,
            refreshTokenGenerator,
            accountId ?? Guid.NewGuid());
    }
}