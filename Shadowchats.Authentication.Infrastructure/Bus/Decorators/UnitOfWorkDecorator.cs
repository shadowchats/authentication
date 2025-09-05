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

public class UnitOfWorkDecorator<TMessage, TResult>(
    IUnitOfWork unitOfWork,
    IMessageHandler<TMessage, TResult> decorated)
    : IMessageHandler<TMessage, TResult>
    where TMessage : IMessage<TResult>
{
    public async Task<TResult> Handle(TMessage message)
    {
        await unitOfWork.Begin();

        try
        {
            var result = await decorated.Handle(message);

            await unitOfWork.Commit();
            
            return result;
        }
        catch
        {
            await unitOfWork.Rollback();
            
            throw;
        }
    }
}