using Backend.Domain.Entities;
using Backend.Features.Contracts.DTOs;

namespace Backend.Features.Contracts.Pricing;

public interface IContractPriceCalculator
{
    Task<ContractPriceResult> CalculateAsync(
        Client client,
        SoftwareProduct softwareProduct,
        int additionalSupportYears,
        DateOnly contractCreationDate);
}