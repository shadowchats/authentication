using Shadowchats.Authentication.Core.Application.Base;
using Shadowchats.Authentication.Core.Application.Interfaces;

namespace Shadowchats.Authentication.Core.Application.UseCases.Logout;

public record LogoutCommand : ICommand<NoResult>
{
    public required string RefreshToken { get; init; }
}