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
using Shadowchats.Authentication.Infrastructure.Persistence.Interfaces;

namespace Shadowchats.Authentication.Infrastructure.Persistence.Repositories;

public class AccountRepository : IAggregateRootRepository<Account>
{
    public AccountRepository(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public Task<Account?> Find(Expression<Func<Account, bool>> predicate) =>
        _unitOfWork.DbContext.Accounts.FirstOrDefaultAsync(predicate);

    public Task<List<Account>> FindAll(Expression<Func<Account, bool>> predicate) =>
        _unitOfWork.DbContext.Accounts.Where(predicate).ToListAsync();

    public Task Add(Account aggregateRoot) => _unitOfWork.DbContext.Accounts.AddAsync(aggregateRoot).AsTask();

    private readonly IUnitOfWork _unitOfWork;
}