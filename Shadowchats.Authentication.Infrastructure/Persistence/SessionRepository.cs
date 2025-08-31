using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Shadowchats.Authentication.Core.Application.Interfaces;
using Shadowchats.Authentication.Core.Domain.Aggregates;

namespace Shadowchats.Authentication.Infrastructure.Persistence;

public class SessionRepository(AuthenticationDbContext dbContext) : IAggregateRootRepository<Session>
{
    public Task<Session?> Find(Expression<Func<Session, bool>> predicate) =>
        _sessions.FirstOrDefaultAsync(predicate);

    public Task<List<Session>> FindAll(Expression<Func<Session, bool>> predicate) =>
        _sessions.Where(predicate).ToListAsync();

    public Task Add(Session aggregateRoot) => _sessions.AddAsync(aggregateRoot).AsTask();
    
    private readonly DbSet<Session> _sessions = dbContext.Sesssions;
}