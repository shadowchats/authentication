namespace Shadowchats.Authentication.Core.Application.Interfaces;

public interface IPersistenceContext
{
    Task SaveChanges();
}