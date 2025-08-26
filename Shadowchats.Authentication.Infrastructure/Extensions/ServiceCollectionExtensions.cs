using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Shadowchats.Authentication.Core.Application.Interfaces;
using Shadowchats.Authentication.Core.Domain.Exceptions;
using Shadowchats.Authentication.Core.Domain.Interfaces;
using Shadowchats.Authentication.Infrastructure.Bus;
using Shadowchats.Authentication.Infrastructure.Bus.Decorators;
using Shadowchats.Authentication.Infrastructure.Identity;
using Shadowchats.Authentication.Infrastructure.Persistence;
using Shadowchats.Authentication.Infrastructure.System;

namespace Shadowchats.Authentication.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration) => services
        .AddLogging()
        .AddBus()
        .AddPersistence(configuration)
        .AddSystem()
        .AddIdentity()
        .AddHostedService<InfrastructureCleanupService>();
    
    private static IServiceCollection AddLogging(this IServiceCollection services)
    {
        Log.Logger = new LoggerConfiguration()
            .Enrich.FromLogContext()
            .Enrich.WithExceptionDetails()
            .MinimumLevel.Information()
            .WriteTo.Async(c => c.Console())
            .CreateLogger();

        services.AddSerilog();

        return services;
    }
    
    private static IServiceCollection AddBus(this IServiceCollection services)
    {
        services.Decorate(typeof(ICommandHandler<,>), typeof(UnitOfWorkDecorator<,>));
        services.Decorate(typeof(ICommandHandler<,>), typeof(LoggingDecorator<,>));

        services.AddScoped<ICommandBus, CommandBus>();

        return services;
    }

    private static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        var persistence = configuration.GetSection("Persistence");
        if (!persistence.Exists())
            throw new BugException("Configuration -> section \"Persistence\" not found.");
        var connectionString = persistence.GetValue<string>("PostgreConnectionString");
        if (connectionString == null)
            throw new BugException("Configuration -> key \"Persistence.PostgreConnectionString\" not found.");
        
        Action<DbContextOptionsBuilder> optionsAction = options => options.UseNpgsql(connectionString);
        var dbContextOptionsBuilder = new DbContextOptionsBuilder();
        optionsAction(dbContextOptionsBuilder);

        using var testDbContext = new AuthenticationDbContext(dbContextOptionsBuilder.Options);
        testDbContext.Database.EnsureCreated();

        services.AddDbContext<AuthenticationDbContext>(optionsAction);
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IAggregateRootsRepository, AggregateRootsRepository>();
        
        return services;
    }
    
    private static IServiceCollection AddSystem(this IServiceCollection services)
    {
        services.AddSingleton<IDateTimeProvider, DateTimeProvider>();
        services.AddSingleton<IGuidGenerator, GuidGenerator>();
        
        return services;
    }
    
    private static IServiceCollection AddIdentity(this IServiceCollection services)
    {
        services.AddSingleton<IRefreshTokenGenerator, RefreshTokenGenerator>();
        services.AddSingleton<IAccessTokenIssuer, AccessTokenIssuer>();
        services.AddSingleton<IPasswordHasher, PasswordHasher>();
        
        return services;
    }
}