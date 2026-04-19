using System.Security.Cryptography;
using System.Text;
using Application.Services;

namespace Infrastructure.Services;

public class TokenService : ITokenService
{
    public Task<string> GenerateToken(string userName)
    {
        if (string.IsNullOrWhiteSpace(userName))
        {
            throw new ArgumentException("User name is required.", nameof(userName));
        }

        var randomBytes = RandomNumberGenerator.GetBytes(32);
        var userBytes = Encoding.UTF8.GetBytes(userName);
        var timestampBytes = BitConverter.GetBytes(DateTimeOffset.UtcNow.ToUnixTimeMilliseconds());

        var payload = new byte[userBytes.Length + timestampBytes.Length + randomBytes.Length];
        Buffer.BlockCopy(userBytes, 0, payload, 0, userBytes.Length);
        Buffer.BlockCopy(timestampBytes, 0, payload, userBytes.Length, timestampBytes.Length);
        Buffer.BlockCopy(randomBytes, 0, payload, userBytes.Length + timestampBytes.Length, randomBytes.Length);

        using var sha256 = SHA256.Create();
        var hash = sha256.ComputeHash(payload);

        return Task.FromResult(Base64UrlEncode(hash));
    }

    private static string Base64UrlEncode(byte[] data)
    {
        return Convert.ToBase64String(data)
            .TrimEnd('=')
            .Replace('+', '-')
            .Replace('/', '_');
    }
}
