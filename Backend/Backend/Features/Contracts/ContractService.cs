using Backend.Domain.Entities;
using Backend.Domain.Enums;
using Backend.Features.Contracts.DTOs;
using Backend.Features.Contracts.Pricing;
using Backend.Infrastructure.Database;
using Backend.Infrastructure.Time;
using Backend.Shared.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace Backend.Features.Contracts;

public class ContractService : IContractService
{
    private readonly AppDbContext _dbContext;
    private readonly IContractPriceCalculator _priceCalculator;
    private readonly IDateTimeProvider _dateTimeProvider;

    public ContractService(
        AppDbContext dbContext,
        IContractPriceCalculator priceCalculator,
        IDateTimeProvider dateTimeProvider)
    {
        _dbContext = dbContext;
        _priceCalculator = priceCalculator;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<ContractResponse> CreateAsync(CreateContractRequest request)
    {
        ValidateContractDates(request.StartDate, request.EndDate);
        ValidateSupportYears(request.AdditionalSupportYears);

        var client = await _dbContext.Clients
            .FirstOrDefaultAsync(c => c.Id == request.ClientId && !c.IsDeleted);

        if (client is null)
        {
            throw new NotFoundException($"Client with id {request.ClientId} was not found.");
        }

        var software = await _dbContext.SoftwareProducts
            .FirstOrDefaultAsync(s => s.Id == request.SoftwareProductId);

        if (software is null)
        {
            throw new NotFoundException($"Software product with id {request.SoftwareProductId} was not found.");
        }

        await EnsureClientHasNoActiveContractForSoftwareAsync(client.Id, software.Id);

        var today = _dateTimeProvider.Today;

        var price = await _priceCalculator.CalculateAsync(
            client,
            software,
            request.AdditionalSupportYears,
            today);

        var contract = new Contract
        {
            ClientId = client.Id,
            SoftwareProductId = software.Id,
            SoftwareVersion = software.CurrentVersion,
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            AdditionalSupportYears = request.AdditionalSupportYears,
            BasePrice = price.BasePrice,
            AdditionalSupportPrice = price.AdditionalSupportPrice,
            DiscountPercentage = price.DiscountPercentage,
            ReturningClientDiscountPercentage = price.ReturningClientDiscountPercentage,
            FinalPrice = price.FinalPrice,
            Status = ContractStatus.Offer,
            CreatedAt = _dateTimeProvider.UtcNow
        };

        _dbContext.Contracts.Add(contract);
        await _dbContext.SaveChangesAsync();

        return MapToResponse(contract);
    }

    public async Task<ContractResponse> GetByIdAsync(int id)
    {
        var contract = await _dbContext.Contracts
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == id);

        if (contract is null)
        {
            throw new NotFoundException($"Contract with id {id} was not found.");
        }

        return MapToResponse(contract);
    }

    public async Task DeleteAsync(int id)
    {
        var contract = await _dbContext.Contracts
            .FirstOrDefaultAsync(c => c.Id == id);

        if (contract is null)
        {
            throw new NotFoundException($"Contract with id {id} was not found.");
        }

        if (contract.Status == ContractStatus.Signed)
        {
            throw new BadRequestException("Signed contract cannot be deleted.");
        }

        contract.Status = ContractStatus.Cancelled;
        await _dbContext.SaveChangesAsync();
    }

    private async Task EnsureClientHasNoActiveContractForSoftwareAsync(int clientId, int softwareProductId)
    {
        var today = _dateTimeProvider.Today;
        var now = _dateTimeProvider.UtcNow;

        var hasActiveOffer = await _dbContext.Contracts
            .AnyAsync(c =>
                c.ClientId == clientId &&
                c.SoftwareProductId == softwareProductId &&
                c.Status == ContractStatus.Offer &&
                c.EndDate >= today);

        if (hasActiveOffer)
        {
            throw new ConflictException("Client already has an active offer for this software.");
        }

        var signedContracts = await _dbContext.Contracts
            .Where(c =>
                c.ClientId == clientId &&
                c.SoftwareProductId == softwareProductId &&
                c.Status == ContractStatus.Signed &&
                c.SignedAt != null)
            .ToListAsync();

        var hasActiveSignedContract = signedContracts.Any(c =>
            c.SignedAt!.Value.AddYears(1 + c.AdditionalSupportYears) >= now);

        if (hasActiveSignedContract)
        {
            throw new ConflictException("Client already has an active signed contract for this software.");
        }
    }

    private static void ValidateContractDates(DateOnly startDate, DateOnly endDate)
    {
        var days = endDate.DayNumber - startDate.DayNumber;

        if (days < 3)
        {
            throw new BadRequestException("Contract duration must be at least 3 days.");
        }

        if (days > 30)
        {
            throw new BadRequestException("Contract duration cannot be longer than 30 days.");
        }
    }

    private static void ValidateSupportYears(int additionalSupportYears)
    {
        if (additionalSupportYears is < 0 or > 3)
        {
            throw new BadRequestException("Additional support years must be 0, 1, 2 or 3.");
        }
    }

    private static ContractResponse MapToResponse(Contract contract)
    {
        return new ContractResponse
        {
            Id = contract.Id,
            ClientId = contract.ClientId,
            SoftwareProductId = contract.SoftwareProductId,
            SoftwareVersion = contract.SoftwareVersion,
            StartDate = contract.StartDate,
            EndDate = contract.EndDate,
            AdditionalSupportYears = contract.AdditionalSupportYears,
            BasePrice = contract.BasePrice,
            AdditionalSupportPrice = contract.AdditionalSupportPrice,
            DiscountPercentage = contract.DiscountPercentage,
            ReturningClientDiscountPercentage = contract.ReturningClientDiscountPercentage,
            FinalPrice = contract.FinalPrice,
            Status = contract.Status,
            CreatedAt = contract.CreatedAt,
            SignedAt = contract.SignedAt
        };
    }
}