using Microsoft.EntityFrameworkCore;
using Shadowchats.Authentication.Core.Domain.Aggregates;

namespace Shadowchats.Authentication.Infrastructure.Persistence;

internal class SessionRepository
{
    public SessionRepository(AuthenticationDbContext dbContext) => _dbContext = dbContext;
    
    public async Task<Session?> GetByRefreshToken(string refreshToken) =>
        await _dbContext.Sesssions.FirstOrDefaultAsync(s => s.RefreshToken == refreshToken);

    public async Task Add(Session session) => await _dbContext.Sesssions.AddAsync(session);
    
    private readonly AuthenticationDbContext _dbContext;
}