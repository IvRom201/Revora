using Backend.Features.Contracts.DTOs;

namespace Backend.Features.Contracts;

public interface IContractService
{
    Task<ContractResponse> CreateAsync(CreateContractRequest request);

    Task<ContractResponse> GetByIdAsync(int id);

    Task DeleteAsync(int id);
}