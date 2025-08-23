using Microsoft.EntityFrameworkCore;
using Shadowchats.Authentication.Core.Domain.Aggregates;

namespace Shadowchats.Authentication.Infrastructure.Application;

public class SessionRepository
{
    public SessionRepository(UnitOfWork.UserDbContext dbContext) => _dbContext = dbContext;
    
    public async Task<Session?> GetByRefreshToken(string refreshToken) =>
        await _dbContext.Sesssions.FirstOrDefaultAsync(s => s.RefreshToken == refreshToken);

    public async Task Add(Session session) => await _dbContext.Sesssions.AddAsync(session);
    
    private readonly UnitOfWork.UserDbContext _dbContext;
}