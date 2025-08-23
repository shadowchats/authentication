using Shadowchats.Authentication.Core.Domain.Base;

namespace Shadowchats.Authentication.Core.Application.Interfaces;

public interface IUnitOfWork : IInfrastructureService
{
    Task Begin();
    Task Commit();
    Task Rollback();
}