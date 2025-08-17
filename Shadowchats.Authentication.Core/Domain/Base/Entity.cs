using System.ComponentModel;
using Shadowchats.Authentication.Core.Domain.Exceptions;

namespace Shadowchats.Authentication.Core.Domain.Base;

public abstract class Entity
{
    protected Entity(Guid guid) => Guid = guid;

    public void AddDomainEvent(IEvent eventItem) => _domainEvents.Add(eventItem);

    public void RemoveDomainEvent(IEvent eventItem) => _domainEvents.Remove(eventItem);

    public void ClearDomainEvents() => _domainEvents.Clear();

    public override bool Equals(object? obj) => obj is Entity entity && Equals(entity);

    public bool Equals(Entity other) => Guid == other.Guid;

    public override int GetHashCode() => Guid.GetHashCode();

    [Obsolete("Don't use == operator, use Equals or ReferenceEquals methods instead", true)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static bool operator ==(Entity _, Entity __) => throw new BugException("== operator is not allowed");

    [Obsolete("Don't use != operator, use !Equals or !ReferenceEquals methods instead", true)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static bool operator !=(Entity _, Entity __) => throw new BugException("!= operator is not allowed");

    public Guid Guid { get; }
    
    public IReadOnlyCollection<IEvent> DomainEvents => _domainEvents.AsReadOnly();

    private readonly List<IEvent> _domainEvents = [];
}