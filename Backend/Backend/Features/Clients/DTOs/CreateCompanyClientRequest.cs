using System.ComponentModel.DataAnnotations;

namespace Backend.Features.Clients.DTOs;

public record CreateCompanyClientRequest
{
    [Required]
    [MaxLength(200)]
    public string CompanyName { get; set; } = null!;

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

    [Required]
    [StringLength(10, MinimumLength = 10)]
    public string Krs { get; set; } = null!;
}