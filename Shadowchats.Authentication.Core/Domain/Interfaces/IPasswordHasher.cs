namespace Shadowchats.Authentication.Core.Domain.Interfaces;

public interface IPasswordHasher
{
    string Hash(string password);
    bool Verify(string dynamicSaltAndHashedPassword, string password);
}
