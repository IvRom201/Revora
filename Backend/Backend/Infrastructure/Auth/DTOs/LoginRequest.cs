using System.ComponentModel.DataAnnotations;

namespace Backend.Infrastructure.Auth.DTOs;

public record LoginRequest
{
    [Required]
    public string Login { get; set; } = null!;

    [Required]
    public string Password { get; set; } = null!;
}