using Shadowchats.Authentication.Core.Application.Base;
using Shadowchats.Authentication.Core.Application.Interfaces;

namespace Shadowchats.Authentication.Core.Application.UseCases.LogoutAll;

public record LogoutAllCommand : ICommand<NoResult>
{
    public required string Login { get; init; }

    public required string Password { get; init; }
}