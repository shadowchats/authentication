using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Shadowchats.Authentication.Infrastructure.Persistence.AuthenticationDbContext;

[UsedImplicitly]
public class AuthenticationDbContextReadWriteFactory : IDesignTimeDbContextFactory<AuthenticationDbContextReadWrite>
{
    public AuthenticationDbContextReadWrite CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<AuthenticationDbContextReadWrite>();
        
        optionsBuilder.UseNpgsql(args.Length > 0 ? args[0] : "Host=.;Database=Dummy;");

        return new AuthenticationDbContextReadWrite(optionsBuilder.Options);
    }
}