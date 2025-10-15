﻿// Shadowchats — Copyright (C) 2025
// Dorovskoy Alexey Vasilievich (One290 / 0ne290) <lenya.dorovskoy@mail.ru>
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version. See the LICENSE file for details.
// For full copyright and authorship information, see the COPYRIGHT file.

using System.ComponentModel;
using JetBrains.Annotations;
using Shadowchats.Authentication.Core.Domain.Exceptions;

namespace Shadowchats.Authentication.Core.Domain.Base;

public abstract class Entity<TEntity> where TEntity : Entity<TEntity>
{
    [UsedImplicitly]
    protected Entity()
    {
        _domainEvents = [];
        DomainEvents = _domainEvents.AsReadOnly();
    }
    
    protected Entity(Guid guid) : this()
    {
        Guid = guid;
    }

    public void AddDomainEvent(IDomainEvent domainEventItem) => _domainEvents.Add(domainEventItem);

    public void RemoveDomainEvent(IDomainEvent domainEventItem) => _domainEvents.Remove(domainEventItem);

    public void ClearDomainEvents() => _domainEvents.Clear();

    public sealed override bool Equals(object? obj) => obj is TEntity entity && Equals(entity);

    public bool Equals(TEntity other) => Guid == other.Guid; 
    
    // ReSharper disable once NonReadonlyMemberInGetHashCode
    public sealed override int GetHashCode() => Guid.GetHashCode();

    [Obsolete("Don't use == operator, use Equals or ReferenceEquals methods instead", true)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static bool operator ==(Entity<TEntity> _, Entity<TEntity> __) => throw new BugException("== operator is not allowed");

    [Obsolete("Don't use != operator, use !Equals or !ReferenceEquals methods instead", true)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static bool operator !=(Entity<TEntity> _, Entity<TEntity> __) => throw new BugException("!= operator is not allowed");

    public Guid Guid { get; private set; }
    
    public IReadOnlyList<IDomainEvent> DomainEvents { get; }

    private readonly List<IDomainEvent> _domainEvents;
}