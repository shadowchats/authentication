using Moq;
using Shadowchats.Authentication.Core.Application.Interfaces;
using Shadowchats.Authentication.Infrastructure.Bus.Decorators;
using Shadowchats.Authentication.Infrastructure.Persistence;
using Shadowchats.Authentication.Infrastructure.Tests.Bus.Shadowchats.Authentication.Infrastructure.Tests.Bus.Fakes;

namespace Shadowchats.Authentication.Infrastructure.Tests.Bus.Decorators;

public class UnitOfWorkDecoratorTests
{
    [Fact]
    public async Task Handle_Success_CommitsTransaction()
    {
        // Arrange
        var command = new TestCommand();
        var expectedResult = new TestResult();
        
        var unitOfWork = new Mock<IUnitOfWork>();
        var decoratedHandler = new Mock<ICommandHandler<TestCommand, TestResult>>();
        decoratedHandler.Setup(h => h.Handle(command)).ReturnsAsync(expectedResult);
        
        var decorator = new UnitOfWorkDecorator<TestCommand, TestResult>(
            unitOfWork.Object, decoratedHandler.Object);
        
        // Act
        var result = await decorator.Handle(command);
        
        // Assert
        Assert.Equal(expectedResult, result);
        
        var sequence = new MockSequence();
        unitOfWork.InSequence(sequence).Setup(uow => uow.Begin());
        decoratedHandler.InSequence(sequence).Setup(h => h.Handle(command));
        unitOfWork.InSequence(sequence).Setup(uow => uow.Commit());
        
        unitOfWork.Verify(uow => uow.Begin(), Times.Once);
        unitOfWork.Verify(uow => uow.Commit(), Times.Once);
        unitOfWork.Verify(uow => uow.Rollback(), Times.Never);
    }
    
    [Fact]
    public async Task Handle_Exception_RollsBackTransaction()
    {
        // Arrange
        var command = new TestCommand();
        var exception = new InvalidOperationException("Test error");
        
        var unitOfWork = new Mock<IUnitOfWork>();
        var decoratedHandler = new Mock<ICommandHandler<TestCommand, TestResult>>();
        decoratedHandler.Setup(h => h.Handle(command)).ThrowsAsync(exception);
        
        var decorator = new UnitOfWorkDecorator<TestCommand, TestResult>(
            unitOfWork.Object, decoratedHandler.Object);
        
        // Act & Assert
        var thrownException = await Assert.ThrowsAsync<InvalidOperationException>(
            () => decorator.Handle(command));
        
        Assert.Same(exception, thrownException);
        
        unitOfWork.Verify(uow => uow.Begin(), Times.Once);
        unitOfWork.Verify(uow => uow.Rollback(), Times.Once);
        unitOfWork.Verify(uow => uow.Commit(), Times.Never);
    }
}