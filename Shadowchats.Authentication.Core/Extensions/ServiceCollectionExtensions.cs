using Microsoft.Extensions.DependencyInjection;
using Shadowchats.Authentication.Core.Application.Interfaces;
using Shadowchats.Authentication.Core.Application.UseCases.Login;

namespace Shadowchats.Authentication.Core.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services) => services.Scan(scan => scan
        .FromAssemblyOf<LoginHandler>()
        .AddClasses(c => c.AssignableTo(typeof(ICommandHandler<,>)))
        .AsImplementedInterfaces()
        .WithScopedLifetime());
}