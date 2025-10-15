// Shadowchats — Copyright (C) 2025
// Dorovskoy Alexey Vasilievich (One290 / 0ne290) <lenya.dorovskoy@mail.ru>
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version. See the LICENSE file for details.
// For full copyright and authorship information, see the COPYRIGHT file.

using Microsoft.Extensions.DependencyInjection;
using Shadowchats.Authentication.Core.Application.Base;
using Shadowchats.Authentication.Core.Application.Interfaces;
using Shadowchats.Authentication.Core.Domain.Exceptions;
using Shadowchats.Authentication.Infrastructure.Persistence;
using Shadowchats.Authentication.Infrastructure.Persistence.AuthenticationDbContext;

namespace Shadowchats.Authentication.Infrastructure.Bus.Decorators;

public class UnitOfWorkDecorator<TMessage, TResult> : IMessageHandler<TMessage, TResult> where TMessage : IMessage<TResult>
{
    public UnitOfWorkDecorator(IUnitOfWork unitOfWork, IServiceProvider services, IMessageHandler<TMessage, TResult> decorated)
    {
        _unitOfWork = unitOfWork;
        _services = services;
        _decorated = decorated;
    }

    public async Task<TResult> Handle(TMessage message)
    {
        var (dbContextKeyType, transactionMode) = message switch
        {
            IQuery<TResult>   => (typeof(ReadOnly), IUnitOfWork.TransactionMode.None),
            ICommand<TResult> => (typeof(ReadWrite), IUnitOfWork.TransactionMode.WithReadCommitted),
            _ => throw new BugException("Unhandled message type.")
        };

        await _unitOfWork.Begin((IAuthenticationDbContext)_services.GetRequiredService(dbContextKeyType),
            transactionMode);

        try
        {
            var result = await _decorated.Handle(message);

            await _unitOfWork.End(IUnitOfWork.Outcome.Success);
            
            return result;
        }
        catch
        {
            await _unitOfWork.End(IUnitOfWork.Outcome.Failure);
            
            throw;
        }
    }

    private readonly IUnitOfWork _unitOfWork;
    
    private readonly IServiceProvider _services;
    
    private readonly IMessageHandler<TMessage, TResult> _decorated;
}