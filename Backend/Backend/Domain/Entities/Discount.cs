using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Backend.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Backend.Domain.Entities;

public class Discount
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(150)]
    public string Name { get; set; } = null!;

    public DiscountType DiscountType { get; set; }

    [Precision(5, 2)]
    public decimal Percentage { get; set; }

    [Column(TypeName = "date")]
    public DateOnly StartDate { get; set; }

    [Column(TypeName = "date")]
    public DateOnly EndDate { get; set; }

    public int SoftwareProductId { get; set; }

    [ForeignKey(nameof(SoftwareProductId))]
    public SoftwareProduct SoftwareProduct { get; set; } = null!;
}