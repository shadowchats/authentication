namespace Shadowchats.Authentication.Core.Application.Interfaces;

public interface ICommandBus
{
    Task<TResult> Execute<TCommand, TResult>(TCommand command) where TCommand : ICommand<TResult>;
}