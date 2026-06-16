using Backend.Infrastructure.Auth.DTOs;

namespace Backend.Infrastructure.Auth;

public interface IAuthService
{
    Task<LoginResponse> LoginAsync(LoginRequest request);
}