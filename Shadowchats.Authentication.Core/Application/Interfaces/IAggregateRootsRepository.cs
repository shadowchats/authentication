// Shadowchats — Copyright (C) 2025
// Dorovskoy Alexey Vasilievich (One290 / 0ne290) <lenya.dorovskoy@mail.ru>
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version. See the LICENSE file for details.
// For full copyright and authorship information, see the COPYRIGHT file.

using System.Linq.Expressions;
using Shadowchats.Authentication.Core.Domain.Base;

namespace Shadowchats.Authentication.Core.Application.Interfaces;

internal interface IAggregateRootsRepository
{
    Task<T?> Find<T>(Expression<Func<T, bool>> predicate) where T : AggregateRoot<T>;
    
    Task<List<T>> FindAll<T>(Expression<Func<T, bool>> predicate) where T : AggregateRoot<T>;

    Task Add<T>(T aggregateRoot) where T : AggregateRoot<T>;

    Task<bool> Exists<T>(Expression<Func<T, bool>> predicate) where T : AggregateRoot<T>;
}