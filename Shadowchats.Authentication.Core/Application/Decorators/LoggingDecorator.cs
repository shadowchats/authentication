using System.Diagnostics;
using Microsoft.Extensions.Logging;
using Shadowchats.Authentication.Core.Application.Interfaces;
using Shadowchats.Authentication.Core.Domain.Exceptions;

namespace Shadowchats.Authentication.Core.Application.Decorators;

public class LoggingDecorator<TCommand, TResult> : ICommandHandler<TCommand, TResult>
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
        var traceId = Activity.Current?.TraceId.ToString() ?? "N/A";

        _logger.LogInformation(
            "TraceId: {TraceId}. Stage: {Stage}. CommandName: {CommandName}. HandlerName: {HandlerName}. Payload: {@Command}", 
            traceId, "Start", _commandName, _handlerName, command);

        try
        {
            var result = await _decorated.Handle(command);

            _logger.LogInformation(
                "TraceId: {TraceId}. Stage: {Stage}. Result: {@Result}", traceId, "Successful end without exception",
                result);

            return result;
        }
        catch (BaseException expectedException)
        {
            _logger.LogInformation(
                "TraceId: {TraceId}. Stage: {Stage}. ExpectedException: {@ExpectedException}", traceId, "Successful end with expected exception",
                expectedException);

            throw;
        }
        catch (Exception unexcpectedException)
        {
            _logger.LogError(
                "TraceId: {TraceId}. Stage: {Stage}. ExpectedException: {@UnexpectedException}", traceId, "Failed end with unexpected exception",
                unexcpectedException);

            throw;
        }
    }
    
    private readonly ILogger<LoggingDecorator<TCommand, TResult>> _logger;
    
    private readonly ICommandHandler<TCommand, TResult> _decorated;

    private readonly string _commandName;

    private readonly string _handlerName;
}
