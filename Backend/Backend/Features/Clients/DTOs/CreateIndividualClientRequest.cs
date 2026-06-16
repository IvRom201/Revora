using System.ComponentModel.DataAnnotations;

namespace Backend.Features.Clients.DTOs;

public record CreateIndividualClientRequest
{
    [Required]
    [MaxLength(100)]
    public string FirstName { get; set; } = null!;

    [Required]
    [MaxLength(100)]
    public string LastName { get; set; } = null!;

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
    [StringLength(11, MinimumLength = 11)]
    public string Pesel { get; set; } = null!;
}