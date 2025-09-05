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

public class LoggingDecorator<TMessage, TResult>(
    ILogger<LoggingDecorator<TMessage, TResult>> logger,
    IMessageHandler<TMessage, TResult> decorated)
    : IMessageHandler<TMessage, TResult>
    where TMessage : IMessage<TResult>
{
    public async Task<TResult> Handle(TMessage message)
    {
        logger.LogInformation(
            "Stage: {Stage}. MessageName: {MessageName}. HandlerName: {HandlerName}. Payload: {@Message}", 
            "Start", _messageName, _handlerName, message);

        try
        {
            var result = await decorated.Handle(message);

            logger.LogInformation(
                "Stage: {Stage}. MessageName: {MessageName}. HandlerName: {HandlerName}. Payload: {@Message}. Result: {@Result}",
                "Success", _messageName, _handlerName, message, result);

            return result;
        }
        catch (BaseException expectedException) when (expectedException is not BugException)
        {
            logger.LogInformation(expectedException,
                "Stage: {Stage}. MessageName: {MessageName}. HandlerName: {HandlerName}. Payload: {@Message}",
                "ExpectedFailure", _messageName, _handlerName, message);

            throw;
        }
        catch (Exception unexpectedException)
        {
            logger.LogError(unexpectedException,
                "Stage: {Stage}. CommandName: {CommandName}. HandlerName: {HandlerName}. Payload: {@Command}",
                "UnexpectedFailure", _messageName, _handlerName, message);

            throw;
        }
    }

    private readonly string _messageName = typeof(TMessage).Name;

    private readonly string _handlerName = decorated.GetType().Name;
}
