using FinTech.API.DTOs.Requests;
using FinTech.API.DTOs.Responses;

namespace FinTech.API.Services.Interfaces;

public interface IAuthService
{
    Task<AuthResponse?> LoginAsync(LoginRequest request);
}
