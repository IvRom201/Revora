using System.ComponentModel.DataAnnotations;
using Backend.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Backend.Domain.Entities;

[Index(nameof(Pesel), IsUnique = true)]
[Index(nameof(Krs), IsUnique = true)]
public class Client
{
    [Key]
    public int Id { get; set; }

    public ClientType ClientType { get; set; }

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

    [MaxLength(11)]
    public string? Pesel { get; set; }

    [MaxLength(10)]
    public string? Krs { get; set; }

    public bool IsDeleted { get; set; } = false;

    public ICollection<Contract> Contracts { get; set; } = new List<Contract>();
}