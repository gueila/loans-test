using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using FinTech.API.DTOs.Requests;
using FinTech.API.DTOs.Responses;
using FinTech.API.Repositories.Interfaces;
using FinTech.API.Services.Interfaces;
using Microsoft.IdentityModel.Tokens;

namespace FinTech.API.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IConfiguration _configuration;

    public AuthService(IUserRepository userRepository, IConfiguration configuration)
    {
        _userRepository = userRepository;
        _configuration = configuration;
    }

    public async Task<AuthResponse?> LoginAsync(LoginRequest request)
    {
        var user = await _userRepository.GetByEmailAsync(request.Email);
        if (user is null)
            return null;

        if (user.PasswordHash != request.Password)
            return null;

        var token = GenerateJwtToken(user);

        return new AuthResponse
        {
            Token = token,
            UserId = user.Id,
            Name = user.Name,
            Email = user.Email
        };
    }

    private static SymmetricSecurityKey GetSigningKey(IConfiguration config)
    {
        var secret = config["Jwt:Key"] ?? "SuperSecretKeyForDevelopment12345678!";
        var keyBytes = HMACSHA256.HashData(Encoding.UTF8.GetBytes(secret), new byte[32]);
        return new SymmetricSecurityKey(keyBytes);
    }

    private string GenerateJwtToken(Models.User user)
    {
        var credentials = new SigningCredentials(GetSigningKey(_configuration), SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Name, user.Name)
        };

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"] ?? "FinTech.API",
            audience: _configuration["Jwt:Audience"] ?? "FinTech.App",
            claims: claims,
            expires: DateTime.UtcNow.AddHours(24),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
