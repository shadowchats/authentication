using Microsoft.EntityFrameworkCore;

namespace Shadowchats.Authentication.Infrastructure.Persistence.AuthenticationDbContext;

public class AuthenticationDbContextReadWrite : AuthenticationDbContextBase<AuthenticationDbContextReadWrite>
{
    public AuthenticationDbContextReadWrite(DbContextOptions<AuthenticationDbContextReadWrite> options) : base(options) { }
}