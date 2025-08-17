using Shadowchats.Authentication.Core.Domain.Base;

namespace Shadowchats.Authentication.Core.Domain.Interfaces;

public interface ICredentialsValidator : IDomainService
{
    void EnsureCredentialsValidity(string password);
}