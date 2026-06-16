using Backend.Features.Revenue.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Features.Revenue;

[ApiController]
[Route("api/revenue")]
public class RevenueController : ControllerBase
{
    private readonly IRevenueService _revenueService;

    public RevenueController(IRevenueService revenueService)
    {
        _revenueService = revenueService;
    }

    [HttpGet("current")]
    public async Task<ActionResult<RevenueResponse>> GetCurrentRevenue(
        [FromQuery] int? softwareProductId,
        [FromQuery] string? currency)
    {
        var result = await _revenueService.GetCurrentRevenueAsync(softwareProductId, currency);
        return Ok(result);
    }

    [HttpGet("predicted")]
    public async Task<ActionResult<RevenueResponse>> GetPredictedRevenue(
        [FromQuery] int? softwareProductId,
        [FromQuery] string? currency)
    {
        var result = await _revenueService.GetPredictedRevenueAsync(softwareProductId, currency);
        return Ok(result);
    }
}