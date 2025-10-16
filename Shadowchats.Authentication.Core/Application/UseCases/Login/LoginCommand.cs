using Shadowchats.Authentication.Core.Application.Interfaces;

namespace Shadowchats.Authentication.Core.Application.UseCases.Login;

public record LoginCommand : ICommand<LoginResult>
{
    public required string Login { get; init; }

    public required string Password { get; init; }
}