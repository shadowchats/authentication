using Newtonsoft.Json;
using Shadowchats.Authentication.Core.Application.Interfaces;
using Shadowchats.Authentication.Core.Domain.Aggregates;
using Shadowchats.Authentication.Core.Domain.Exceptions;
using Shadowchats.Authentication.Core.Domain.Interfaces;
using Shadowchats.Authentication.Core.Domain.ValueObjects;

namespace Shadowchats.Authentication.Core.Application.UseCases
{
    namespace RegisterAccount
    {
        public class RegisterAccountCommand : ICommand<RegisterAccountResult>
        {
            [JsonProperty(Required = Required.Always, PropertyName = "login")]
            public required string Login { get; init; }
            
            [JsonProperty(Required = Required.Always, PropertyName = "password")]
            public required string Password { get; init; }
        }
        
        public class RegisterAccountResult
        {
            [JsonProperty(Required = Required.Always, PropertyName = "message")]
            public required string Message { get; init; }
        }
        
        public class RegisterAccountHandler : ICommandHandler<RegisterAccountCommand, RegisterAccountResult>
        {
            public RegisterAccountHandler(IAccountRepository accountRepository, IPasswordHasher passwordHasher, IGuidGenerator guidGenerator)
            {
                _accountRepository = accountRepository;
                _passwordHasher = passwordHasher;
                _guidGenerator = guidGenerator;
            }

            public async Task<RegisterAccountResult> Handle(RegisterAccountCommand command)
            {
                if (await _accountRepository.IsExistsWithLogin(command.Login))
                    throw new InvariantViolationException("Account with this login already exists.");
                
                await _accountRepository.Add(Account.Create(_guidGenerator,
                    Credentials.Create(_passwordHasher, command.Login, command.Password)));

                return new RegisterAccountResult
                {
                    Message = "Account registered."
                };
            }

            private readonly IAccountRepository _accountRepository;
            
            private readonly IPasswordHasher _passwordHasher;
            
            private readonly IGuidGenerator _guidGenerator;
        }
    }
}