using Shadowchats.Authentication.Core.Application.Interfaces;

namespace Shadowchats.Authentication.Core.Application.UseCases.GenerateAccessToken;

public record GenerateAccessTokenQuery : IQuery<GenerateAccessTokenResult>
{
    public required string RefreshToken { get; init; }
}