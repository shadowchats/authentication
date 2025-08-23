using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shadowchats.Authentication.Core.Domain.Aggregates;

namespace Shadowchats.Authentication.Infrastructure.Persistence;

internal class AuthenticationDbContext(DbContextOptions options) : DbContext(options)
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