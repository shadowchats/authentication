using Shadowchats.Authentication.Core.Domain.Base;

namespace Shadowchats.Authentication.Core.Domain.Interfaces;

public interface IGuidGenerator : IInfrastructureService
{
    Guid Generate();
}