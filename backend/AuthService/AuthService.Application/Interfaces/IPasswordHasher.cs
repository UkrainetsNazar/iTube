namespace AuthService.Application.Interfaces;

public interface IPasswordHasher
{
    string Hash(string password);
    string HashToken(string token);
    bool Verify(string password, string hash);
}