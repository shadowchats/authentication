// Shadowchats — Copyright (C) 2025
// Dorovskoy Alexey Vasilievich (One290 / 0ne290) <lenya.dorovskoy@mail.ru>
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version. See the LICENSE file for details.
// For full copyright and authorship information, see the COPYRIGHT file.

using Shadowchats.Authentication.Core.Application.Interfaces;
using Shadowchats.Authentication.Infrastructure.Persistence;

namespace Shadowchats.Authentication.Infrastructure.Bus.Decorators;

internal class UnitOfWorkDecorator<TCommand, TResult> : ICommandHandler<TCommand, TResult>
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