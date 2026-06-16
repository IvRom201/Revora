using Backend.Features.Software.DTOs;

namespace Backend.Features.Software;

public interface ISoftwareService
{
    Task<List<SoftwareResponse>> GetAllAsync();

    Task<SoftwareResponse> GetByIdAsync(int id);
}