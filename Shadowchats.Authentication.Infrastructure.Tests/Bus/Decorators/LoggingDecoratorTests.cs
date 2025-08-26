using Microsoft.Extensions.Logging;
using Moq;
using Shadowchats.Authentication.Core.Application.Interfaces;
using Shadowchats.Authentication.Core.Domain.Exceptions;
using Shadowchats.Authentication.Infrastructure.Bus.Decorators;
using Shadowchats.Authentication.Infrastructure.Tests.Bus.Shadowchats.Authentication.Infrastructure.Tests.Bus.Fakes;

namespace Shadowchats.Authentication.Infrastructure.Tests.Bus.Decorators;

public class LoggingDecoratorTests
{
    [Fact]
    public async Task Handle_StartAndSuccess_Test()
    {
        // Arrange
        var command = new TestCommand { Data = "test" };
        var expectedResult = new TestResult { Value = "result" };
        
        var logger = new Mock<ILogger<LoggingDecorator<TestCommand, TestResult>>>();
        var decoratedHandler = new Mock<ICommandHandler<TestCommand, TestResult>>();
        decoratedHandler.Setup(h => h.Handle(command)).ReturnsAsync(expectedResult);
        
        var decorator = new LoggingDecorator<TestCommand, TestResult>(
            logger.Object, decoratedHandler.Object);
        
        // Act
        var result = await decorator.Handle(command);
        
        // Assert
        Assert.Equal(expectedResult, result);
        
        VerifyLog(logger, LogLevel.Information, "Stage: Start");
        VerifyLog(logger, LogLevel.Information, "Stage: Success");
        decoratedHandler.Verify(h => h.Handle(command), Times.Once);
    }
    
    [Fact]
    public async Task Handle_ExpectedFailure_Test()
    {
        // Arrange
        var command = new TestCommand();
        var expectedException = new InvariantViolationException("Test error");
        
        var logger = new Mock<ILogger<LoggingDecorator<TestCommand, TestResult>>>();
        var decoratedHandler = new Mock<ICommandHandler<TestCommand, TestResult>>();
        decoratedHandler.Setup(h => h.Handle(command)).ThrowsAsync(expectedException);
        
        var decorator = new LoggingDecorator<TestCommand, TestResult>(
            logger.Object, decoratedHandler.Object);
        
        // Act & Assert
        var thrownException = await Assert.ThrowsAsync<InvariantViolationException>(
            () => decorator.Handle(command));
        
        Assert.Same(expectedException, thrownException);
        VerifyLog(logger, LogLevel.Information, "Stage: ExpectedFailure");
    }
    
    [Fact]
    public async Task Handle_UnexpectedFailure_Test()
    {
        // Arrange
        var command = new TestCommand();
        var unexpectedException = new InvalidOperationException("Unexpected");
        
        var logger = new Mock<ILogger<LoggingDecorator<TestCommand, TestResult>>>();
        var decoratedHandler = new Mock<ICommandHandler<TestCommand, TestResult>>();
        decoratedHandler.Setup(h => h.Handle(command)).ThrowsAsync(unexpectedException);
        
        var decorator = new LoggingDecorator<TestCommand, TestResult>(
            logger.Object, decoratedHandler.Object);
        
        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => decorator.Handle(command));
        
        VerifyLog(logger, LogLevel.Error, "Stage: UnexpectedFailure");
    }
    
    private static void VerifyLog<T>(Mock<ILogger<T>> logger, LogLevel level, string message)
    {
        logger.Verify(
            x => x.Log(
                level,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains(message)),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }
}