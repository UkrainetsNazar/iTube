using System.Text;
using Elastic.Clients.Elasticsearch;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Shared.Domain.Interfaces;
using StackExchange.Redis;
using VideoService.Application.Interfaces;
using VideoService.Infrastructure.BackgroundServices;
using VideoService.Infrastructure.Consumers;
using VideoService.Infrastructure.Persistence;
using VideoService.Infrastructure.Repositories;
using VideoService.Infrastructure.Search;
using VideoService.Infrastructure.Services;

namespace VideoService.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<VideoDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("VideoDb")));

        services.AddSingleton(_ => new ElasticsearchClient(
            new ElasticsearchClientSettings(new Uri(configuration["Elasticsearch:Uri"]!))
            .DefaultIndex(ElasticIndexInitializer.IndexName)));

        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IVideoRepository, VideoRepository>();
        services.AddScoped<ICommentRepository, CommentRepository>();
        services.AddScoped<IVideoReactionRepository, VideoReactionRepository>();
        services.AddScoped<IRecommendationFeedRepository, RecommendationFeedRepository>();
        services.AddScoped<IVideoSearchIndex, ElasticVideoSearchIndex>();
        services.AddScoped<IUserSubscriptionRepository, UserSubscriptionRepository>();

        services.AddSingleton<IConnectionMultiplexer>(_ =>
            ConnectionMultiplexer.Connect(configuration["Redis:ConnectionString"]!));
        services.AddScoped<IViewsBufferService, RedisViewsBufferService>();
        services.AddHostedService<ViewsSyncBackgroundService>();

        services.AddMassTransit(busConfigurator =>
        {
            busConfigurator.AddEntityFrameworkOutbox<VideoDbContext>(outboxConfigurator =>
            {
                outboxConfigurator.UsePostgres();
                outboxConfigurator.UseBusOutbox();
            });

            busConfigurator.AddConsumer<VideoPublishedIntegrationEventConsumer>();
            busConfigurator.AddConsumer<VideoDeletedIntegrationEventConsumer>();
            busConfigurator.AddConsumer<MediaProcessingCompletedConsumer>();
            busConfigurator.AddConsumer<UserSubscribedIntegrationEventConsumer>();
            busConfigurator.AddConsumer<UserUnsubscribedIntegrationEventConsumer>();

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

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = configuration["Jwt:Issuer"],
                    ValidateAudience = true,
                    ValidAudience = configuration["Jwt:Audience"],
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                        configuration["Jwt:Secret"] ?? throw new InvalidOperationException("Jwt:Secret is not configured."))),
                    ValidateLifetime = true
                };
            });

        services.AddAuthorization();

        return services;
    }
}