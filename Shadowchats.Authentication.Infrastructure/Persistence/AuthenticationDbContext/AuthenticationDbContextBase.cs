using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage;
using Shadowchats.Authentication.Core.Domain.Aggregates;
using Shadowchats.Authentication.Infrastructure.Persistence.Interfaces;

namespace Shadowchats.Authentication.Infrastructure.Persistence.AuthenticationDbContext;

public abstract class AuthenticationDbContextBase<TContext> : DbContext, IAuthenticationDbContext where TContext : DbContext
{
    protected AuthenticationDbContextBase(DbContextOptions<TContext> options) : base(options) { }

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

    private class SessionEntityTypeConfiguration : IEntityTypeConfiguration<Session>
    {
        public void Configure(EntityTypeBuilder<Session> builder)
        {
            builder.ToTable("Sessions");

            builder.HasKey(x => x.Guid);
            builder.Property(x => x.Guid).HasColumnName("Id");

            builder.Property(x => x.AccountId)
                .IsRequired();

            builder.Property(x => x.ExpiresAt)
                .IsRequired();

            builder.Property(x => x.RefreshToken)
                .IsRequired();

            builder.Property(x => x.IsActive)
                .IsRequired();

            builder.Ignore(x => x.DomainEvents);

            builder.HasIndex(x => x.RefreshToken)
                .IsUnique()
                .HasDatabaseName("IX_Sessions_RefreshToken");

            builder.HasIndex(x => x.AccountId)
                .HasDatabaseName("IX_Sessions_AccountId");

            builder.HasOne<Account>()
                .WithMany()
                .HasForeignKey(x => x.AccountId)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new AccountEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new SessionEntityTypeConfiguration());
        base.OnModelCreating(modelBuilder);
    }
    
    public Task<IDbContextTransaction> BeginTransaction() => Database.BeginTransactionAsync();
    
    public new Task SaveChanges() => SaveChangesAsync();

    public DbSet<Account> Accounts { get; set; } = null!;

    public DbSet<Session> Sessions { get; set; } = null!;
}