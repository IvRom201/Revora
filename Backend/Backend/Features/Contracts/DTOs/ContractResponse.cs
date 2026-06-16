using Backend.Domain.Enums;

namespace Backend.Features.Contracts.DTOs;

public record ContractResponse
{
    public int Id { get; set; }

    public int ClientId { get; set; }

    public int SoftwareProductId { get; set; }

    public string SoftwareVersion { get; set; } = null!;

    public DateOnly StartDate { get; set; }

    public DateOnly EndDate { get; set; }

    public int AdditionalSupportYears { get; set; }

    public decimal BasePrice { get; set; }

    public decimal AdditionalSupportPrice { get; set; }

    public decimal DiscountPercentage { get; set; }

    public decimal ReturningClientDiscountPercentage { get; set; }

    public decimal FinalPrice { get; set; }

    public ContractStatus Status { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? SignedAt { get; set; }
}