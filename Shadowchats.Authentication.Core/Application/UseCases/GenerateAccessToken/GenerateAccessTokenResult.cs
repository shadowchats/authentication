namespace Shadowchats.Authentication.Core.Application.UseCases.GenerateAccessToken;

public record GenerateAccessTokenResult
{
    public required string AccessToken { get; init; }
}