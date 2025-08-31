using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Shadowchats.Authentication.Core.Application.Interfaces;
using Shadowchats.Authentication.Core.Domain.Aggregates;

namespace Shadowchats.Authentication.Infrastructure.Persistence;

public class SessionRepository(AuthenticationDbContext dbContext) : IAggregateRootRepository<Session>
{
    public Task<Session?> Find(Expression<Func<Session, bool>> predicate) =>
        dbContext.Sessions.FirstOrDefaultAsync(predicate);

    public Task<List<Session>> FindAll(Expression<Func<Session, bool>> predicate) =>
        dbContext.Sessions.Where(predicate).ToListAsync();

    public async Task Add(Session aggregateRoot)
    {
        await dbContext.Sessions.AddAsync(aggregateRoot);
        await dbContext.SaveChangesAsync();
    }
}