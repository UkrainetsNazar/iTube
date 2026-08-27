using MassTransit;
using MediaService.Application.Interfaces;
using MediaService.Infrastructure.Persistence;
using MediaService.Infrastructure.Processing;
using MediaService.Infrastructure.Repositories;
using MediaService.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Minio;
using Shared.Domain.Interfaces;

namespace MediaService.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<MediaDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("MediaDb")));

        services.AddScoped<IMediaAssetRepository, MediaAssetRepository>();

        services.AddMinio(client => client
            .WithEndpoint(configuration["Minio:Endpoint"])
            .WithCredentials(configuration["Minio:AccessKey"], configuration["Minio:SecretKey"])
            .WithSSL(bool.Parse(configuration["Minio:UseSsl"] ?? "false"))
            .Build());

        services.AddMassTransit(x =>
        {
            x.UsingRabbitMq((context, cfg) =>
            {
                var rabbitMqHost = configuration["RabbitMQ:Host"] ?? "localhost";
                cfg.Host(rabbitMqHost, "/", h =>
                {
                    h.Username(configuration["RabbitMQ:Username"] ?? "guest");
                    h.Password(configuration["RabbitMQ:Password"] ?? "guest");
                });
            });
        });

        services.AddScoped<IVideoStorageService, MinioStorageService>();
        services.AddScoped<IVideoProcessor, FfmpegVideoProcessor>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        services.AddSingleton<InMemoryMediaProcessingQueue>();
        services.AddSingleton<IMediaProcessingQueue>(sp => sp.GetRequiredService<InMemoryMediaProcessingQueue>());
        services.AddHostedService<MediaProcessingBackgroundService>();

        return services;
    }
}