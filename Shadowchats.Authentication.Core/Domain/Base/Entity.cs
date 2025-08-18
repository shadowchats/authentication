using System.ComponentModel;
using Shadowchats.Authentication.Core.Domain.Exceptions;

namespace Shadowchats.Authentication.Core.Domain.Base;

public abstract class Entity<TEntity> where TEntity : Entity<TEntity>
{
    protected Entity(Guid guid)
    {
        if (guid == Guid.Empty)
            throw new InvariantViolationException("Guid is empty.");
        
        Guid = guid;
    }

    public void AddDomainEvent(IDomainEvent domainEventItem) => _domainEvents.Add(domainEventItem);

    public void RemoveDomainEvent(IDomainEvent domainEventItem) => _domainEvents.Remove(domainEventItem);

    public void ClearDomainEvents() => _domainEvents.Clear();

    public sealed override bool Equals(object? obj) => obj is TEntity entity && Equals(entity);

    public bool Equals(TEntity other) => Guid == other.Guid;

    public sealed override int GetHashCode() => Guid.GetHashCode();

    [Obsolete("Don't use == operator, use Equals or ReferenceEquals methods instead", true)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static bool operator ==(Entity<TEntity> _, Entity<TEntity> __) => throw new BugException("== operator is not allowed");

    [Obsolete("Don't use != operator, use !Equals or !ReferenceEquals methods instead", true)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static bool operator !=(Entity<TEntity> _, Entity<TEntity> __) => throw new BugException("!= operator is not allowed");

    public Guid Guid { get; }
    
    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    private readonly List<IDomainEvent> _domainEvents = [];
}