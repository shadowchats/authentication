using Shadowchats.Authentication.Core.Domain.Base;
using Shadowchats.Authentication.Core.Domain.Interfaces;

namespace Shadowchats.Authentication.Core.Domain.Aggregates;

public class Account : AggregateRoot
{
    public Account(ICredentialsValidator credentialsValidator, IPasswordHasher passwordHasher, Guid guid, string login, string password) : base(guid)
    {
        credentialsValidator.EnsureCredentialsValidity(password);
        if (lo)

        PasswordHash = passwordHasher.Hash(password);
    }

    private static void EnsurePasswordValidity(string password)
    {
        
    }
    
    
}
