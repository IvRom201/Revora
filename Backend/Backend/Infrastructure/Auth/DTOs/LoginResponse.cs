namespace Backend.Infrastructure.Auth.DTOs;

public record LoginResponse
{
    public string Token { get; set; } = null!;

    public string Login { get; set; } = null!;

    public string Role { get; set; } = null!;
}