﻿// Shadowchats — Copyright (C) 2025
// Dorovskoy Alexey Vasilievich (One290 / 0ne290) <lenya.dorovskoy@mail.ru>
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version. See the LICENSE file for details.
// For full copyright and authorship information, see the COPYRIGHT file.

using JetBrains.Annotations;
using Shadowchats.Authentication.Core.Domain.Base;
using Shadowchats.Authentication.Core.Domain.Interfaces;
using Shadowchats.Authentication.Core.Domain.ValueObjects;

namespace Shadowchats.Authentication.Core.Domain.Aggregates;

public class Account : AggregateRoot<Account>
{
    public static Account Create(IGuidGenerator guidGenerator, Credentials credentials) => new(guidGenerator.Generate(), credentials); 

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    [UsedImplicitly]
    private Account() { } 
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    
    private Account(Guid guid, Credentials credentials) : base(guid) => Credentials = credentials;

    public Credentials Credentials { get; private set; }
}
