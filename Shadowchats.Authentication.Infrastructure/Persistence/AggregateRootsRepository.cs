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
using Shadowchats.Authentication.Core.Domain.Base;

namespace Shadowchats.Authentication.Infrastructure.Persistence;

internal class AggregateRootsRepository : IAggregateRootsRepository
{
    public AggregateRootsRepository(AuthenticationDbContext dbContext) => _dbContext = dbContext;

    public Task<T?> Find<T>(Expression<Func<T, bool>> predicate) where T : AggregateRoot<T> =>
        _dbContext.Set<T>().FirstOrDefaultAsync(predicate);

    public Task<List<T>> FindAll<T>(Expression<Func<T, bool>> predicate) where T : AggregateRoot<T> =>
        _dbContext.Set<T>().Where(predicate).ToListAsync();

    public Task Add<T>(T aggregateRoot) where T : AggregateRoot<T> =>
        _dbContext.Set<T>().AddAsync(aggregateRoot).AsTask();

    public Task<bool> Exists<T>(Expression<Func<T, bool>> predicate) where T : AggregateRoot<T> =>
        _dbContext.Set<T>().AnyAsync(predicate);
    
    private readonly AuthenticationDbContext _dbContext;
}