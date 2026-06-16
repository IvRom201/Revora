using Backend.Domain.Enums;
using Backend.Features.Revenue.DTOs;
using Backend.Infrastructure.Currency;
using Backend.Infrastructure.Database;
using Backend.Infrastructure.Time;
using Backend.Shared.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace Backend.Features.Revenue;

public class RevenueService : IRevenueService
{
    private readonly AppDbContext _dbContext;
    private readonly ICurrencyService _currencyService;
    private readonly IDateTimeProvider _dateTimeProvider;

    public RevenueService(
        AppDbContext dbContext,
        ICurrencyService currencyService,
        IDateTimeProvider dateTimeProvider)
    {
        _dbContext = dbContext;
        _currencyService = currencyService;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<RevenueResponse> GetCurrentRevenueAsync(int? softwareProductId, string? currency)
    {
        await ValidateSoftwareProductAsync(softwareProductId);

        var query = _dbContext.Contracts
            .AsNoTracking()
            .Where(c => c.Status == ContractStatus.Signed);

        if (softwareProductId.HasValue)
        {
            query = query.Where(c => c.SoftwareProductId == softwareProductId.Value);
        }

        var amountInPln = await query
            .Select(c => (decimal?)c.FinalPrice)
            .SumAsync() ?? 0m;

        var converted = await _currencyService.ConvertFromPlnAsync(amountInPln, currency);

        return new RevenueResponse
        {
            Amount = converted.Amount,
            Currency = converted.Currency,
            AmountInPln = amountInPln,
            ExchangeRate = converted.ExchangeRate,
            SoftwareProductId = softwareProductId,
            CalculatedAt = _dateTimeProvider.UtcNow
        };
    }

    public async Task<RevenueResponse> GetPredictedRevenueAsync(int? softwareProductId, string? currency)
    {
        await ValidateSoftwareProductAsync(softwareProductId);

        var today = _dateTimeProvider.Today;

        var query = _dbContext.Contracts
            .AsNoTracking()
            .Where(c =>
                c.Status == ContractStatus.Signed ||
                c.Status == ContractStatus.Offer && c.EndDate >= today);

        if (softwareProductId.HasValue)
        {
            query = query.Where(c => c.SoftwareProductId == softwareProductId.Value);
        }

        var amountInPln = await query
            .Select(c => (decimal?)c.FinalPrice)
            .SumAsync() ?? 0m;

        var converted = await _currencyService.ConvertFromPlnAsync(amountInPln, currency);

        return new RevenueResponse
        {
            Amount = converted.Amount,
            Currency = converted.Currency,
            AmountInPln = amountInPln,
            ExchangeRate = converted.ExchangeRate,
            SoftwareProductId = softwareProductId,
            CalculatedAt = _dateTimeProvider.UtcNow
        };
    }

    private async Task ValidateSoftwareProductAsync(int? softwareProductId)
    {
        if (!softwareProductId.HasValue)
        {
            return;
        }

        var exists = await _dbContext.SoftwareProducts
            .AnyAsync(s => s.Id == softwareProductId.Value);

        if (!exists)
        {
            throw new NotFoundException($"Software product with id {softwareProductId.Value} was not found.");
        }
    }
}