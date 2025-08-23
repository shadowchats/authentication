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
