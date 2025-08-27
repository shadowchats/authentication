// Shadowchats — Copyright (C) 2025
// Dorovskoy Alexey Vasilievich (One290 / 0ne290) <lenya.dorovskoy@mail.ru>
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version. See the LICENSE file for details.
// For full copyright and authorship information, see the COPYRIGHT file.

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
    public async Task Execute_InvalidOperationException_Test()
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