using Backend.Features.Clients.DTOs;

namespace Backend.Features.Clients;

public interface IClientService
{
    Task<ClientResponse> CreateIndividualAsync(CreateIndividualClientRequest request);

    Task<ClientResponse> CreateCompanyAsync(CreateCompanyClientRequest request);

    Task<List<ClientResponse>> GetAllAsync();

    Task<ClientResponse> GetByIdAsync(int id);

    Task<ClientResponse> UpdateAsync(int id, UpdateClientRequest request);

    Task DeleteAsync(int id);
}