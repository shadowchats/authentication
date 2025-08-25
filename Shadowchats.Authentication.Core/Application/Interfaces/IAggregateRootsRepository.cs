using System.Linq.Expressions;
using Shadowchats.Authentication.Core.Domain.Base;

namespace Shadowchats.Authentication.Core.Application.Interfaces;

internal interface IAggregateRootsRepository
{
    Task<T?> Find<T>(Expression<Func<T, bool>> predicate) where T : AggregateRoot<T>;
    
    Task<List<T>> FindAll<T>(Expression<Func<T, bool>> predicate) where T : AggregateRoot<T>;

    Task Add<T>(T aggregateRoot) where T : AggregateRoot<T>;

    Task<bool> Exists<T>(Expression<Func<T, bool>> predicate) where T : AggregateRoot<T>;
}