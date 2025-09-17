// Shadowchats — Copyright (C) 2025
// Dorovskoy Alexey Vasilievich (One290 / 0ne290) <lenya.dorovskoy@mail.ru>
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version. See the LICENSE file for details.
// For full copyright and authorship information, see the COPYRIGHT file.

using Microsoft.EntityFrameworkCore.Storage;
using Shadowchats.Authentication.Core.Domain.Exceptions;

namespace Shadowchats.Authentication.Infrastructure.Persistence;

public class UnitOfWork : IUnitOfWork
{
    public async Task Begin(AuthenticationDbContext dbContext, IUnitOfWork.TransactionMode transactionMode)
    {
        if (_dbContext is not null)
            throw new BugException("Is already begun.");
        
        _dbContext = dbContext;

        _transaction = transactionMode switch
        {
            IUnitOfWork.TransactionMode.None => null,
            IUnitOfWork.TransactionMode.WithReadCommitted => await _dbContext.Database.BeginTransactionAsync(),
            _ => throw new BugException("Transaction mode is not supported.")
        };
    }

    public async Task End(IUnitOfWork.Outcome outcome)
    {
        if (_dbContext is null)
            throw new BugException("Is not yet begun or is already ended.");
        
        _dbContext = null;

        if (_transaction is not null)
        {
            try
            {
                switch (outcome)
                {
                    case IUnitOfWork.Outcome.Success:
                        await _transaction.CommitAsync();
                        break;
                    case IUnitOfWork.Outcome.Failure:
                        await _transaction.RollbackAsync();
                        break;
                    default:
                        throw new BugException("Outcome is not supported.");
                }
            }
            finally
            {
                await _transaction.DisposeAsync();

                _transaction = null;
            }
        }
    }

    public AuthenticationDbContext DbContext =>
        _dbContext ?? throw new BugException("Is not yet begun or is already ended.");

    private AuthenticationDbContext? _dbContext;

    private IDbContextTransaction? _transaction;
}