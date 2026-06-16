namespace Backend.Features.Payments.DTOs;

public record ContractPaymentResponse
{
    public int Id { get; set; }

    public int ContractId { get; set; }

    public int ClientId { get; set; }

    public decimal Amount { get; set; }

    public DateTime PaymentDate { get; set; }

    public bool IsRefunded { get; set; }

    public bool ContractSigned { get; set; }

    public decimal TotalPaid { get; set; }

    public decimal RequiredAmount { get; set; }
}