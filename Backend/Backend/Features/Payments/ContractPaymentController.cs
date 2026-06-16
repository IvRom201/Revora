using Backend.Features.Payments.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Features.Payments;

[ApiController]
[Route("api/contracts/{contractId:int}/payments")]
[Authorize]
public class PaymentsController : ControllerBase
{
    private readonly IContractPaymentService _paymentService;

    public PaymentsController(IContractPaymentService paymentService)
    {
        _paymentService = paymentService;
    }

    [HttpPost]
    public async Task<ActionResult<ContractPaymentResponse>> PayForContract(
        int contractId,
        CreateContractPaymentRequest request)
    {
        var result = await _paymentService.PayForContractAsync(contractId, request);
        return Ok(result);
    }
}