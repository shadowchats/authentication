using Moq;
using Shadowchats.Authentication.Core.Domain.Interfaces;
using Shadowchats.Authentication.Infrastructure.Identity;

namespace Shadowchats.Authentication.Infrastructure.Tests.Identity;

public class AccessTokenIssuerTests
{
    [Fact]
    public void TryParse_True_Test()
    {
        // Arrange
        var jwtSettings = new JwtSettings
        {
            SecretKey = Convert.FromBase64String("NjJPS3QyVk10aGdueU9HQ1VhekxpNmV6THlmZGFDcTk="),
            Issuer = "Test",
            Audience = "Test"
        };
        var expectedAccountId = Guid.Parse("7e801397-2c65-4748-be5b-7d4062e564db");
        
        /*
         * {
         *     "header": {
         *         "typ": "JWT",
         *         "alg": "HS256",// key = jwtSettings.SecretKey
         *     },
         *     "payload": {
         *         "iss": "{jwtSettings.Issuer}",
         *         "iat": 1756087200,
         *         "exp": 2545005600,
         *         "aud": "{jwtSettings.Audience}",
         *         "sub": "{expectedAccoutId}",
         *         "nbf": "1756116000"
         *     }
         * }
         */
        const string accessToken = "eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJpc3MiOiJUZXN0IiwiaWF0IjoxNzU2MDg3MjAwLCJleHAiOjI1NDUwMDU2MDAsImF1ZCI6IlRlc3QiLCJzdWIiOiI3ZTgwMTM5Ny0yYzY1LTQ3NDgtYmU1Yi03ZDQwNjJlNTY0ZGIiLCJuYmYiOiIxNzU2MTE2MDAwIn0.Qk_sV4YcybnUl3VxAjxZLmdf6wvy5roNkm-vjGPFLeY";

        var accessTokenIssuer = new AccessTokenIssuer(jwtSettings, Mock.Of<IDateTimeProvider>());

        // Act
        var actualResult = accessTokenIssuer.TryParse(accessToken, out var actualAccountId);

        // Assert
        Assert.True(actualResult);
        Assert.Equal(expectedAccountId, actualAccountId);
    }
    
    // За основу взят токен доступа из метода TryParse_True_Test.
    [Theory]
    [InlineData("eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJpc3MiOiJUZXN0MSIsImlhdCI6MTc1NjA4NzIwMCwiZXhwIjoyNTQ1MDA1NjAwLCJhdWQiOiJUZXN0Iiwic3ViIjoiN2U4MDEzOTctMmM2NS00NzQ4LWJlNWItN2Q0MDYyZTU2NGRiIiwibmJmIjoiMTc1NjExNjAwMCJ9.SKNHmPr_8DL15-w4NGX5iL1G0HLxMZtY80A1pH3Si50")]// iss == "Test1"
    [InlineData("eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJpc3MiOiJUZXN0IiwiaWF0IjoxNzU2MDg3MjAwLCJleHAiOjI1NDUwMDU2MDAsImF1ZCI6IlRlc3QxIiwic3ViIjoiN2U4MDEzOTctMmM2NS00NzQ4LWJlNWItN2Q0MDYyZTU2NGRiIiwibmJmIjoiMTc1NjExNjAwMCJ9.ZO97kefXpxB3DS0KUWZ8epEnZXKLVT8CWyrdybaFYKc")]// aud == "Test1"
    [InlineData("eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJpc3MiOiJUZXN0IiwiaWF0IjoxNzU2MDg3MjAwLCJleHAiOjI1NDUwMDU2MDAsImF1ZCI6IlRlc3QiLCJzdWIiOiI3ZTgwMTM5Ny0yYzY1LTQ3NDgtYmU1Yi03ZDQwNjJlNTY0ZGIiLCJuYmYiOiIxNzU2MTE2MDAwIn0.PY46-BvgH7Dn8l6nV6JnFxeXet6zTcyvc7TnPsV9y0Q")]// Another key
    [InlineData("eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzUxMiJ9.eyJpc3MiOiJUZXN0IiwiaWF0IjoxNzU2MDg3MjAwLCJleHAiOjI1NDUwMDU2MDAsImF1ZCI6IlRlc3QiLCJzdWIiOiI3ZTgwMTM5Ny0yYzY1LTQ3NDgtYmU1Yi03ZDQwNjJlNTY0ZGIiLCJuYmYiOiIxNzU2MTE2MDAwIn0.ghAA0H8RimGIy8HL6PekaWD6NvVVui99Ku8cdqexMykD35Ns-h3iL1UnSXrzuUyoaehfNaDq1xXgNiYselgjVw")]// Another algorithm
    [InlineData("eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJpc3MiOiJUZXN0IiwiaWF0IjoxNzU2MDg3MjAwLCJleHAiOjE3NTYwODcyMDAsImF1ZCI6IlRlc3QiLCJzdWIiOiI3ZTgwMTM5Ny0yYzY1LTQ3NDgtYmU1Yi03ZDQwNjJlNTY0ZGIiLCJuYmYiOiIxNzU2MTE2MDAwIn0.1c9T77bnP4QASDy2n76WcaKivHKu9XdbdIqMjYj2Sbw")]// exp = 2025-08-25T10:00:00
    public void TryParse_False_Test(string accessToken)
    {
        // Arrange
        var jwtSettings = new JwtSettings
        {
            SecretKey = Convert.FromBase64String("NjJPS3QyVk10aGdueU9HQ1VhekxpNmV6THlmZGFDcTk="),
            Issuer = "Test",
            Audience = "Test"
        };
        
        var accessTokenIssuer = new AccessTokenIssuer(jwtSettings, Mock.Of<IDateTimeProvider>());

        // Act
        var actualResult = accessTokenIssuer.TryParse(accessToken, out _);

        // Assert
        Assert.False(actualResult);
    }

    [Fact]
    public void Issue_Test()
    {
        // Arrange
        var jwtSettings = new JwtSettings
        {
            SecretKey = Convert.FromBase64String("NjJPS3QyVk10aGdueU9HQ1VhekxpNmV6THlmZGFDcTk="),
            Issuer = "Test",
            Audience = "Test"
        };
        var expectedAccountId = Guid.Parse("7e801397-2c65-4748-be5b-7d4062e564db");
        
        var dateTimeProvider = new Mock<IDateTimeProvider>();
        dateTimeProvider.Setup(dtp => dtp.UtcNow).Returns(DateTime.UtcNow);
        
        var accessTokenIssuer = new AccessTokenIssuer(jwtSettings, dateTimeProvider.Object);

        // Act
        var accessToken = accessTokenIssuer.Issue(expectedAccountId);
        var actualResult = accessTokenIssuer.TryParse(accessToken, out var actualAccountId);

        // Assert
        Assert.True(actualResult);
        Assert.Equal(expectedAccountId, actualAccountId);
        
        dateTimeProvider.Verify(dtp => dtp.UtcNow, Times.Once);
    }
}