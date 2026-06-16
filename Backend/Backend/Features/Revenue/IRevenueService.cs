using Backend.Features.Revenue.DTOs;

namespace Backend.Features.Revenue;

public interface IRevenueService
{
    Task<RevenueResponse> GetCurrentRevenueAsync(int? softwareProductId, string? currency);

    Task<RevenueResponse> GetPredictedRevenueAsync(int? softwareProductId, string? currency);
}