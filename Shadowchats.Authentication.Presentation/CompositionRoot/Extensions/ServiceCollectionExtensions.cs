// Shadowchats — Copyright (C) 2025
// Dorovskoy Alexey Vasilievich (One290 / 0ne290) <lenya.dorovskoy@mail.ru>
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version. See the LICENSE file for details.
// For full copyright and authorship information, see the COPYRIGHT file.

using Microsoft.EntityFrameworkCore;
using Serilog;
using Serilog.Enrichers.Span;
using Shadowchats.Authentication.Core.Application.Interfaces;
using Shadowchats.Authentication.Core.Domain.Interfaces;
using Shadowchats.Authentication.Infrastructure.Bus;
using Shadowchats.Authentication.Infrastructure.Bus.Decorators;
using Shadowchats.Authentication.Infrastructure.Identity;
using Shadowchats.Authentication.Infrastructure.Persistence;
using Shadowchats.Authentication.Infrastructure.System;

namespace Shadowchats.Authentication.Presentation.CompositionRoot.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection
        AddInfrastructure(this IServiceCollection services, IConfiguration configuration) => services
        .AddBus()
        .AddPersistence(configuration)
        .AddSystem()
        .AddIdentity(configuration);
    
    private static IServiceCollection AddBus(this IServiceCollection services)
    {
        services.Decorate(typeof(ICommandHandler<,>), typeof(UnitOfWorkDecorator<,>));
        services.Decorate(typeof(ICommandHandler<,>), typeof(LoggingDecorator<,>));

        services.AddScoped<ICommandBus, CommandBus>();

        return services;
    }

    private static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AuthenticationDbContext>(options =>
            options.UseNpgsql(configuration.GetValue<string>("Persistence:PostgreConnectionString")));

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
    
    private static IServiceCollection AddIdentity(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<JwtSettings>(configuration.GetSection("Identity:JwtSettings"));
        services.PostConfigure<JwtSettings>(opts =>
            opts.SecretKey = Convert.FromBase64String(configuration["Identity:JwtSettings:SecretKey"]!));
        
        services.AddSingleton<IRefreshTokenGenerator, RefreshTokenGenerator>();
        services.AddSingleton<IAccessTokenIssuer, AccessTokenIssuer>();
        services.AddSingleton<ISaltsManager, SaltsManager>();
        services.AddSingleton<IPasswordHasher, PasswordHasher>();
        
        return services;
    }
    
    public static IServiceCollection AddApplication(this IServiceCollection services) => services.Scan(scan => scan
        .FromAssemblyOf<ICommandBus>()
        .AddClasses(c => c.AssignableTo(typeof(ICommandHandler<,>)))
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