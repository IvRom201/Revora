using Backend.Shared.Exceptions;

namespace Backend.Infrastructure.Currency;

public class NbpCurrencyService : ICurrencyService
{
    private readonly HttpClient _httpClient;

    public NbpCurrencyService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<CurrencyConversionResult> ConvertFromPlnAsync(decimal amountInPln, string? currency)
    {
        var currencyCode = string.IsNullOrWhiteSpace(currency)
            ? "PLN"
            : currency.Trim().ToUpperInvariant();

        if (currencyCode == "PLN")
        {
            return new CurrencyConversionResult
            {
                Amount = amountInPln,
                Currency = "PLN",
                ExchangeRate = 1m
            };
        }

        var rate = await GetExchangeRateAsync(currencyCode);

        var convertedAmount = amountInPln / rate;

        return new CurrencyConversionResult
        {
            Amount = Math.Round(convertedAmount, 2),
            Currency = currencyCode,
            ExchangeRate = rate
        };
    }

    private async Task<decimal> GetExchangeRateAsync(string currencyCode)
    {
        try
        {
            var response = await _httpClient.GetFromJsonAsync<NbpRateResponse>(
                $"https://api.nbp.pl/api/exchangerates/rates/a/{currencyCode}/?format=json");

            var rate = response?.Rates.FirstOrDefault()?.Mid;

            if (rate is null or <= 0)
            {
                throw new BadRequestException($"Exchange rate for currency '{currencyCode}' was not found.");
            }

            return rate.Value;
        }
        catch (HttpRequestException)
        {
            throw new BadRequestException($"Currency '{currencyCode}' is not supported or exchange rate service is unavailable.");
        }
    }

    private class NbpRateResponse
    {
        public string Code { get; set; } = null!;

        public List<NbpRate> Rates { get; set; } = new();
    }

    private class NbpRate
    {
        public decimal Mid { get; set; }
    }
}