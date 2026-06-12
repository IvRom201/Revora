using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Backend.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Backend.Domain.Entities;

[Index(nameof(ClientId), nameof(SoftwareProductId), nameof(Status))]
public class Contract
{
    [Key]
    public int Id { get; set; }

    public int ClientId { get; set; }

    [ForeignKey(nameof(ClientId))]
    public Client Client { get; set; } = null!;

    public int SoftwareProductId { get; set; }

    [ForeignKey(nameof(SoftwareProductId))]
    public SoftwareProduct SoftwareProduct { get; set; } = null!;

    [Required]
    [MaxLength(50)]
    public string SoftwareVersion { get; set; } = null!;

    [Column(TypeName = "date")]
    public DateOnly StartDate { get; set; }

    [Column(TypeName = "date")]
    public DateOnly EndDate { get; set; }

    [Range(0, 3)]
    public int AdditionalSupportYears { get; set; }

    [Precision(18, 2)]
    public decimal BasePrice { get; set; }

    [Precision(18, 2)]
    public decimal AdditionalSupportPrice { get; set; }

    [Precision(5, 2)]
    public decimal DiscountPercentage { get; set; }

    [Precision(5, 2)]
    public decimal ReturningClientDiscountPercentage { get; set; }

    [Precision(18, 2)]
    public decimal FinalPrice { get; set; }

    public ContractStatus Status { get; set; } = ContractStatus.Offer;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? SignedAt { get; set; }

    public ICollection<ContractPayment> Payments { get; set; } = new List<ContractPayment>();

    [NotMapped]
    public int TotalSupportYears => 1 + AdditionalSupportYears;
}