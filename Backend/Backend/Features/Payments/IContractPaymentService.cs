using Backend.Features.Payments.DTOs;

namespace Backend.Features.Payments;

public interface IContractPaymentService
{
    Task<ContractPaymentResponse> PayForContractAsync(
        int contractId,
        CreateContractPaymentRequest request);
}