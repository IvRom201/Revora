namespace Backend.Infrastructure.Currency;

public interface ICurrencyService
{
    Task<CurrencyConversionResult> ConvertFromPlnAsync(decimal amountInPln, string? currency);
}

public class CurrencyConversionResult
{
    public decimal Amount { get; set; }

    public string Currency { get; set; } = null!;

    public decimal? ExchangeRate { get; set; }
}