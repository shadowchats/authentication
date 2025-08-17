namespace Shadowchats.Authentication.Core.Domain.Interfaces;

public interface IUnitOfWork
{
    void Begin();
    void Commit();
    void Rollback();
}