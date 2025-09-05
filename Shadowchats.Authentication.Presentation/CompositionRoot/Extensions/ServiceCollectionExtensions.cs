// Shadowchats — Copyright (C) 2025
// Dorovskoy Alexey Vasilievich (One290 / 0ne290) <lenya.dorovskoy@mail.ru>
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version. See the LICENSE file for details.
// For full copyright and authorship information, see the COPYRIGHT file.

using Serilog;
using Serilog.Enrichers.Span;
using Shadowchats.Authentication.Core.Application.Interfaces;
using Shadowchats.Authentication.Core.Domain.Aggregates;
using Shadowchats.Authentication.Core.Domain.Interfaces;
using Shadowchats.Authentication.Infrastructure.Bus;
using Shadowchats.Authentication.Infrastructure.Bus.Decorators;
using Shadowchats.Authentication.Infrastructure.Identity;
using Shadowchats.Authentication.Infrastructure.Persistence;
using Shadowchats.Authentication.Infrastructure.Scheduling;
using Shadowchats.Authentication.Infrastructure.System;

namespace Shadowchats.Authentication.Presentation.CompositionRoot.Extensions;

public static class ServiceCollectionExtensions
{
    
    
    public static IServiceCollection
        AddInfrastructure(this IServiceCollection services, IConfiguration configuration) => services
        .AddSystem()
        .AddIdentity(configuration)
        .AddPersistence(configuration)
        .AddScheduling()
        .AddBus();

    private static IServiceCollection AddBus(this IServiceCollection services) => services
        .Decorate(typeof(IMessageHandler<,>), typeof(UnitOfWorkDecorator<,>))
        .Decorate(typeof(IMessageHandler<,>), typeof(LoggingDecorator<,>))
        .AddScoped<IBus, Bus>();

    private static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration) =>
        services
            .AddScoped<AuthenticationDbContext>(_ =>
                new AuthenticationDbContext(configuration.GetValue<string>("Persistence:PostgresConnectionString")))
            .AddScoped<IUnitOfWork, UnitOfWork>()
            .AddScoped<IPersistenceContext, PersistenceContext>()
            .AddScoped<IAggregateRootRepository<Account>, AccountRepository>()
            .AddScoped<IAggregateRootRepository<Session>, SessionRepository>();

    private static IServiceCollection AddSystem(this IServiceCollection services) => services
        .AddSingleton<IDateTimeProvider, DateTimeProvider>()
        .AddSingleton<IGuidGenerator, GuidGenerator>();

    private static IServiceCollection AddIdentity(this IServiceCollection services, IConfiguration configuration) =>
        services.Configure<JwtSettings>(configuration.GetSection("Identity:JwtSettings"))
            .PostConfigure<JwtSettings>(opts =>
                opts.SecretKey = Convert.FromBase64String(configuration["Identity:JwtSettings:SecretKey"]!))
            .AddSingleton<IRefreshTokenGenerator, RefreshTokenGenerator>()
            .AddSingleton<IAccessTokenIssuer, AccessTokenIssuer>()
            .AddSingleton<ISaltsManager, SaltsManager>()
            .AddSingleton<IPasswordHasher, PasswordHasher>();

    private static IServiceCollection AddScheduling(this IServiceCollection services) =>
        services.AddHostedService<RevokeExpiredSessionsScheduler>();

    public static IServiceCollection AddApplication(this IServiceCollection services) => services.Scan(scan =>
        scan.FromAssembliesOf(typeof(IBus))
            .AddClasses(c => c.AssignableTo(typeof(IMessageHandler<,>)))
            .AsImplementedInterfaces()
            .WithScopedLifetime());
    
    public static IServiceCollection AddCustomLogging(this IServiceCollection services)
    {
        Log.Logger = new LoggerConfiguration()
            .Enrich.FromLogContext()
            .Enrich.WithExceptionDetails()
            .Enrich.WithSpan()
            .MinimumLevel.Information()
            .WriteTo.Async(c => c.Console(new Serilog.Formatting.Json.JsonFormatter()))
            .CreateLogger();

        services.AddSerilog();

        return services;
    }
}