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
using Shadowchats.Authentication.Infrastructure.Tests.Bus.Shadowchats.Authentication.Infrastructure.Tests.Bus.Fakes;

namespace Shadowchats.Authentication.Infrastructure.Tests.Bus;

public class BusTests
{
    [Fact]
    public async Task Execute_Test()
    {
        // Arrange
        var expectedResult = new TestResult { Value = "test" };
        var message = new TestMessage { Data = "input" };
        
        var handler = new Mock<IMessageHandler<TestMessage, TestResult>>();
        handler.Setup(h => h.Handle(message)).ReturnsAsync(expectedResult);
        
        var services = new ServiceCollection();
        services.AddSingleton(handler.Object);
        var serviceProvider = services.BuildServiceProvider();
        
        var commandBus = new Infrastructure.Bus.Bus(serviceProvider);
        
        // Act
        var result = await commandBus.Execute<TestMessage, TestResult>(message);
        
        // Assert
        Assert.Equal(expectedResult, result);
        handler.Verify(h => h.Handle(message), Times.Once);
    }
    
    [Fact]
    public async Task Execute_InvalidOperationException_Test()
    {
        // Arrange
        var message = new TestMessage { Data = "input" };
        var serviceProvider = new ServiceCollection().BuildServiceProvider();
        var commandBus = new Infrastructure.Bus.Bus(serviceProvider);
        
        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => commandBus.Execute<TestMessage, TestResult>(message));
    }
}