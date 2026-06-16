namespace Backend.Features.Revenue.DTOs;

public record RevenueResponse
{
    public decimal Amount { get; set; }

    public string Currency { get; set; } = null!;

    public decimal AmountInPln { get; set; }

    public decimal? ExchangeRate { get; set; }

    public int? SoftwareProductId { get; set; }

    public DateTime CalculatedAt { get; set; }
}