
using AuthService.Domain.Entities;

namespace AuthService.Application.Interfaces;

public interface IJwtGenerator
{
    string GenerateAccessToken(User user);
    string GenerateRefreshToken();
}