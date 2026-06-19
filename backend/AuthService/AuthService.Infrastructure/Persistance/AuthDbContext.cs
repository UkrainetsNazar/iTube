using AuthService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Infrastructure.Persistance;

public class AuthDbContext(DbContextOptions<AuthDbContext> options) : DbContext(options)
{
    public DbSet<User> UserProfiles => Set<User>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
}