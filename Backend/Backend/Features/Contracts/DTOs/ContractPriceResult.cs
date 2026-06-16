namespace Backend.Features.Contracts.DTOs;

public record ContractPriceResult
{
    public decimal BasePrice { get; set; }

    public decimal AdditionalSupportPrice { get; set; }

    public decimal DiscountPercentage { get; set; }

    public decimal ReturningClientDiscountPercentage { get; set; }

    public decimal FinalPrice { get; set; }
}