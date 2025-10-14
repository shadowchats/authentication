// Shadowchats — Copyright (C) 2025
// Dorovskoy Alexey Vasilievich (One290 / 0ne290) <lenya.dorovskoy@mail.ru>
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version. See the LICENSE file for details.
// For full copyright and authorship information, see the COPYRIGHT file.

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Shadowchats.Authentication.Infrastructure.Persistence;

public class AuthenticationDbContextFactory : IDesignTimeDbContextFactory<AuthenticationDbContext.ReadWrite>
{
    public AuthenticationDbContext.ReadWrite CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<AuthenticationDbContext.ReadWrite>();
        
        optionsBuilder.UseNpgsql(args[0]);

        return new AuthenticationDbContext.ReadWrite(optionsBuilder.Options);
    }
}