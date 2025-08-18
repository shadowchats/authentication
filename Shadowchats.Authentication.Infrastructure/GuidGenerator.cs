using Shadowchats.Authentication.Core.Domain.Interfaces;

namespace Shadowchats.Authentication.Infrastructure;

public class GuidGenerator : IGuidGenerator
{
    public Guid Generate() => Guid.NewGuid();
}