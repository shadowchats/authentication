// Shadowchats — Copyright (C) 2025
// Dorovskoy Alexey Vasilievich (One290 / 0ne290) <lenya.dorovskoy@mail.ru>
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version. See the LICENSE file for details.
// For full copyright and authorship information, see the COPYRIGHT file.

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage;
using Shadowchats.Authentication.Core.Domain.Aggregates;

namespace Shadowchats.Authentication.Infrastructure.Persistence.AuthenticationDbContext;

public abstract class Base<TContext> : DbContext, IAuthenticationDbContext where TContext : DbContext
{
    public Base(DbContextOptions<TContext> options) : base(options) { }

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
    
public class ReadWrite : Base<ReadWrite>
{
    public ReadWrite(DbContextOptions<ReadWrite> options) : base(options) { }
}

public class ReadOnly : Base<ReadOnly>
{
    public ReadOnly(DbContextOptions<ReadOnly> options) : base(options) { }
}

public class Factory : IDesignTimeDbContextFactory<ReadWrite>
{
    public ReadWrite CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<ReadWrite>();
        
        optionsBuilder.UseNpgsql(args.Length > 0 ? args[0] : "Host=.;Database=Dummy;");

        return new ReadWrite(optionsBuilder.Options);
    }
}