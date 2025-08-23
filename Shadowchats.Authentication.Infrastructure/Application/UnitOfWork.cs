using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage;
using Shadowchats.Authentication.Core.Application.Interfaces;
using Shadowchats.Authentication.Core.Domain.Aggregates;

namespace Shadowchats.Authentication.Infrastructure.Application;

public class UnitOfWork : IUnitOfWork
{
    public class UserDbContext(DbContextOptions options) : DbContext(options)
    {
        private class AccountEntityTypeConfiguration : IEntityTypeConfiguration<Account>
        {
            public void Configure(EntityTypeBuilder<Account> builder)
            {
                builder.ToTable("Accounts");
        
                builder.HasKey(x => x.Guid);
                builder.Property(x => x.Guid).HasColumnName("Id");
        
                builder.OwnsOne(x => x.Credentials, credentialsBuilder =>
                {
                    credentialsBuilder.Property(c => c.Login)
                        .HasColumnName("Login")
                        .IsRequired();
                
                    credentialsBuilder.Property(c => c.PasswordHash)
                        .HasColumnName("PasswordHash")
                        .IsRequired();
                
                    credentialsBuilder.HasIndex(c => c.Login)
                        .IsUnique()
                        .HasDatabaseName("IX_Accounts_Login");
                });
        
                builder.Ignore(x => x.DomainEvents);
            }
        }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new AccountEntityTypeConfiguration());
            base.OnModelCreating(modelBuilder);
        }

        public DbSet<Account> Accounts { get; set; } = null!;
        
        public DbSet<Session> Sesssions { get; set; } = null!;
    }

    public UnitOfWork(UserDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task Begin() => _transaction = await _dbContext.Database.BeginTransactionAsync();

    public async Task Commit()
    {
        try
        {
            await _dbContext.SaveChangesAsync();
            if (_transaction != null)
                await _transaction.CommitAsync();
        }
        finally
        {
            if (_transaction != null)
                await _transaction.DisposeAsync();
        }
    }

    public async Task Rollback()
    {
        if (_transaction != null)
            await _transaction.RollbackAsync();
    }

    public void Dispose()
    {
        _transaction?.Dispose();
        _dbContext.Dispose();
    }
    
    private readonly UserDbContext _dbContext;
    
    private IDbContextTransaction? _transaction;
}