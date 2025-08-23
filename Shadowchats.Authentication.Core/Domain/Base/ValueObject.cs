// Shadowchats — Copyright (C) 2025
// Dorovskoy Alexey Vasilievich (One290 / 0ne290) <lenya.dorovskoy@mail.ru>
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version. See the LICENSE file for details.
// For full copyright and authorship information, see the COPYRIGHT file.

using System.ComponentModel;
using Shadowchats.Authentication.Core.Domain.Exceptions;

namespace Shadowchats.Authentication.Core.Domain.Base;

internal abstract class ValueObject<TValueObject> where TValueObject : ValueObject<TValueObject>
{
    protected abstract IEnumerable<object> GetEqualityComponents();
    
    public sealed override bool Equals(object? obj) => obj is TValueObject valueObject && Equals(valueObject);

    public bool Equals(TValueObject other) => GetEqualityComponents().SequenceEqual(other.GetEqualityComponents());

    public sealed override int GetHashCode()
    {
        var hashCode = new HashCode();
        foreach (var component in GetEqualityComponents())
            hashCode.Add(component);
        return hashCode.ToHashCode();
    }

    [Obsolete("Don't use == operator, use Equals method instead", true)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static bool operator ==(ValueObject<TValueObject> _, ValueObject<TValueObject> __) => throw new BugException("== operator is not allowed");

    [Obsolete("Don't use != operator, use !Equals method instead", true)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static bool operator !=(ValueObject<TValueObject> _, ValueObject<TValueObject> __) => throw new BugException("!= operator is not allowed");
}