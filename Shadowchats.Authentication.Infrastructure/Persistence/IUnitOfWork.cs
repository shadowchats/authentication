namespace Shadowchats.Authentication.Infrastructure.Persistence;

internal interface IUnitOfWork
{
    Task Begin();
    Task Commit();
    Task Rollback();
}