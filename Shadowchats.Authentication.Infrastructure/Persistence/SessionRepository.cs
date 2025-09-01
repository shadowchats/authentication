// Shadowchats — Copyright (C) 2025
// Dorovskoy Alexey Vasilievich (One290 / 0ne290) <lenya.dorovskoy@mail.ru>
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version. See the LICENSE file for details.
// For full copyright and authorship information, see the COPYRIGHT file.

using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Shadowchats.Authentication.Core.Application.Interfaces;
using Shadowchats.Authentication.Core.Domain.Aggregates;

namespace Shadowchats.Authentication.Infrastructure.Persistence;

public class SessionRepository(AuthenticationDbContext dbContext) : IAggregateRootRepository<Session>
{
    public Task<Session?> Find(Expression<Func<Session, bool>> predicate) =>
        dbContext.Sessions.FirstOrDefaultAsync(predicate);

    public Task<List<Session>> FindAll(Expression<Func<Session, bool>> predicate) =>
        dbContext.Sessions.Where(predicate).ToListAsync();

    public async Task Add(Session aggregateRoot)
    {
        await dbContext.Sessions.AddAsync(aggregateRoot);
        await dbContext.SaveChangesAsync();
    }
}