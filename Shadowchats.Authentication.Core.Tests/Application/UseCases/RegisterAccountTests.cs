// Shadowchats — Copyright (C) 2025
// Dorovskoy Alexey Vasilievich (One290 / 0ne290) <lenya.dorovskoy@mail.ru>
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version. See the LICENSE file for details.
// For full copyright and authorship information, see the COPYRIGHT file.

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