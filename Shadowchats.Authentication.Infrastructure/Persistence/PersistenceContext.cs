using Microsoft.EntityFrameworkCore;
using Npgsql;
using Shadowchats.Authentication.Core.Application.Exceptions;
using Shadowchats.Authentication.Core.Application.Interfaces;
using Shadowchats.Authentication.Core.Domain.Aggregates;

namespace Shadowchats.Authentication.Infrastructure.Persistence;

public class PersistenceContext(AuthenticationDbContext dbContext) : IPersistenceContext
{
    public async Task SaveChanges()
    {
        try
        {
            await dbContext.SaveChangesAsync();
        }
        catch (DbUpdateException ex) when (ex.InnerException is PostgresException
                                           {
                                               SqlState: "23505", ConstraintName: "IX_Accounts_Login"
                                           })
        {
            throw new EntityAlreadyExistsException<Account, string>(a => a.Credentials.Login);
        }
    }
}