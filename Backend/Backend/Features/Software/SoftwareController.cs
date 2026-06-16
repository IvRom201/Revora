using Backend.Features.Software.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Features.Software;

[ApiController]
[Route("api/software")]
[Authorize]
public class SoftwareController : ControllerBase
{
    private readonly ISoftwareService _softwareService;

    public SoftwareController(ISoftwareService softwareService)
    {
        _softwareService = softwareService;
    }

    [HttpGet]
    public async Task<ActionResult<List<SoftwareResponse>>> GetAll()
    {
        var result = await _softwareService.GetAllAsync();
        return Ok(result);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<SoftwareResponse>> GetById(int id)
    {
        var result = await _softwareService.GetByIdAsync(id);
        return Ok(result);
    }
}