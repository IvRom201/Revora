using Backend.Domain.Entities;
using Backend.Domain.Enums;
using Backend.Features.Contracts.DTOs;
using Backend.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Backend.Features.Contracts.Pricing;

public class ContractPriceCalculator : IContractPriceCalculator
{
    private const decimal AdditionalSupportYearPrice = 1000m;
    private const decimal ReturningClientDiscount = 5m;

    private readonly AppDbContext _dbContext;

    public ContractPriceCalculator(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ContractPriceResult> CalculateAsync(
        Client client,
        SoftwareProduct softwareProduct,
        int additionalSupportYears,
        DateOnly contractCreationDate)
    {
        var basePrice = softwareProduct.YearlyLicensePrice;
        var supportPrice = additionalSupportYears * AdditionalSupportYearPrice;

        var price = basePrice + supportPrice;

        var bestDiscount = await _dbContext.Discounts
            .Where(d =>
                d.SoftwareProductId == softwareProduct.Id &&
                d.DiscountType == DiscountType.Contract &&
                d.StartDate <= contractCreationDate &&
                d.EndDate >= contractCreationDate)
            .OrderByDescending(d => d.Percentage)
            .Select(d => (decimal?)d.Percentage)
            .FirstOrDefaultAsync() ?? 0m;

        if (bestDiscount > 0)
        {
            price -= price * bestDiscount / 100m;
        }

        var isReturningClient = await _dbContext.Contracts
            .AnyAsync(c => c.ClientId == client.Id && c.Status == ContractStatus.Signed);

        var returningDiscount = isReturningClient ? ReturningClientDiscount : 0m;

        if (returningDiscount > 0)
        {
            price -= price * returningDiscount / 100m;
        }

        return new ContractPriceResult
        {
            BasePrice = basePrice,
            AdditionalSupportPrice = supportPrice,
            DiscountPercentage = bestDiscount,
            ReturningClientDiscountPercentage = returningDiscount,
            FinalPrice = Math.Round(price, 2)
        };
    }
}