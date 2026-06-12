using System.ComponentModel.DataAnnotations;
using Backend.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Backend.Domain.Entities;

[Index(nameof(Login), IsUnique = true)]
public class Employee
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string Login { get; set; } = null!;

    [Required]
    [MaxLength(500)]
    public string PasswordHash { get; set; } = null!;

    public EmployeeRole Role { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public bool IsActive { get; set; } = true;
}