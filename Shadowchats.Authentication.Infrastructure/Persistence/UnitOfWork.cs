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

public class UnitOfWork(AuthenticationDbContext dbContext) : IUnitOfWork
{
    public async Task Begin()
    {
        if (_transaction != null)
            throw new BugException("Transaction has already begun.");
        
        _transaction = await dbContext.Database.BeginTransactionAsync();
    }

    public async Task Commit()
    {
        if (_transaction == null)
            throw new BugException("Transaction has not yet begun or has already finished.");

        await _transaction.CommitAsync();
        await _transaction.DisposeAsync();
        _transaction = null;
        
        dbContext.ChangeTracker.Clear();
    }

    public async Task Rollback()
    {
        if (_transaction == null)
            throw new BugException("Transaction has not yet begun or has already finished.");
        
        await _transaction.RollbackAsync();
        await _transaction.DisposeAsync();
        _transaction = null;
        
        dbContext.ChangeTracker.Clear();
    }

    private IDbContextTransaction? _transaction;
}