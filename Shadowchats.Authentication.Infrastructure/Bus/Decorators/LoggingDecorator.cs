// Shadowchats — Copyright (C) 2025
// Dorovskoy Alexey Vasilievich (One290 / 0ne290) <lenya.dorovskoy@mail.ru>
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version. See the LICENSE file for details.
// For full copyright and authorship information, see the COPYRIGHT file.

using Microsoft.Extensions.Logging;
using Shadowchats.Authentication.Core.Application.Base;
using Shadowchats.Authentication.Core.Application.Interfaces;
using Shadowchats.Authentication.Core.Domain.Exceptions;

namespace Shadowchats.Authentication.Infrastructure.Bus.Decorators;

public class LoggingDecorator<TMessage, TResult> : IMessageHandler<TMessage, TResult> where TMessage : IMessage<TResult>
{
    public LoggingDecorator(ILogger<LoggingDecorator<TMessage, TResult>> logger, IMessageHandler<TMessage, TResult> decorated)
    {
        _logger = logger;
        _decorated = decorated;
        _messageName = typeof(TMessage).Name;
        _handlerName = decorated.GetType().Name;
    }

    public async Task<TResult> Handle(TMessage message)
    {
        _logger.LogInformation(
            "Stage: {Stage}. MessageName: {MessageName}. HandlerName: {HandlerName}. Payload: {@Message}", 
            "Start", _messageName, _handlerName, message);

        try
        {
            var result = await _decorated.Handle(message);

            _logger.LogInformation(
                "Stage: {Stage}. MessageName: {MessageName}. HandlerName: {HandlerName}. Payload: {@Message}. Result: {@Result}",
                "Success", _messageName, _handlerName, message, result);

            return result;
        }
        catch (BaseException expectedException) when (expectedException is not BugException)
        {
            _logger.LogInformation(expectedException,
                "Stage: {Stage}. MessageName: {MessageName}. HandlerName: {HandlerName}. Payload: {@Message}",
                "ExpectedFailure", _messageName, _handlerName, message);

            throw;
        }
        catch (Exception unexpectedException)
        {
            _logger.LogError(unexpectedException,
                "Stage: {Stage}. CommandName: {CommandName}. HandlerName: {HandlerName}. Payload: {@Command}",
                "UnexpectedFailure", _messageName, _handlerName, message);

            throw;
        }
    }

    private readonly ILogger<LoggingDecorator<TMessage, TResult>> _logger;
    
    private readonly IMessageHandler<TMessage, TResult> _decorated;

    private readonly string _messageName;

    private readonly string _handlerName;
}
