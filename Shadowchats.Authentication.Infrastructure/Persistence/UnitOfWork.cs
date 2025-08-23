using Microsoft.EntityFrameworkCore.Storage;
using Shadowchats.Authentication.Core.Domain.Exceptions;

namespace Shadowchats.Authentication.Infrastructure.Persistence;

internal class UnitOfWork : IUnitOfWork
{
    public UnitOfWork(AuthenticationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task Begin()
    {
        if (_transaction != null)
            throw new BugException("Transaction has already begun.");
        
        _transaction ??= await _dbContext.Database.BeginTransactionAsync();
    }

    public async Task Commit()
    {
        if (_transaction == null)
            throw new BugException("Transaction has not yet begun or has already finished.");

        await _dbContext.SaveChangesAsync();
        
        await _transaction.CommitAsync();
        
        await _transaction.DisposeAsync();
        
        _dbContext.ChangeTracker.Clear();

        _transaction = null;
    }

    public async Task Rollback()
    {
        if (_transaction == null)
            throw new BugException("Transaction has not yet begun or has already finished.");
        
        await _transaction.RollbackAsync();
        
        await _transaction.DisposeAsync();
        
        _dbContext.ChangeTracker.Clear();
        
        _transaction = null;
    }
    
    private readonly AuthenticationDbContext _dbContext;
    
    private IDbContextTransaction? _transaction;
}