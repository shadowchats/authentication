// Shadowchats — Copyright (C) 2025
// Dorovskoy Alexey Vasilievich (One290 / 0ne290) <lenya.dorovskoy@mail.ru>
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version. See the LICENSE file for details.
// For full copyright and authorship information, see the COPYRIGHT file.

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
        var message = new TestMessage { Data = "test" };
        var expectedResult = new TestResult { Value = "result" };
        
        var logger = new Mock<ILogger<LoggingDecorator<TestMessage, TestResult>>>();
        var decoratedHandler = new Mock<IMessageHandler<TestMessage, TestResult>>();
        decoratedHandler.Setup(h => h.Handle(message)).ReturnsAsync(expectedResult);
        
        var decorator = new LoggingDecorator<TestMessage, TestResult>(
            logger.Object, decoratedHandler.Object);
        
        // Act
        var result = await decorator.Handle(message);
        
        // Assert
        Assert.Equal(expectedResult, result);
        
        VerifyLog(logger, LogLevel.Information, "Stage: Start");
        VerifyLog(logger, LogLevel.Information, "Stage: Success");
        decoratedHandler.Verify(h => h.Handle(message), Times.Once);
    }
    
    [Fact]
    public async Task Handle_ExpectedFailure_Test()
    {
        // Arrange
        var message = new TestMessage();
        var expectedException = new InvariantViolationException("Test error");
        
        var logger = new Mock<ILogger<LoggingDecorator<TestMessage, TestResult>>>();
        var decoratedHandler = new Mock<IMessageHandler<TestMessage, TestResult>>();
        decoratedHandler.Setup(h => h.Handle(message)).ThrowsAsync(expectedException);
        
        var decorator = new LoggingDecorator<TestMessage, TestResult>(
            logger.Object, decoratedHandler.Object);
        
        // Act & Assert
        var thrownException = await Assert.ThrowsAsync<InvariantViolationException>(
            () => decorator.Handle(message));
        
        Assert.Same(expectedException, thrownException);
        VerifyLog(logger, LogLevel.Information, "Stage: ExpectedFailure");
    }
    
    [Fact]
    public async Task Handle_UnexpectedFailure_Test()
    {
        // Arrange
        var message = new TestMessage();
        var unexpectedException = new InvalidOperationException("Unexpected");
        
        var logger = new Mock<ILogger<LoggingDecorator<TestMessage, TestResult>>>();
        var decoratedHandler = new Mock<IMessageHandler<TestMessage, TestResult>>();
        decoratedHandler.Setup(h => h.Handle(message)).ThrowsAsync(unexpectedException);
        
        var decorator = new LoggingDecorator<TestMessage, TestResult>(
            logger.Object, decoratedHandler.Object);
        
        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => decorator.Handle(message));
        
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