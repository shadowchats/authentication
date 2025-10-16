namespace Shadowchats.Authentication.Core.Application.UseCases.Login;

public record LoginResult
{
    public required string RefreshToken { get; init; }

    public required string AccessToken { get; init; }
}