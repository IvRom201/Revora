using Backend.Domain.Entities;

namespace Backend.Infrastructure.Auth;

public interface IJwtService
{
    string GenerateToken(Employee employee);
}