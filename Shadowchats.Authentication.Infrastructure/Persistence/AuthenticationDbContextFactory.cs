using Microsoft.EntityFrameworkCore.Design;

namespace Shadowchats.Authentication.Infrastructure.Persistence;

public class AuthenticationDbContextFactory : IDesignTimeDbContextFactory<AuthenticationDbContext>
{
    public AuthenticationDbContext CreateDbContext(string[] args) => new(args[0]);
}