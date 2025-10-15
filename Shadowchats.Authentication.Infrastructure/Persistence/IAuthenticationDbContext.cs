using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Shadowchats.Authentication.Core.Domain.Aggregates;

namespace Shadowchats.Authentication.Infrastructure.Persistence;

public interface IAuthenticationDbContext
{
    Task<IDbContextTransaction> BeginTransaction();
    
    Task SaveChanges();
    
    DbSet<Account> Accounts { get; }
    DbSet<Session> Sessions { get; }
}