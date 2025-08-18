using System.ComponentModel;
using Shadowchats.Authentication.Core.Domain.Exceptions;

namespace Shadowchats.Authentication.Core.Domain.Base;

public abstract class ValueObject<TValueObject> where TValueObject : ValueObject<TValueObject>
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