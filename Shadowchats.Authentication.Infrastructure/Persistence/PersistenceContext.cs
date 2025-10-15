using Microsoft.EntityFrameworkCore;
using Npgsql;
using Shadowchats.Authentication.Core.Application.Exceptions;
using Shadowchats.Authentication.Core.Application.Interfaces;
using Shadowchats.Authentication.Core.Domain.Aggregates;
using Shadowchats.Authentication.Infrastructure.Persistence.AuthenticationDbContext;

namespace Shadowchats.Authentication.Infrastructure.Persistence;

public class PersistenceContext : IPersistenceContext
{
    public PersistenceContext(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    
    public async Task SaveChanges()
    {
        try
        {
            await _unitOfWork.DbContext.SaveChanges();
        }
        catch (DbUpdateException ex) when (ex.InnerException is PostgresException
                                           {
                                               SqlState: "23505", ConstraintName: "IX_Accounts_Login"
                                           })
        {
            throw new EntityAlreadyExistsException<Account, string>(a => a.Credentials.Login);
        }
    }
    
    private readonly IUnitOfWork _unitOfWork;
}