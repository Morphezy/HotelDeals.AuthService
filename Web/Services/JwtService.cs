using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Application.Services;
using Microsoft.IdentityModel.Tokens;

namespace Web.Services;

public class JwtService(IConfiguration configuration) : ITokenService
{
    public Task<string> GenerateToken(string userName)
    {
        if (string.IsNullOrWhiteSpace(userName))
        {
            throw new ArgumentException("User name is required.", nameof(userName));
        }

        var jwtKey = configuration["JwtKey"] ?? throw new InvalidOperationException("JwtKey is not configured.");
        var jwtIssuer = configuration["JwtIssuer"] ?? throw new InvalidOperationException("JwtIssuer is not configured.");
        var jwtAudience = configuration["JwtAudience"] ?? throw new InvalidOperationException("JwtAudience is not configured.");

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, userName),
            new(JwtRegisteredClaimNames.UniqueName, userName),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var signingCredentials = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
            SecurityAlgorithms.HmacSha256);

        var tokenDescriptor = new JwtSecurityToken(
            issuer: jwtIssuer,
            audience: jwtAudience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(30),
            signingCredentials: signingCredentials);

        var token = new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
        return Task.FromResult(token);
    }
}
