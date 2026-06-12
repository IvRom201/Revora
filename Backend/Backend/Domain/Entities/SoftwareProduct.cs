using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Backend.Domain.Entities;

[Index(nameof(Name), IsUnique = true)]
public class SoftwareProduct
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(150)]
    public string Name { get; set; } = null!;

    [Required]
    [MaxLength(1000)]
    public string Description { get; set; } = null!;

    [Required]
    [MaxLength(50)]
    public string CurrentVersion { get; set; } = null!;

    [Required]
    [MaxLength(100)]
    public string Category { get; set; } = null!;

    [Precision(18, 2)]
    public decimal YearlyLicensePrice { get; set; }

    public ICollection<Discount> Discounts { get; set; } = new List<Discount>();

    public ICollection<Contract> Contracts { get; set; } = new List<Contract>();
}