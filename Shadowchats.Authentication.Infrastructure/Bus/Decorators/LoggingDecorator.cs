// Shadowchats — Copyright (C) 2025
// Dorovskoy Alexey Vasilievich (One290 / 0ne290) <lenya.dorovskoy@mail.ru>
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version. See the LICENSE file for details.
// For full copyright and authorship information, see the COPYRIGHT file.

using Microsoft.Extensions.Logging;
using Shadowchats.Authentication.Core.Application.Interfaces;
using Shadowchats.Authentication.Core.Domain.Exceptions;

namespace Shadowchats.Authentication.Infrastructure.Bus.Decorators;

internal class LoggingDecorator<TCommand, TResult> : ICommandHandler<TCommand, TResult>
    where TCommand : ICommand<TResult>
{
    public LoggingDecorator(
        ILogger<LoggingDecorator<TCommand, TResult>> logger,
        ICommandHandler<TCommand, TResult> decorated)
    {
        _logger = logger;
        _decorated = decorated;
        _commandName = typeof(TCommand).Name;
        _handlerName = decorated.GetType().Name;
    }

    public async Task<TResult> Handle(TCommand command)
    {
        _logger.LogInformation(
            "Stage: {Stage}. CommandName: {CommandName}. HandlerName: {HandlerName}. Payload: {@Command}", 
            "Start", _commandName, _handlerName, command);

        try
        {
            var result = await _decorated.Handle(command);

            _logger.LogInformation(
                "Stage: {Stage}. CommandName: {CommandName}. HandlerName: {HandlerName}. Payload: {@Command}. Result: {@Result}",
                "Success", _commandName, _handlerName, command, result);

            return result;
        }
        catch (BaseException expectedException)
        {
            _logger.LogInformation(expectedException,
                "Stage: {Stage}. CommandName: {CommandName}. HandlerName: {HandlerName}. Payload: {@Command}",
                "ExpectedFailure", _commandName, _handlerName, command);

            throw;
        }
        catch (Exception unexpectedException)
        {
            _logger.LogError(unexpectedException,
                "Stage: {Stage}. CommandName: {CommandName}. HandlerName: {HandlerName}. Payload: {@Command}",
                "UnexpectedFailure", _commandName, _handlerName, command);

            throw;
        }
    }
    
    private readonly ILogger<LoggingDecorator<TCommand, TResult>> _logger;
    
    private readonly ICommandHandler<TCommand, TResult> _decorated;

    private readonly string _commandName;

    private readonly string _handlerName;
}
