using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shared.Domain.Interfaces;
using UserService.Application.Interfaces;
using UserService.Infrastructure.Consumers;
using UserService.Infrastructure.Persistence;
using UserService.Infrastructure.Repositories;

namespace UserService.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<UserDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("UserDb")));

        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IUserAccountRepository, UserAccountRepository>();
        services.AddScoped<IChannelRepository, ChannelRepository>();
        services.AddScoped<ISubscriptionRepository, SubscriptionRepository>();

        services.AddMassTransit(busConfigurator =>
        {
            busConfigurator.AddEntityFrameworkOutbox<UserDbContext>(outboxConfigurator =>
            {
                outboxConfigurator.UsePostgres();
                outboxConfigurator.UseBusOutbox();
            });

            busConfigurator.AddConsumer<EmailConfirmedConsumer>();

            busConfigurator.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host(configuration["RabbitMQ:Host"], "/", h =>
                {
                    h.Username(configuration["RabbitMQ:Username"]!);
                    h.Password(configuration["RabbitMQ:Password"]!);
                });

                cfg.ConfigureEndpoints(context);
            });
        });

        return services;
    }
}