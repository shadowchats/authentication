using Shadowchats.Authentication.Core.Domain.Aggregates;

namespace Shadowchats.Authentication.Core.Application.Interfaces;

internal interface IAccountRepository
{
    Task<Account?> GetByLogin(string login);
    Task Add(Account account);
    Task<bool> IsExistsWithLogin(string login);
}