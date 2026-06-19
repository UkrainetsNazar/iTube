using AuthService.Application.Interfaces;
using AuthService.Infrastructure.Persistance;
using AuthService.Infrastructure.Repositories;
using AuthService.Infrastructure.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AuthService.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<AuthDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("AuthDb")));

        services.AddScoped<IJwtGenerator, JwtGenerator>();
        services.AddScoped<IPasswordHasher, PasswordHasher>();

        return services;
    }
}