// Shadowchats — Copyright (C) 2025
// Dorovskoy Alexey Vasilievich (One290 / 0ne290) <lenya.dorovskoy@mail.ru>
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version. See the LICENSE file for details.
// For full copyright and authorship information, see the COPYRIGHT file.

using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using Shadowchats.Authentication.Core.Application.Exceptions;
using Shadowchats.Authentication.Core.Application.Interfaces;
using Shadowchats.Authentication.Core.Domain.Aggregates;

namespace Shadowchats.Authentication.Infrastructure.Persistence;

public class AccountRepository(AuthenticationDbContext dbContext) : IAggregateRootRepository<Account>
{
    public Task<Account?> Find(Expression<Func<Account, bool>> predicate) =>
        dbContext.Accounts.FirstOrDefaultAsync(predicate);

    public Task<List<Account>> FindAll(Expression<Func<Account, bool>> predicate) =>
        dbContext.Accounts.Where(predicate).ToListAsync();

    public async Task Add(Account aggregateRoot)
    {
        try
        {
            await dbContext.Accounts.AddAsync(aggregateRoot);
            await dbContext.SaveChangesAsync();
        }
        catch (DbUpdateException ex) when (ex.InnerException is PostgresException
                                           {
                                               SqlState: "23505", ConstraintName: "IX_Accounts_Login"
                                           })
        {
            throw new EntityAlreadyExistsException<Account, string>(a => a.Credentials.Login);
        }
    }
}