// Shadowchats — Copyright (C) 2025
// Dorovskoy Alexey Vasilievich (One290 / 0ne290) <lenya.dorovskoy@mail.ru>
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version. See the LICENSE file for details.
// For full copyright and authorship information, see the COPYRIGHT file.

using Shadowchats.Authentication.Core.Application.Base;
using Shadowchats.Authentication.Core.Application.Exceptions;
using Shadowchats.Authentication.Core.Application.Interfaces;
using Shadowchats.Authentication.Core.Domain.Aggregates;
using Shadowchats.Authentication.Core.Domain.Exceptions;
using Shadowchats.Authentication.Core.Domain.Interfaces;
using Shadowchats.Authentication.Core.Domain.ValueObjects;

namespace Shadowchats.Authentication.Core.Application.UseCases.RegisterAccount;

public record Command : ICommand<NoResult>
{
    public required string Login { get; init; }

    public required string Password { get; init; }
}

public class Handler : IMessageHandler<Command, NoResult>
{
    public Handler(IAggregateRootRepository<Account> accountRepository,
        IPersistenceContext persistenceContext, IPasswordHasher passwordHasher, IGuidGenerator guidGenerator)
    {
        _accountRepository = accountRepository;
        _persistenceContext = persistenceContext;
        _passwordHasher = passwordHasher;
        _guidGenerator = guidGenerator;
    }

    public async Task<NoResult> Handle(Command command)
    {
        var credentials = Credentials.Create(_passwordHasher, command.Login, command.Password);
        var account = Account.Create(_guidGenerator, credentials);

        await _accountRepository.Add(account);
        try
        {
            await _persistenceContext.SaveChanges();
        }
        catch (EntityAlreadyExistsException<Account, string> ex) when (ex.IsConflictOn(a => a.Credentials.Login))
        {
            throw new InvariantViolationException("Login and/or password is invalid.");
        }

        return NoResult.Value;
    }

    private readonly IAggregateRootRepository<Account> _accountRepository;

    private readonly IPersistenceContext _persistenceContext;

    private readonly IPasswordHasher _passwordHasher;

    private readonly IGuidGenerator _guidGenerator;
}