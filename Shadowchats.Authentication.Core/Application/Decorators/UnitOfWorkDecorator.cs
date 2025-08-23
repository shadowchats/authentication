using Shadowchats.Authentication.Core.Application.Interfaces;

namespace Shadowchats.Authentication.Core.Application.Decorators;

public class UnitOfWorkDecorator<TCommand, TResult> : ICommandHandler<TCommand, TResult>
    where TCommand : ICommand<TResult>
{
    public UnitOfWorkDecorator(IUnitOfWork unitOfWork, ICommandHandler<TCommand, TResult> decorated)
    {
        _unitOfWork = unitOfWork;
        _decorated = decorated;
    }

    public async Task<TResult> Handle(TCommand command)
    {
        await _unitOfWork.Begin();

        try
        {
            var result = await _decorated.Handle(command);

            await _unitOfWork.Commit();
            
            return result;
        }
        catch
        {
            await _unitOfWork.Rollback();
            
            throw;
        }
    }
    
    private readonly IUnitOfWork _unitOfWork;

    private readonly ICommandHandler<TCommand, TResult> _decorated;
}