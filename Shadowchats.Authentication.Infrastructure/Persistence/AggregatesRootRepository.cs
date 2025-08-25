using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Shadowchats.Authentication.Core.Application.Interfaces;
using Shadowchats.Authentication.Core.Domain.Base;

namespace Shadowchats.Authentication.Infrastructure.Persistence;

internal class AggregatesRootRepository : IAggregateRootsRepository
{
    public AggregatesRootRepository(AuthenticationDbContext dbContext) => _dbContext = dbContext;

    public Task<T?> Find<T>(Expression<Func<T, bool>> predicate) where T : AggregateRoot<T> =>
        _dbContext.Set<T>().FirstOrDefaultAsync(predicate);

    public Task<List<T>> FindAll<T>(Expression<Func<T, bool>> predicate) where T : AggregateRoot<T> =>
        _dbContext.Set<T>().Where(predicate).ToListAsync();

    public Task Add<T>(T aggregateRoot) where T : AggregateRoot<T> =>
        _dbContext.Set<T>().AddAsync(aggregateRoot).AsTask();

    public Task<bool> Exists<T>(Expression<Func<T, bool>> predicate) where T : AggregateRoot<T> =>
        _dbContext.Set<T>().AnyAsync(predicate);
    
    private readonly AuthenticationDbContext _dbContext;
}