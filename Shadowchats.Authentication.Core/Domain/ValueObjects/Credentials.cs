using Shadowchats.Authentication.Core.Domain.Base;

namespace Shadowchats.Authentication.Core.Domain.ValueObjects;

public class Credentials : ValueObject
{
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Login;
        yield return PasswordHash;
    }
    
    public string Login { get; }
    public string PasswordHash { get; }
}