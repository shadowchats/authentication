namespace Shadowchats.Authentication.Core.Domain.Base;

public abstract class AggregateRoot<TAggregateRoot>(Guid guid) : Entity<TAggregateRoot>(guid) where TAggregateRoot : AggregateRoot<TAggregateRoot>;