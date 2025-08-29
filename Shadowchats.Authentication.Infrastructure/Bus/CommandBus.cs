// Shadowchats — Copyright (C) 2025
// Dorovskoy Alexey Vasilievich (One290 / 0ne290) <lenya.dorovskoy@mail.ru>
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version. See the LICENSE file for details.
// For full copyright and authorship information, see the COPYRIGHT file.

using Microsoft.Extensions.DependencyInjection;
using Shadowchats.Authentication.Core.Application.Interfaces;

namespace Shadowchats.Authentication.Infrastructure.Bus;

public class CommandBus : ICommandBus
{
    public CommandBus(IServiceProvider services)
    {
        _services = services;
    }

    public Task<TResult> Execute<TCommand, TResult>(TCommand command) where TCommand : ICommand<TResult>
    {
        var handler = _services.GetRequiredService<ICommandHandler<TCommand, TResult>>();
        return handler.Handle(command);
    }
    
    private readonly IServiceProvider _services;
}