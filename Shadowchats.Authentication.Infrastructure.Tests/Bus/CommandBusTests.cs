using Microsoft.Extensions.DependencyInjection;
using Moq;
using Shadowchats.Authentication.Core.Application.Interfaces;
using Shadowchats.Authentication.Infrastructure.Bus;
using Shadowchats.Authentication.Infrastructure.Tests.Bus.Shadowchats.Authentication.Infrastructure.Tests.Bus.Fakes;

namespace Shadowchats.Authentication.Infrastructure.Tests.Bus;

public class CommandBusTests
{
    [Fact]
    public async Task Execute_Test()
    {
        // Arrange
        var expectedResult = new TestResult { Value = "test" };
        var command = new TestCommand { Data = "input" };
        
        var handler = new Mock<ICommandHandler<TestCommand, TestResult>>();
        handler.Setup(h => h.Handle(command)).ReturnsAsync(expectedResult);
        
        var services = new ServiceCollection();
        services.AddSingleton(handler.Object);
        var serviceProvider = services.BuildServiceProvider();
        
        var commandBus = new CommandBus(serviceProvider);
        
        // Act
        var result = await commandBus.Execute<TestCommand, TestResult>(command);
        
        // Assert
        Assert.Equal(expectedResult, result);
        handler.Verify(h => h.Handle(command), Times.Once);
    }
    
    [Fact]
    public async Task Execute_HandlerNotRegistered_ThrowsException()
    {
        // Arrange
        var command = new TestCommand { Data = "input" };
        var serviceProvider = new ServiceCollection().BuildServiceProvider();
        var commandBus = new CommandBus(serviceProvider);
        
        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => commandBus.Execute<TestCommand, TestResult>(command));
    }
}