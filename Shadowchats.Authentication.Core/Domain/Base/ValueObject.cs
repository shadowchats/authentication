using System.ComponentModel;
using Shadowchats.Authentication.Core.Domain.Exceptions;

namespace Shadowchats.Authentication.Core.Domain.Base;

public abstract class ValueObject
{
    protected abstract IEnumerable<object> GetEqualityComponents();
    
    public override bool Equals(object? obj) => obj is ValueObject valueObject && Equals(valueObject);

    public bool Equals(ValueObject other) => GetEqualityComponents().SequenceEqual(other.GetEqualityComponents());

    public override int GetHashCode()
    {
        var hashCode = new HashCode();
        foreach (var component in GetEqualityComponents())
            hashCode.Add(component);
        return hashCode.ToHashCode();
    }

    [Obsolete("Don't use == operator, use Equals method instead", true)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static bool operator ==(ValueObject _, ValueObject __) => throw new BugException("== operator is not allowed");

    [Obsolete("Don't use != operator, use !Equals method instead", true)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static bool operator !=(ValueObject _, ValueObject __) => throw new BugException("!= operator is not allowed");
}