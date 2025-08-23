using Microsoft.EntityFrameworkCore;
using Shadowchats.Authentication.Core.Application.Interfaces;
using Shadowchats.Authentication.Core.Domain.Aggregates;

namespace Shadowchats.Authentication.Infrastructure.Persistence;

internal class AccountRepository : IAccountRepository
{
    public AccountRepository(AuthenticationDbContext dbContext) => _dbContext = dbContext;

    public async Task<Account?> GetByLogin(string login) =>
        await _dbContext.Accounts.FirstOrDefaultAsync(a => a.Credentials.Login == login);

    public async Task Add(Account account) => await _dbContext.Accounts.AddAsync(account);

    public async Task<bool> IsExistsWithLogin(string login) =>
        await _dbContext.Accounts.AnyAsync(a => a.Credentials.Login == login);
    
    private readonly AuthenticationDbContext _dbContext;
}