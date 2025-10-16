using Microsoft.EntityFrameworkCore;

namespace Shadowchats.Authentication.Infrastructure.Persistence.AuthenticationDbContext;

public class AuthenticationDbContextReadOnly : AuthenticationDbContextBase<AuthenticationDbContextReadOnly>
{
    public AuthenticationDbContextReadOnly(DbContextOptions<AuthenticationDbContextReadOnly> options) : base(options) { }
}