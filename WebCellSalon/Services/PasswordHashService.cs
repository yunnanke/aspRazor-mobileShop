using System.Security.Cryptography;
using System.Text;

namespace WebCellSalon.Services;

public class PasswordHashService
{
    public string CreateHash(string password)
    {
        var saltBytes = RandomNumberGenerator.GetBytes(16);
        var salt = Convert.ToHexString(saltBytes).ToLowerInvariant();
        var hash = ComputeHash(password, salt);
        return $"{salt}:{hash}";
    }

    public string ComputeHash(string password, string salt)
    {
        var payload = Encoding.UTF8.GetBytes(password + salt);
        var hash = SHA256.HashData(payload);
        return Convert.ToHexString(hash).ToLowerInvariant();
    }
}
