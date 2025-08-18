using Moq;
using Shadowchats.Authentication.Core.Domain.Exceptions;
using Shadowchats.Authentication.Core.Domain.Interfaces;
using Shadowchats.Authentication.Core.Domain.ValueObjects;

namespace Shadowchats.Authentication.Core.Tests.Domain.ValueObjects;

public class CredentialsTests
{
    [Fact]
    public void Create_OK_Test()
    {
        // Arrange
        const string expectedLogin = "TeSt_LoGiN1=";
        const string password = "*** TeSt_PaSsWoRd1= ***";
        const string expectedPasswordHash = "Hash(*** TeSt_PaSsWoRd1= ***)";

        var passwordHasher = new Mock<IPasswordHasher>();
        passwordHasher.Setup(ph => ph.Hash(password)).Returns(expectedPasswordHash);

        // Act
        var credentials = Credentials.Create(passwordHasher.Object, expectedLogin, password);

        // Assert
        Assert.Equal(expectedLogin, credentials.Login);
        Assert.Equal(expectedPasswordHash, credentials.PasswordHash);

        passwordHasher.Verify(ph => ph.Hash(password), Times.Once);
    }

    [Theory]
    [InlineData("", "", "Login is empty.")]
    [InlineData("u", "", "Login is invalid; detail: length less than 4.")]
    [InlineData("uuuuuuuuuuuuuuuuu", "", "Login is invalid; detail: length greater than 16.")]
    [InlineData("uuuuuuuuuuuuuuuu", "", "Password is empty.")]
    [InlineData("uuuuuuuuuuuuuuuu", "u", "Password is invalid; detail: length less than 16.")]
    [InlineData("uuuuuuuuuuuuuuuu", "uuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuu", "Password is invalid; detail: length greater than 64.")]
    [InlineData("uuuuuuuuuuuuuuuu", "юююююююююююююююююююююю", "Password is invalid; detail: missing characters from regular expression groups [a-z], [A-Z], [0-9], [!@#$%\\^&*()_+\\-=\\[\\]{}|;':\",./\\\\<>?].")]
    [InlineData("uuuuuuuuuuuuuuuu", "ююююююююююююююююююююююh", "Password is invalid; detail: missing characters from regular expression groups [A-Z], [0-9], [!@#$%\\^&*()_+\\-=\\[\\]{}|;':\",./\\\\<>?].")]
    [InlineData("uuuuuuuuuuuuuuuu", "юююююююююююююююююююююHю", "Password is invalid; detail: missing characters from regular expression groups [a-z], [0-9], [!@#$%\\^&*()_+\\-=\\[\\]{}|;':\",./\\\\<>?].")]
    [InlineData("uuuuuuuuuuuuuuuu", "ююююююююююююююююююююю*7ю", "Password is invalid; detail: missing characters from regular expression groups [a-z], [A-Z].")]
    public void Create_InvariantViolationException_Test(string login, string password, string expectedExceptionMessage)
    {
        // Arrange
        var passwordHasher = Mock.Of<IPasswordHasher>();

        // Act
        var exception =
            Assert.Throws<InvariantViolationException>(() => Credentials.Create(passwordHasher, login, password));

        // Assert
        Assert.Equal(expectedExceptionMessage, exception.Message);
    }
    
    [Theory]
    [InlineData("TeSt_LoGiN1=", "*** TeSt_PaSsWoRd1= ***", "TeSt_LoGiN1=", "*** TeSt_PaSsWoRd1= ***", true)]
    [InlineData("TeSt_LoGiN1=", "*** TeSt_PaSsWoRd2= ***", "TeSt_LoGiN1=", "*** TeSt_PaSsWoRd1= ***", false)]
    [InlineData("TeSt_LoGiN2=", "*** TeSt_PaSsWoRd1= ***", "TeSt_LoGiN1=", "*** TeSt_PaSsWoRd1= ***", false)]
    public void Equals_Test(string login1, string password1, string login2, string password2, bool expectedHasEquals)
    {
        // Arrange
        var passwordHasher = new Mock<IPasswordHasher>();
        passwordHasher.Setup(ph => ph.Hash(password1)).Returns(password1);
        passwordHasher.Setup(ph => ph.Hash(password2)).Returns(password2);
        
        var credentials1 = Credentials.Create(passwordHasher.Object, login1, password1);
        var credentials2 = Credentials.Create(passwordHasher.Object, login2, password2);
        object? fakeCredentials1 = null;
        var fakeCredentials2 = new object();

        // Act
        var actualHasEquals1 = credentials1.Equals(credentials2);
        var actualHasEquals2 = credentials2.Equals(credentials1);

        // Assert
        Assert.Equal(expectedHasEquals, actualHasEquals1);
        Assert.Equal(expectedHasEquals, actualHasEquals2);
        Assert.False(credentials1.Equals(fakeCredentials1));
        Assert.False(credentials2.Equals(fakeCredentials1));
        Assert.False(credentials1.Equals(fakeCredentials2));
        Assert.False(credentials2.Equals(fakeCredentials2));
    }
    
    [Fact]
    public void GetHashCode_Test()
    {
        // Arrange
        const string login = "TeSt_LoGiN1=";
        const string password = "*** TeSt_PaSsWoRd1= ***";
        
        var hashCode = new HashCode();
        hashCode.Add(login);
        hashCode.Add(password);
        var expectedHashCode = hashCode.ToHashCode();

        var passwordHasher = new Mock<IPasswordHasher>();
        passwordHasher.Setup(ph => ph.Hash(password)).Returns(password);
        
        var credentials = Credentials.Create(passwordHasher.Object, login, password);

        // Act
        var actualHashCode = credentials.GetHashCode();

        // Assert
        Assert.Equal(expectedHashCode, actualHashCode);
    }
}