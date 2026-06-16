using System.ComponentModel.DataAnnotations;

namespace Backend.Features.Payments.DTOs;

public record CreateContractPaymentRequest
{
    public int ClientId { get; set; }

    [Range(0.01, double.MaxValue)]
    public decimal Amount { get; set; }
}