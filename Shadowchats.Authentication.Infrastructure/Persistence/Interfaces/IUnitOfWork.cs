﻿// Shadowchats — Copyright (C) 2025
// Dorovskoy Alexey Vasilievich (One290 / 0ne290) <lenya.dorovskoy@mail.ru>
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version. See the LICENSE file for details.
// For full copyright and authorship information, see the COPYRIGHT file.

namespace Shadowchats.Authentication.Infrastructure.Persistence.Interfaces;

public interface IUnitOfWork
{
    public enum Outcome { Success, Failure }
    
    public enum TransactionMode { None, WithReadCommitted }
    
    Task Begin(IAuthenticationDbContext dbContext, TransactionMode transactionMode);
    Task End(Outcome outcome);
    
    IAuthenticationDbContext DbContext { get; }
}