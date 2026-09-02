using System.Text;
using MassTransit;
using MediaService.Application.Interfaces;
using MediaService.Infrastructure.Persistence;
using MediaService.Infrastructure.Processing;
using MediaService.Infrastructure.Repositories;
using MediaService.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
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

        services.Configure<FormOptions>(options =>
        {
            options.MultipartBodyLengthLimit = 2_000_000_000;
        });

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

        services.AddScoped<IVideoStorageService, MinioStorageService>();
        services.AddScoped<IVideoProcessor, FfmpegVideoProcessor>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        services.AddSingleton<InMemoryMediaProcessingQueue>();
        services.AddSingleton<IMediaProcessingQueue>(sp => sp.GetRequiredService<InMemoryMediaProcessingQueue>());
        services.AddHostedService<MediaProcessingBackgroundService>();

        return services;
    }
}