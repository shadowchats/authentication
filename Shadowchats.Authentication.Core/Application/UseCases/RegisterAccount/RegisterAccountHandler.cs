using Shadowchats.Authentication.Core.Application.Base;
using Shadowchats.Authentication.Core.Application.Exceptions;
using Shadowchats.Authentication.Core.Application.Interfaces;
using Shadowchats.Authentication.Core.Domain.Aggregates;
using Shadowchats.Authentication.Core.Domain.Exceptions;
using Shadowchats.Authentication.Core.Domain.Interfaces;
using Shadowchats.Authentication.Core.Domain.ValueObjects;

namespace Shadowchats.Authentication.Core.Application.UseCases.RegisterAccount;

public class RegisterAccountHandler : IMessageHandler<RegisterAccountCommand, NoResult>
{
    public RegisterAccountHandler(IAggregateRootRepository<Account> accountRepository,
        IPersistenceContext persistenceContext, IPasswordHasher passwordHasher, IGuidGenerator guidGenerator)
    {
        _accountRepository = accountRepository;
        _persistenceContext = persistenceContext;
        _passwordHasher = passwordHasher;
        _guidGenerator = guidGenerator;
    }

    public async Task<NoResult> Handle(RegisterAccountCommand command)
    {
        var credentials = Credentials.Create(_passwordHasher, command.Login, command.Password);
        var account = Account.Create(_guidGenerator, credentials);

        await _accountRepository.Add(account);
        try
        {
            await _persistenceContext.SaveChanges();
        }
        catch (EntityAlreadyExistsException<Account, string> ex) when (ex.IsConflictOn(a => a.Credentials.Login))
        {
            throw new InvariantViolationException("Login and/or password is invalid.");
        }

        return NoResult.Value;
    }

    private readonly IAggregateRootRepository<Account> _accountRepository;

    private readonly IPersistenceContext _persistenceContext;

    private readonly IPasswordHasher _passwordHasher;

    private readonly IGuidGenerator _guidGenerator;
}