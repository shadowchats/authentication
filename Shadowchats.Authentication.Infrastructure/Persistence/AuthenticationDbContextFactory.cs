using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Shadowchats.Authentication.Infrastructure.Persistence;

public class AuthenticationDbContextFactory : IDesignTimeDbContextFactory<AuthenticationDbContext>
{
    public AuthenticationDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<AuthenticationDbContext>();

        optionsBuilder.UseNpgsql(args[0]);
        
        return new AuthenticationDbContext(optionsBuilder.Options);
    }
}