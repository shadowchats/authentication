using System.Linq.Expressions;
using Moq;
using Shadowchats.Authentication.Core.Application.Interfaces;
using Shadowchats.Authentication.Core.Application.UseCases.RegisterAccount;
using Shadowchats.Authentication.Core.Domain.Aggregates;
using Shadowchats.Authentication.Core.Domain.Exceptions;
using Shadowchats.Authentication.Core.Domain.Interfaces;

namespace Shadowchats.Authentication.Core.Tests.Application.UseCases;

public class RegisterAccountTests
{
    [Fact]
    public async Task Handle_InvariantViolationException_Test()
    {
        // Arrange
        var repository = new Mock<IAggregateRootsRepository>();
        repository.Setup(r => r.Exists(It.IsAny<Expression<Func<Account, bool>>>())).ReturnsAsync(true);

        var handler =
            new RegisterAccountHandler(repository.Object, Mock.Of<IPasswordHasher>(), Mock.Of<IGuidGenerator>());

        // Act & Assert
        await Assert.ThrowsAsync<InvariantViolationException>(() =>
            handler.Handle(new RegisterAccountCommand { Login = "login", Password = "pass" }));
    }

    [Fact]
    public async Task RegisterAccount_Test()
    {
        // Arrange
        var repository = new Mock<IAggregateRootsRepository>();
        repository.Setup(r => r.Exists(It.IsAny<Expression<Func<Account, bool>>>())).ReturnsAsync(false);

        var handler = new RegisterAccountHandler(repository.Object, Mock.Of<IPasswordHasher>(), Mock.Of<IGuidGenerator>());
        
        // Act
        var result = await handler.Handle(new RegisterAccountCommand { Login = "login", Password = "pass" });

        // Assert
        Assert.Equal("Account registered.", result.Message);
        repository.Verify(r => r.Add(It.IsAny<Account>()), Times.Once);
    }
}