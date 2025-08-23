using Microsoft.Extensions.DependencyInjection;
using Shadowchats.Authentication.Core.Application.Interfaces;

namespace Shadowchats.Authentication.Infrastructure.Bus;

internal class CommandBus : ICommandBus
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