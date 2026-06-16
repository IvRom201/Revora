using System.ComponentModel.DataAnnotations;

namespace Backend.Features.Clients.DTOs;

public record UpdateClientRequest
{
    [MaxLength(100)]
    public string? FirstName { get; set; }

    [MaxLength(100)]
    public string? LastName { get; set; }

    [MaxLength(200)]
    public string? CompanyName { get; set; }

    [Required]
    [MaxLength(300)]
    public string Address { get; set; } = null!;

    [Required]
    [EmailAddress]
    [MaxLength(150)]
    public string Email { get; set; } = null!;

    [Required]
    [Phone]
    [MaxLength(30)]
    public string PhoneNumber { get; set; } = null!;
}